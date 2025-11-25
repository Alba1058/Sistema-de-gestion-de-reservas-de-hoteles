using Microsoft.AspNetCore.Mvc;

namespace SGHR.Web.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
