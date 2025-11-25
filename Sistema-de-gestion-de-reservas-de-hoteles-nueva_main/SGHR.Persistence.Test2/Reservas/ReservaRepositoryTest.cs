using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Clientes;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.Persistence.Test2.Reservas
{
    public class ReservaRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly ReservaRepository _reservaRepository;
        private readonly ILogger<ReservaRepository> _logger;

        private Cliente clienteSeed;
        private Habitacion habitacionSeed;

        public ReservaRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ReservaRepository>();

            _reservaRepository = new ReservaRepository(_context, _logger);

            clienteSeed = new Cliente { Id = 1, Nombre = "ClientePrueba", Apellido = "Test", Email = "c@t.com", Telefono = "111", Identificacion = "1" };

            var categoriaSeed = new Categoria { Id = 1, Nombre = "Suite" };
            var pisoSeed = new Piso { Id = 1, Numero = 1 };
            _context.Categorias.Add(categoriaSeed);
            _context.Pisos.Add(pisoSeed);

            habitacionSeed = new Habitacion { Id = 1, Numero = 101, IdCategoria = categoriaSeed.Id, IdPiso = pisoSeed.Id, PrecioBase = 100 };

            _context.Clientes.Add(clienteSeed);
            _context.Habitaciones.Add(habitacionSeed);
            _context.SaveChanges();
        }

        [Fact]
        public async Task SaveEntityAsync_When_ValidReserva_ShouldSaveSuccessfully()
        {
            var reserva = new Reserva
            {
                IdCliente = clienteSeed.Id,
                IdHabitacion = habitacionSeed.Id,
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(3),
                NumeroHuespedes = 2,
                Total = 200,
                EstadoReserva = EstadoReserva.Activa
            };

            var result = await _reservaRepository.SaveEntityAsync(reserva);
            var reservaGuardada = await _context.Reservas.FindAsync(reserva.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Id > 0);
            Assert.NotNull(reservaGuardada);
            Assert.Equal(200, reservaGuardada!.Total);
        }


        [Fact]
        public async Task GetReservasPorFechaAsync_ShouldReturnCorrectOverlaps()
        {
            var hoy = DateTime.Today;
            _context.Reservas.AddRange(new List<Reserva>
            {
                new Reserva { IdCliente = 1, IdHabitacion = 1, FechaInicio = hoy.AddDays(-2), FechaFin = hoy.AddDays(1), Total = 100, IsDeleted = false },
                new Reserva { IdCliente = 1, IdHabitacion = 1, FechaInicio = hoy.AddDays(2), FechaFin = hoy.AddDays(4), Total = 100, IsDeleted = false },
                new Reserva { IdCliente = 1, IdHabitacion = 1, FechaInicio = hoy.AddDays(3), FechaFin = hoy.AddDays(4), Total = 100, IsDeleted = true }
            });
            await _context.SaveChangesAsync();

            var result = await _reservaRepository.GetReservasPorFechaAsync(hoy, hoy.AddDays(5));

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count); 
        }

        [Fact]
        public async Task GetReservasPorFechaAsync_When_NoneFound_ShouldReturnFail()
        {
            var hoy = DateTime.Today;

            var result = await _reservaRepository.GetReservasPorFechaAsync(hoy, hoy.AddDays(5));

            Assert.False(result.Success); 
            Assert.Equal("No se encontraron reservas en ese rango de fechas.", result.Message);
        }

        [Fact]
        public async Task GetReservasPorClienteAsync_ShouldReturnOnlyMatchingClient()
        {
            var cliente2 = new Cliente { Id = 2, Nombre = "Ana", Apellido = "Perez", Email = "c2@t.com", Telefono = "222", Identificacion = "2" };
            _context.Clientes.Add(cliente2);
            await _context.SaveChangesAsync();

            _context.Reservas.AddRange(new List<Reserva>
            {
                new Reserva { IdCliente = 1, IdHabitacion = 1, Total = 100, IsDeleted = false, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(1) },
                new Reserva { IdCliente = 2, IdHabitacion = 1, Total = 100, IsDeleted = false, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(1) }, // Cliente 2
                new Reserva { IdCliente = 1, IdHabitacion = 1, Total = 100, IsDeleted = false, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(1) },
                new Reserva { IdCliente = 1, IdHabitacion = 1, Total = 100, IsDeleted = true, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(1) } // Borrada
            });
            await _context.SaveChangesAsync();

            var result = await _reservaRepository.GetReservasPorClienteAsync(1);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count); 
        }

        [Fact]
        public async Task GetReservasPorClienteAsync_When_NoneFound_ShouldReturnFail()
        {

            var result = await _reservaRepository.GetReservasPorClienteAsync(clienteSeed.Id);

            Assert.False(result.Success); 
            Assert.Equal("El cliente no tiene reservas registradas.", result.Message);
        }

        [Fact]
        public async Task CancelarReservaAsync_ShouldSetIsDeletedTrue()
        {
            var reserva = new Reserva { IdCliente = 1, IdHabitacion = 1, Total = 100, IsDeleted = false, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(1) };
            await _reservaRepository.SaveEntityAsync(reserva);

            var cancelResult = await _reservaRepository.CancelarReservaAsync(reserva.Id);
            var retrievedReserva = await _reservaRepository.GetEntityByIdAsync(reserva.Id); 

            Assert.True(cancelResult.Success);
            Assert.True(cancelResult.Data);
            Assert.NotNull(retrievedReserva);
            Assert.True(retrievedReserva.IsDeleted);
        }
    }
}