using Microsoft.AspNetCore.Mvc;

namespace SGHR.Web.Areas.Recepcionista.Controllers
{
    [Area("Recepcionista")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
