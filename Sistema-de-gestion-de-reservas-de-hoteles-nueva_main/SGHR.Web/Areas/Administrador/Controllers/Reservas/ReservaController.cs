using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Interfaces.Clientes;
using System.Threading.Tasks;

namespace SGHR.Web.Areas.Administrador.Controllers.Reservas
{
    [Area("Administrador")]
    public class ReservaController : Controller
    {
        private readonly IReservaService _reservaService;
        private readonly IClienteService _clienteService;
        private readonly IHabitacionService _habitacionService;

        public ReservaController(
            IReservaService reservaService,
            IClienteService clienteService,
            IHabitacionService habitacionService)
        {
            _reservaService = reservaService;
            _clienteService = clienteService;
            _habitacionService = habitacionService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _reservaService.GetAllAsync();
            
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new List<ReservaDTO>());
            }

            var clientesResult = await _clienteService.GetAllAsync();
            var habitacionesResult = await _habitacionService.GetAllAsync();

            ViewBag.Clientes = clientesResult.Data ?? new List<ClienteDTO>();
            ViewBag.Habitaciones = habitacionesResult.Data ?? new List<HabitacionDTO>();

            return View(result.Data ?? new List<ReservaDTO>());
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new CreateReservaDTO { FechaInicio = DateTime.Today, FechaFin = DateTime.Today.AddDays(1) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(dto);
            }

            var result = await _reservaService.CreateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                await LoadDropdowns();
                return View(dto);
            }

            TempData["Success"] = "Reserva creada exitosamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _reservaService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            var dto = new UpdateReservaDTO
            {
                Id = result.Data.Id,
                FechaInicio = result.Data.FechaInicio,
                FechaFin = result.Data.FechaFin,
                IdCliente = result.Data.IdCliente,
                IdHabitacion = result.Data.IdHabitacion,
                Estado = result.Data.Estado,
                NumeroHuespedes = result.Data.NumeroHuespedes
            };

            await LoadDropdowns();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateReservaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(dto);
            }

            var result = await _reservaService.UpdateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                await LoadDropdowns();
                return View(dto);
            }

            TempData["Success"] = "Reserva actualizada exitosamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reservaService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectToAction("Index");

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _reservaService.RemoveAsync(new DeleteReservaDTO { Id = id });
            
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "Reserva eliminada exitosamente.";
            }
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BuscarPorFecha(DateTime inicio, DateTime fin)
        {
            var result = await _reservaService.GetReservasPorFechaAsync(inicio, fin);
            return View("Index", result.Data);
        }

        public async Task<IActionResult> BuscarPorCliente(int idCliente)
        {
            var result = await _reservaService.GetReservasPorClienteAsync(idCliente);
            return View("Index", result.Data);
        }

        public async Task<IActionResult> Cancelar(int id)
        {
            var result = await _reservaService.CancelarReservaAsync(id);
            
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "Reserva cancelada exitosamente.";
            }
            
            return RedirectToAction("Index");
        }

        private async Task LoadDropdowns()
        {
            var clientes = await _clienteService.GetAllAsync();
            var habitaciones = await _habitacionService.GetAllAsync();

            ViewBag.Clientes = clientes.Data ?? new List<ClienteDTO>();
            ViewBag.Habitaciones = habitaciones.Data ?? new List<HabitacionDTO>();
        }
    }
}
