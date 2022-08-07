using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lab4.Controllers
{
    public class HomeController : Controller
    {
        private readonly MarketDbContext _context;

        public HomeController(MarketDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();

        }
    }
}
