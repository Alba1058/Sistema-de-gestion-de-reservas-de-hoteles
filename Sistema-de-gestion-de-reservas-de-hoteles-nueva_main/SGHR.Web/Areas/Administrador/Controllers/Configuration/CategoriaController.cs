using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Application.Interfaces.Configuration;

namespace SGHR.Web.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var result = await _categoriaService.GetAllAsync();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoriaService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoriaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _categoriaService.CreateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoriaService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            var c = result.Data;

            var updateDto = new UpdateCategoriaDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Estado = c.Estado
            };

            return View(updateDto);
        }

        // POST: Categorias/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoriaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _categoriaService.UpdateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoriaService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: Categorias/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleteDto = new DeleteCategoriaDTO { Id = id };

            var result = await _categoriaService.RemoveAsync(deleteDto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View("Delete");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
