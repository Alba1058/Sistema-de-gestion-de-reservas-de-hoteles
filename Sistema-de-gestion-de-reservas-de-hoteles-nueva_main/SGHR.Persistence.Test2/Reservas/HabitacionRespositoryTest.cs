using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.Persistence.Test2.Reservas
{
    public class HabitacionRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly HabitacionRepository _habitacionRepository;
        private readonly ILogger<HabitacionRepository> _logger;

        public HabitacionRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<HabitacionRepository>();

            _habitacionRepository = new HabitacionRepository(_context, _logger);

            var categoriaSeed = new Categoria { Id = 1, Nombre = "Suite" };
            var pisoSeed = new Piso { Id = 1, Numero = 1 };
            _context.Categorias.Add(categoriaSeed);
            _context.Pisos.Add(pisoSeed);
            _context.SaveChanges();
        }

        [Fact]
        public async Task SaveEntityAsync_When_ValidHabitacion_ShouldSaveSuccessfully()
        {
            var habitacion = new Habitacion
            {
                Numero = 101,
                IdCategoria = 1,
                IdPiso = 1,
                EstadoH = EstadoHabitacion.Disponible,
                PrecioBase = 150.00m
            };

            var result = await _habitacionRepository.SaveEntityAsync(habitacion);
            var habitacionGuardada = await _context.Habitaciones.FindAsync(habitacion.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Id > 0);
            Assert.NotNull(habitacionGuardada);
            Assert.Equal(101, habitacionGuardada!.Numero);
        }


        [Fact]
        public async Task DeleteEntityAsync_ShouldSetIsDeletedTrue() 
        {
            var habitacion = new Habitacion { Numero = 102, IdCategoria = 1, IdPiso = 1, PrecioBase = 100 };
            await _habitacionRepository.SaveEntityAsync(habitacion);

            var deleteResult = await _habitacionRepository.DeleteEntityAsync(habitacion);

            var retrievedHabitacion = await _habitacionRepository.GetEntityByIdAsync(habitacion.Id);

            Assert.True(deleteResult.Success);

            Assert.NotNull(retrievedHabitacion);
            Assert.True(retrievedHabitacion.IsDeleted);
        }

        [Fact]
        public async Task GetHabitacionesDisponiblesAsync_ShouldReturnOnlyAvailableAndActive()
        {
            _context.Habitaciones.AddRange(new List<Habitacion>
            {
                new Habitacion { Numero = 101, IdCategoria = 1, IdPiso = 1, PrecioBase = 100, EstadoH = EstadoHabitacion.Disponible, Estado = true, IsDeleted = false },
                new Habitacion { Numero = 102, IdCategoria = 1, IdPiso = 1, PrecioBase = 100, EstadoH = EstadoHabitacion.Ocupada, Estado = true, IsDeleted = false }, // Ocupada
                new Habitacion { Numero = 103, IdCategoria = 1, IdPiso = 1, PrecioBase = 100, EstadoH = EstadoHabitacion.Mantenimiento, Estado = true, IsDeleted = false }, // Mantenimiento
                new Habitacion { Numero = 104, IdCategoria = 1, IdPiso = 1, PrecioBase = 100, EstadoH = EstadoHabitacion.Disponible, Estado = false, IsDeleted = false }, // Inactiva
                new Habitacion { Numero = 105, IdCategoria = 1, IdPiso = 1, PrecioBase = 100, EstadoH = EstadoHabitacion.Disponible, Estado = true, IsDeleted = true } // Eliminada
            });
            await _context.SaveChangesAsync();

            var habitacionesDisponibles = await _habitacionRepository.GetHabitacionesDisponiblesAsync();
            Assert.NotNull(habitacionesDisponibles);
            Assert.Single(habitacionesDisponibles); 
            Assert.Equal(101, habitacionesDisponibles.First().Numero);
        }
    }
}