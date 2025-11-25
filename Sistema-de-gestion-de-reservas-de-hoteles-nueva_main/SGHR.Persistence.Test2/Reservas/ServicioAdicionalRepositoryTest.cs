using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.Persistence.Test2.Reservas
{
    public class ServicioAdicionalRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly ServicioAdicionalRepository _servicioRepository;
        private readonly ILogger<ServicioAdicionalRepository> _logger;

        public ServicioAdicionalRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ServicioAdicionalRepository>();

            _servicioRepository = new ServicioAdicionalRepository(_context, _logger);
        }

        [Fact]
        public async Task SaveEntityAsync_When_ValidServicio_ShouldSaveSuccessfully()
        {
            var servicio = new ServicioAdicional
            {
                Nombre = "WiFi Premium",
                Precio = 10.00m,
                Descripcion = "Acceso de alta velocidad"
            };

            var result = await _servicioRepository.SaveEntityAsync(servicio);
            var servicioGuardado = await _context.ServiciosAdicionales.FindAsync(servicio.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Id > 0);
            Assert.NotNull(servicioGuardado);
            Assert.Equal(10.00m, servicioGuardado!.Precio);
        }

        [Fact]
        public async Task DeleteEntityAsync_ShouldSetIsDeletedTrue() 
        {
            var servicio = new ServicioAdicional { Nombre = "Servicio a Eliminar", Precio = 5 };
            await _servicioRepository.SaveEntityAsync(servicio);

            var deleteResult = await _servicioRepository.DeleteEntityAsync(servicio);

            var retrievedServicio = await _servicioRepository.GetEntityByIdAsync(servicio.Id);

            Assert.True(deleteResult.Success);

            Assert.NotNull(retrievedServicio);
            Assert.True(retrievedServicio.IsDeleted);
        }

        [Fact]
        public async Task GetServiciosDisponiblesAsync_ShouldReturnOnlyActiveAndNotDeleted()
        {
            _context.ServiciosAdicionales.AddRange(new List<ServicioAdicional>
            {
                new ServicioAdicional { Nombre = "Servicio Activo 1", Precio = 1, Estado = true, IsDeleted = false },
                new ServicioAdicional { Nombre = "Servicio Activo 2", Precio = 2, Estado = true, IsDeleted = false },
                new ServicioAdicional { Nombre = "Servicio Inactivo", Precio = 3, Estado = false, IsDeleted = false }, // Inactivo
                new ServicioAdicional { Nombre = "Servicio Eliminado", Precio = 4, Estado = true, IsDeleted = true } // Eliminado
            });
            await _context.SaveChangesAsync();

            var serviciosActivos = await _servicioRepository.GetServiciosDisponiblesAsync();

            Assert.NotNull(serviciosActivos);
            Assert.Equal(2, serviciosActivos.Count); // Solo Activo 1 y 2
            Assert.Contains(serviciosActivos, s => s.Nombre == "Servicio Activo 1");
            Assert.Contains(serviciosActivos, s => s.Nombre == "Servicio Activo 2");
        }
    }
}