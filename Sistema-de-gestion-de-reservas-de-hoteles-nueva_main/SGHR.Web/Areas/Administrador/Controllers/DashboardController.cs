using Microsoft.AspNetCore.Mvc;

namespace SGHR.Web.Areas.Administrador.Controllers
{
    [Area("Administrador")]

    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}