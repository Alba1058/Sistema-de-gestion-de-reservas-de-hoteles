using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Application.Interfaces.Reservas;
using System.Threading.Tasks;

namespace SGHR.Web.Areas.Administrador.Controllers.Reservas
{
    [Area("Administrador")]
    public class ServicioAdicionalController : Controller
    {
        private readonly IServicioAdicionalService _servicioAdicionalService;

        public ServicioAdicionalController(IServicioAdicionalService servicioAdicionalService)
        {
            _servicioAdicionalService = servicioAdicionalService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _servicioAdicionalService.GetAllAsync();
            return View(result.Data);
        }

        public IActionResult Create()
        {
            return View(new CreateServicioAdicionalDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateServicioAdicionalDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _servicioAdicionalService.CreateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _servicioAdicionalService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectToAction("Index");

            var dto = new UpdateServicioAdicionalDTO
            {
                Id = result.Data.Id,
                Nombre = result.Data.Nombre,
                Precio = result.Data.Precio,
                Estado = result.Data.Estado,
                Descripcion = result.Data.Descripcion
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateServicioAdicionalDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _servicioAdicionalService.UpdateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _servicioAdicionalService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectToAction("Index");

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _servicioAdicionalService.RemoveAsync(new DeleteServicioAdicionalDTO { Id = id });

            return RedirectToAction("Index");
        }
    }
}
