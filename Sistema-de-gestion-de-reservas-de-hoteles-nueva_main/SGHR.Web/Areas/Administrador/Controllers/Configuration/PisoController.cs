using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.Interfaces.Configuration;

namespace SGHR.Web.Areas.Administrador.Controllers.Configuration
{
    [Area("Administrador")]
    public class PisosController : Controller
    {
        private readonly IPisoService _pisoService;

        public PisosController(IPisoService pisoService)
        {
            _pisoService = pisoService;
        }

        // GET: Pisos
        public async Task<IActionResult> Index()
        {
            var result = await _pisoService.GetAllAsync();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Pisos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _pisoService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Pisos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pisos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePisoDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _pisoService.CreateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Pisos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _pisoService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            var piso = result.Data;

            var updateDto = new UpdatePisoDTO
            {
                Id = piso.Id,
                Numero = piso.Numero,
                Descripcion = piso.Descripcion,
                Estado = piso.Estado
            };

            return View(updateDto);
        }

        // POST: Pisos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePisoDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _pisoService.UpdateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Pisos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pisoService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: Pisos/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleteDto = new DeletePisoDTO { Id = id };

            var result = await _pisoService.RemoveAsync(deleteDto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View("Delete");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
