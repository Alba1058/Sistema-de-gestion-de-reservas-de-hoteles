using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Interfaces.Clientes;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Domain.Enums;

namespace SGHR.Web.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class DashboardController : Controller
    {
        private readonly IReservaService _reservaService;
        private readonly IHabitacionService _habitacionService;
        private readonly IClienteService _clienteService;

        public DashboardController(
            IReservaService reservaService,
            IHabitacionService habitacionService,
            IClienteService clienteService)
        {
            _reservaService = reservaService;
            _habitacionService = habitacionService;
            _clienteService = clienteService;
        }

        public async Task<IActionResult> Index()
        {
            var reservasResult = await _reservaService.GetAllAsync();
            var habitacionesResult = await _habitacionService.GetAllAsync();
            var clientesResult = await _clienteService.GetAllAsync();

            var reservas = reservasResult.Data ?? new List<ReservaDTO>();
            var habitaciones = habitacionesResult.Data ?? new List<HabitacionDTO>();
            var clientes = clientesResult.Data ?? new List<ClienteDTO>();

            var reservasHoy = reservas.Count(r => r.FechaInicio.Date == DateTime.Today);
            var habitacionesDisponibles = habitaciones.Count(h => 
                h.EstadoHabitacion == (int)EstadoHabitacion.Disponible && h.Estado);
            var ingresosMensuales = reservas
                .Where(r => r.FechaInicio.Month == DateTime.Now.Month && r.FechaInicio.Year == DateTime.Now.Year)
                .Sum(r => r.Total);
            var totalHabitaciones = habitaciones.Count(h => h.Estado);
            var habitacionesOcupadas = habitaciones.Count(h => 
                h.EstadoHabitacion == (int)EstadoHabitacion.Ocupada);
            var ocupacion = totalHabitaciones > 0 
                ? (habitacionesOcupadas * 100.0 / totalHabitaciones) 
                : 0;

            var reservasRecientes = reservas
                .OrderByDescending(r => r.FechaInicio)
                .Take(5)
                .ToList();

            ViewBag.ReservasHoy = reservasHoy;
            ViewBag.HabitacionesDisponibles = habitacionesDisponibles;
            ViewBag.IngresosMensuales = ingresosMensuales;
            ViewBag.Ocupacion = Math.Round(ocupacion, 1);
            ViewBag.ReservasRecientes = reservasRecientes;
            ViewBag.Clientes = clientes;
            ViewBag.Habitaciones = habitaciones;
            ViewBag.TotalReservas = reservas.Count;
            ViewBag.TotalHabitaciones = totalHabitaciones;
            ViewBag.TotalClientes = clientes.Count;

            return View();
        }
    }
}