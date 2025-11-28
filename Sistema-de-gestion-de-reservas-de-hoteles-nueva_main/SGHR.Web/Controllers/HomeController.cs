using Microsoft.AspNetCore.Mvc;
using SGHR.Web.Models;
using System.Diagnostics;

namespace SGHR.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Authentication");
        }

        public IActionResult Api()
        {
            var apiUrl = "http://localhost:5066/swagger";
            return Redirect(apiUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message = null)
        {
            var viewModel = new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            };
            
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.ErrorMessage = message;
            }
            
            return View(viewModel);
        }
    }
}
