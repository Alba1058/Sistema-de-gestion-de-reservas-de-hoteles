using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.Interfaces.Clientes;

namespace SGHR.Web.Areas.Administrador.Controllers.Clientes
{
    [Area("Administrador")]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var result = await _clienteService.GetAllAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new List<ClienteDTO>());
            }

            return View(result.Data ?? new List<ClienteDTO>());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _clienteService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _clienteService.CreateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            TempData["Success"] = "Cliente creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _clienteService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            var cliente = result.Data;

            var updateDto = new ClienteUpdateDTO
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Direccion = cliente.Direccion,
                Identificacion = cliente.Identificacion
            };

            return View(updateDto);
        }

        // POST: Clientes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _clienteService.UpdateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(dto);
            }

            TempData["Success"] = "Cliente actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _clienteService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: Clientes/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleteDto = new ClienteDeleteDTO { Id = id };

            var result = await _clienteService.RemoveAsync(deleteDto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Cliente eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
