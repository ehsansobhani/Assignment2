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

namespace Assignment2.Controllers
{
    public class ClientsController : Controller
    {
        private  MarketDbContext _context;

        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string? ID)
        {
            var viewModel = new BrokerageViewModel
            {
                Clients = await _context.Clients
                 .Include(i => i.Subscriptions)
                 .AsNoTracking()
                 .OrderBy(i => i.ID)
                 .ToListAsync()

            };

            if (ID != null)
            {
                /*select linq query */
                var brokerages = from cust in _context.Subscriptions
                              where cust.ClientId == Int16.Parse(ID) 
                              select cust.Brokerage;
                viewModel.Brokerages = brokerages;
            }

            return View(viewModel);

            // return View(await _context.Clients.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstName,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstName,BirthDate")] Client client)
        {
            if (id != client.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ID))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);

            
        }

        public async Task<IActionResult> Editsubscribe( int id, string? title , int functionID )
        {
            if (functionID == 1)
            {
                if (_context.Subscriptions == null)
                {
                    return Problem("Entity set 'MarketDbContext.Subscription'  is null.");
                }
                var brokerId = from cust in _context.Brokerages where cust.Title.Contains(title) select cust.Id;
                string brokerToremoveId = brokerId.First().ToString();
                var subToRemove = from cust in _context.Subscriptions
                                  where cust.ClientId == id && cust.BrokerageId.Contains(brokerToremoveId)
                                  select cust;



                Subscription sub = subToRemove.FirstOrDefault();
                if (sub != null)
                {
                    _context.Subscriptions.Remove(sub);
                }
            }else if(functionID == 2)
            {
                var brokerId = from cust in _context.Brokerages where cust.Title.Contains(title) select cust.Id;
                Subscription sub = new Subscription();
                sub.ClientId = id;
                string brokerToAddId = brokerId.First().ToString();
                sub.BrokerageId = brokerToAddId;
                _context.Subscriptions.Add(sub);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditSubscription(int id)
        {
            var viewModel = new BrokerageViewModel
            {
              
            };

            if (id != null)
            {
                var clients = from cust in _context.Clients where cust.ID == id select cust;

                viewModel.Clients = clients;
                /*select linq query */
                var brokerages = from cust in _context.Subscriptions
                                 where cust.ClientId == id
                                 select cust.Brokerage;
                viewModel.Brokerages = brokerages;
            }

            return View(viewModel);

        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'MarketDbContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
          return _context.Clients.Any(e => e.ID == id);
        }
    }
}
