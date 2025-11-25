using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Interfaces.Usuarios;

namespace SGHR.Web.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var result = await _usuarioService.GetAllAsync();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _usuarioService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _usuarioService.CreateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _usuarioService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            var u = result.Data;

            var updateDto = new UsuarioUpdateDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Activo = u.Activo,
                RolUsuarioId = u.RolUsuarioId
            };

            return View(updateDto);
        }

        // POST: Usuarios/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _usuarioService.UpdateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _usuarioService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: Usuarios/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleteDto = new UsuarioDeleteDTO { Id = id };

            var result = await _usuarioService.RemoveAsync(deleteDto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View("Delete");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
