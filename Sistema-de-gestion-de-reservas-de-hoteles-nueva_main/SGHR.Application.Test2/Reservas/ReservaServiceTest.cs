using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Services.Reservas;
using SGHR.Domain.Entities.Clientes;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Reservas;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Application.Test2.Reservas
{
    public class ReservaServiceTest
    {
        private readonly SGHRContext _contexto;
        private readonly IReservaRepository _repositorio;
        private readonly IReservaServicioRepository _repoReservaServicio;
        private readonly ReservaService _servicio;
        private readonly ILoggerFactory _loggerFactory;

        private readonly Cliente clienteSeed;
        private readonly Habitacion habitacionSeed;

        public ReservaServiceTest()
        {
            var opciones = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new SGHRContext(opciones);
            _contexto.Database.EnsureCreated();
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var loggerReservaRepo = _loggerFactory.CreateLogger<ReservaRepository>();
            var loggerService = _loggerFactory.CreateLogger<ReservaService>();
            var loggerReservaServicio = _loggerFactory.CreateLogger<ReservaServicioRepository>();

            _repositorio = new ReservaRepository(_contexto, loggerReservaRepo);
            _repoReservaServicio = new ReservaServicioRepository(_contexto, loggerReservaServicio);

            _servicio = new ReservaService(_repositorio, _repoReservaServicio, loggerService);

            clienteSeed = new Cliente
            {
                Id = 1,
                Identificacion = "40200000001",
                Nombre = "Juan",
                Apellido = "Pérez",
                Telefono = "8090000000",
                Email = "juan@perez.com",
            };
            _contexto.Clientes.Add(clienteSeed);

            var categoriaSeed = new Categoria { Id = 1, Nombre = "Suite" };
            var pisoSeed = new Piso { Id = 1, Numero = 1 };
            _contexto.Categorias.Add(categoriaSeed);
            _contexto.Pisos.Add(pisoSeed);
            _contexto.SaveChanges();

            habitacionSeed = new Habitacion
            {
                Id = 1,
                Numero = 101,
                PrecioBase = 1500,
                IdCategoria = categoriaSeed.Id,
                IdPiso = pisoSeed.Id,
                Estado = true,
                EstadoH = EstadoHabitacion.Disponible
            };
            _contexto.Habitaciones.Add(habitacionSeed);
            _contexto.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateReservationSuccessfully()
        {
            var dto = new CreateReservaDTO
            {
                IdCliente = clienteSeed.Id,
                IdHabitacion = habitacionSeed.Id,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(3),
                NumeroHuespedes = 2,
                Total = 3000,
                EstadoReserva = (int)EstadoReserva.Activa
            };

            var resultado = await _servicio.CreateAsync(dto);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal(clienteSeed.Id, resultado.Data.IdCliente);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenHabitacionIsOccupied()
        {
            var dtoExistente = new CreateReservaDTO
            {
                IdCliente = clienteSeed.Id,
                IdHabitacion = habitacionSeed.Id,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(3),
                NumeroHuespedes = 2,
                Total = 3000,
                EstadoReserva = (int)EstadoReserva.Activa
            };
            await _servicio.CreateAsync(dtoExistente);

            var dtoDuplicada = new CreateReservaDTO
            {
                IdCliente = clienteSeed.Id,
                IdHabitacion = habitacionSeed.Id,
                FechaInicio = DateTime.UtcNow.AddDays(2),
                FechaFin = DateTime.UtcNow.AddDays(4),
                NumeroHuespedes = 1,
                Total = 1500,
                EstadoReserva = (int)EstadoReserva.Activa
            };
            var resultado = await _servicio.CreateAsync(dtoDuplicada);

            Assert.False(resultado.Success);
            Assert.Equal("La habitación ya está reservada en esas fechas.", resultado.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenDatesAreInvalid()
        {
            var dto = new CreateReservaDTO
            {
                IdCliente = clienteSeed.Id,
                IdHabitacion = habitacionSeed.Id,
                FechaInicio = DateTime.UtcNow.AddDays(3),
                FechaFin = DateTime.UtcNow.AddDays(1),
                NumeroHuespedes = 2,
                Total = 3000,
                EstadoReserva = (int)EstadoReserva.Activa
            };

            var resultado = await _servicio.CreateAsync(dto);

            Assert.False(resultado.Success);
            Assert.Equal("La fecha de inicio debe ser anterior a la fecha de fin.", resultado.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateReservationSuccessfully()
        {
            var crear = await _servicio.CreateAsync(new CreateReservaDTO
            {
                IdCliente = 1,
                IdHabitacion = 1,
                FechaInicio = DateTime.UtcNow.AddDays(5),
                FechaFin = DateTime.UtcNow.AddDays(7),
                NumeroHuespedes = 2,
                Total = 2800,
                EstadoReserva = (int)EstadoReserva.Activa
            });

            Assert.True(crear.Success);
            Assert.NotNull(crear.Data);

            var dtoActualizar = new UpdateReservaDTO
            {
                Id = crear.Data.Id,
                IdCliente = 1,
                IdHabitacion = 1,
                FechaInicio = DateTime.UtcNow.AddDays(6),
                FechaFin = DateTime.UtcNow.AddDays(8),
                NumeroHuespedes = 3,
                Total = 3500,
                EstadoReserva = (int)EstadoReserva.Activa
            };

            var resultado = await _servicio.UpdateAsync(dtoActualizar);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal(3, resultado.Data.NumeroHuespedes);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetReservationByIdSuccessfully()
        {
            var crear = await _servicio.CreateAsync(new CreateReservaDTO
            {
                IdCliente = 1,
                IdHabitacion = 1,
                FechaInicio = DateTime.UtcNow.AddDays(2),
                FechaFin = DateTime.UtcNow.AddDays(4),
                NumeroHuespedes = 2,
                Total = 2500,
                EstadoReserva = (int)EstadoReserva.Activa
            });

            Assert.True(crear.Success);
            Assert.NotNull(crear.Data);

            var resultado = await _servicio.GetByIdAsync(crear.Data.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal(crear.Data.Id, resultado.Data.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenIsDeleted()
        {
            var crear = await _servicio.CreateAsync(new CreateReservaDTO
            {
                IdCliente = 1,
                IdHabitacion = 1,
                FechaInicio = DateTime.UtcNow.AddDays(2),
                FechaFin = DateTime.UtcNow.AddDays(4),
                NumeroHuespedes = 2,
                Total = 2500,
                EstadoReserva = (int)EstadoReserva.Activa
            });

            Assert.True(crear.Success);
            Assert.NotNull(crear.Data);

            await _servicio.RemoveAsync(new DeleteReservaDTO { Id = crear.Data.Id });
            var resultado = await _servicio.GetByIdAsync(crear.Data.Id);

            Assert.False(resultado.Success);
            Assert.Equal("Reserva no encontrada.", resultado.Message);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveReservationSuccessfully()
        {
            var crear = await _servicio.CreateAsync(new CreateReservaDTO
            {
                IdCliente = 1,
                IdHabitacion = 1,
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow.AddDays(2),
                NumeroHuespedes = 1,
                Total = 1200,
                EstadoReserva = (int)EstadoReserva.Activa
            });

            Assert.True(crear.Success);
            Assert.NotNull(crear.Data);

            var dtoEliminar = new DeleteReservaDTO { Id = crear.Data.Id };
            var resultado = await _servicio.RemoveAsync(dtoEliminar);
            var reservaEnDb = await _repositorio.GetEntityByIdAsync(crear.Data.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(reservaEnDb);
            Assert.True(reservaEnDb.IsDeleted);
        }

        [Fact]
        public async Task CancelarReservaAsync_ShouldCancelReservationSuccessfully()
        {
            var crear = await _servicio.CreateAsync(new CreateReservaDTO
            {
                IdCliente = 1,
                IdHabitacion = 1,
                FechaInicio = DateTime.UtcNow.AddDays(10),
                FechaFin = DateTime.UtcNow.AddDays(12),
                NumeroHuespedes = 2,
                Total = 2000,
                EstadoReserva = (int)EstadoReserva.Activa
            });

            Assert.True(crear.Success);
            Assert.NotNull(crear.Data);

            var resultado = await _servicio.CancelarReservaAsync(crear.Data.Id);
            var reservaEnDb = await _repositorio.GetEntityByIdAsync(crear.Data.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(reservaEnDb);
            Assert.True(reservaEnDb.IsDeleted);
        }
    }
}