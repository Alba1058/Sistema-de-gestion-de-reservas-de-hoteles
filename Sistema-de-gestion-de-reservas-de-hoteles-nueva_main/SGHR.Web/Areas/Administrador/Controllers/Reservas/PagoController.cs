using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Application.Interfaces.Reservas;
using System.Threading.Tasks;

namespace SGHR.Web.Areas.Administrador.Controllers.Reservas
{
    [Area("Administrador")]
    public class PagoController : Controller
    {
        private readonly IPagoService _pagoService;

        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _pagoService.GetAllAsync();
            return View(result.Data);
        }

        public IActionResult Create()
        {
            return View(new CreatePagoDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePagoDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _pagoService.CreateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _pagoService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectToAction("Index");

            var dto = new UpdatePagoDTO
            {
                Id = result.Data.Id,
                Monto = result.Data.Monto,
                Metodo = result.Data.Metodo,
                FechaPago = result.Data.FechaPago,
                IdReserva = result.Data.IdReserva,
                Estado = result.Data.Estado
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePagoDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _pagoService.UpdateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pagoService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectToAction("Index");

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _pagoService.RemoveAsync(new DeletePagoDTO { Id = id });
            return RedirectToAction("Index");
        }
    }
}
