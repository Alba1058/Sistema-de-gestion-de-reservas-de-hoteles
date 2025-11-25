using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.Interfaces.Configuration;

namespace SGHR.Web.Areas.Administrador.Controllers.Configuration
{
    [Area("Administrador")]
    public class RolesUsuarioController : Controller
    {
        private readonly IRolUsuarioService _rolUsuarioService;

        public RolesUsuarioController(IRolUsuarioService rolUsuarioService)
        {
            _rolUsuarioService = rolUsuarioService;
        }

        // GET: RolesUsuario
        public async Task<IActionResult> Index()
        {
            var result = await _rolUsuarioService.GetAllAsync();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: RolesUsuario/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _rolUsuarioService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: RolesUsuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RolesUsuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRolUsuarioDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _rolUsuarioService.CreateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RolesUsuario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _rolUsuarioService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            var item = result.Data;

            var updateDto = new UpdateRolUsuarioDTO
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion,
                Estado = item.Estado
            };

            return View(updateDto);
        }

        // POST: RolesUsuario/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateRolUsuarioDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _rolUsuarioService.UpdateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RolesUsuario/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rolUsuarioService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: RolesUsuario/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleteDto = new DeleteRolUsuarioDTO { Id = id };

            var result = await _rolUsuarioService.RemoveAsync(deleteDto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View("Delete");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
