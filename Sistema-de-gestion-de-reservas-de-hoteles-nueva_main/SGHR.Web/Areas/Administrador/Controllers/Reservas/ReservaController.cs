using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Interfaces.Reservas;
using System.Threading.Tasks;

namespace SGHR.Web.Areas.Administrador.Controllers.Reservas
{
    [Area("Administrador")]
    public class ReservaController : Controller
    {
        private readonly IReservaService _reservaService;

        public ReservaController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _reservaService.GetAllAsync();
            return View(result.Data);
        }

        public IActionResult Create()
        {
            return View(new CreateReservaDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReservaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _reservaService.CreateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _reservaService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectToAction("Index");

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

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateReservaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _reservaService.UpdateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _reservaService.RemoveAsync(new DeleteReservaDTO { Id = id });
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
            await _reservaService.CancelarReservaAsync(id);
            return RedirectToAction("Index");
        }
    }
}
