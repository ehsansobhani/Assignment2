using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;
using Azure.Storage.Blobs;
using Azure;
using System.Collections;

namespace Assignment2.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly MarketDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private Brokerage broker;

        public AdvertisementsController(BlobServiceClient blobServiceClient,MarketDbContext context)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Advertisements
        public async Task<IActionResult> Index(string? Id)
        {
            var viewModel = new FileInputViewModel
            {

                BrokerageId = Id ,
                 
          };

            if (Id != null)
            {
                var advertisements = from cust in _context.Advertisements
                              where cust.Brokerage.Id == Id
                              select cust;
                viewModel.Advertisements = advertisements;
                var broker  = from cust in _context.Brokerages where cust.Id == Id select cust .Title;
                viewModel.BrokerageTitle = broker.FirstOrDefault();
            }
            return View(viewModel);
        }

        // GET: Advertisements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Advertisements == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AnswerImageId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // GET: Advertisements/Create
        public IActionResult Create(string? Id)
        {
            var viewModel = new FileInputViewModel
            {
                
                BrokerageId = Id

            };

            if (Id != null)
            {

                viewModel.Advertisements = _context.Brokerages.Where(
                    x => x.Id == Id).Single().Advertisements;
            }

            return View(viewModel);
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       // public async Task<IActionResult> Create([Bind("AnswerImageId,FileName,Url")] Advertisement advertisement)
       public async Task<IActionResult> Create(IFormFile? file , string? Id)
        {
            
            //////////////////////////////////
            BlobContainerClient containerClient;
            // Create the container and return a container client object
            var containerName = "ehsanjos";
            
            try
            {
                
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException e)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                string randomFileName = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }
                
                // add the photo to the database if it uploaded successfully
                var brokerage = await _context.Brokerages
                .FirstOrDefaultAsync(m => m.Id == Id);
                var advertisement = new Advertisement
                {
                    Url = blockBlob.Uri.AbsoluteUri,
                    FileName = randomFileName,
                    Brokerage = brokerage
                };
                _context.Advertisements.Add(advertisement);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/pages/Error");
            }

            var viewModel = new FileInputViewModel
            {

                BrokerageId = Id

            };

            if (Id != null)
            {

                viewModel.Advertisements = _context.Brokerages.Where(
                    x => x.Id == Id).Single().Advertisements;
            }


            // return View(viewModel);
            //return RedirectToAction(nameof(Index(viewModel)));
            return RedirectToAction("Index", "Brokerages");
        }

        // GET: Advertisements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Advertisements == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AnswerImageId,FileName,Url")] Advertisement advertisement)
        {
            if (id != advertisement.AnswerImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advertisement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertisementExists(advertisement.AnswerImageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        public async Task<IActionResult> Delete(int? id , string? BrokerageTitle)
        {
            if (id == null || _context.Advertisements == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AnswerImageId == id);
            if (advertisement == null)
            {
                return NotFound();
            }
           
            BrokerageAdvertisementViewModel modelToDel =  new BrokerageAdvertisementViewModel();
            modelToDel.Brokerages = from cust in _context.Brokerages where BrokerageTitle == cust.Title select cust ;
            modelToDel.Advertisements = from cust in _context.Advertisements where cust.AnswerImageId == id select cust;


            return View(modelToDel);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Advertisements == null)
            {
                return Problem("Entity set 'MarketDbContext.Advertisements'  is null.");
            }
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement != null)
            {
                _context.Advertisements.Remove(advertisement);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Brokerages");
        }

        private bool AdvertisementExists(int id)
        {
          return _context.Advertisements.Any(e => e.AnswerImageId == id);
        }
    }
}
