using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.Services.Configuration;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Configuration;

namespace SGHR.Application.Test2.Configuration
{
    public class PisoServiceTest
    {
        private readonly SGHRContext _context;
        private readonly PisoService _pisoService;
        private readonly PisoRepository _pisoRepository;
        private readonly ILogger<PisoService> _logger;

        private readonly Piso pisoExistente;

        public PisoServiceTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<PisoService>();
            var loggerRepo = loggerFactory.CreateLogger<PisoRepository>();

            _pisoRepository = new PisoRepository(_context, loggerRepo);

            _pisoService = new PisoService(_pisoRepository, _logger);

            pisoExistente = new Piso
            {
                Numero = 1,
                Descripcion = "Primer piso",
                Estado = true
            };
            _pisoRepository.SaveEntityAsync(pisoExistente).Wait();
        }


        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNumeroIsZero()
        {
            var dto = new CreatePisoDTO
            {
                Numero = 0, 
                Descripcion = "Piso Cero"
            };

            var result = await _pisoService.CreateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El número de piso debe ser mayor que cero.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNumeroIsDuplicate()
        {
            var dto = new CreatePisoDTO
            {
                Numero = 1, 
                Descripcion = "Piso Repetido"
            };

            var result = await _pisoService.CreateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Ya existe un piso con el número 1.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldSucceed_WhenValidData()
        {
            var dto = new CreatePisoDTO
            {
                Numero = 2,
                Descripcion = "Segundo piso"
            };

            var result = await _pisoService.CreateAsync(dto);
            var pisoEnDb = await _pisoRepository.GetEntityByIdAsync(result.Data!.Id);

            Assert.True(result.Success);
            Assert.Equal("Piso creado correctamente.", result.Message);
            Assert.NotNull(result.Data); 
            Assert.Equal(2, result.Data.Numero);
            Assert.NotNull(pisoEnDb);
        }


        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenNotFound()
        {
            var dto = new UpdatePisoDTO
            {
                Id = 999,
                Numero = 10,
                Descripcion = "No existe",
                Estado = true
            };

            var result = await _pisoService.UpdateAsync(dto);

            Assert.False(result.Success);

            Assert.Equal("El piso no existe.", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenNumeroIsDuplicate()
        {
            var piso2 = new Piso { Numero = 2, Descripcion = "Piso 2" };
            await _pisoRepository.SaveEntityAsync(piso2);

            var dto = new UpdatePisoDTO
            {
                Id = piso2.Id,
                Numero = 1, 
                Descripcion = "Piso Actualizado",
                Estado = true
            };

            var result = await _pisoService.UpdateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Ya existe otro piso con el número 1.", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSucceed_WhenValid()
        {
            var dto = new UpdatePisoDTO
            {
                Id = pisoExistente.Id,
                Numero = pisoExistente.Numero,
                Descripcion = "Piso actualizado", 
                Estado = true
            };

            var result = await _pisoService.UpdateAsync(dto);
            var pisoEnDb = await _pisoRepository.GetEntityByIdAsync(pisoExistente.Id);

            Assert.True(result.Success);
            Assert.Equal("Piso actualizado correctamente.", result.Message);
            Assert.NotNull(result.Data); 
            Assert.Equal("Piso actualizado", result.Data.Descripcion);
            Assert.Equal("Piso actualizado", pisoEnDb?.Descripcion);
        }

        [Fact]
        public async Task RemoveAsync_ShouldFail_WhenNotFound()
        {
            var dto = new DeletePisoDTO { Id = 999 };

            var result = await _pisoService.RemoveAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El piso no existe.", result.Message);
        }

        [Fact]
        public async Task RemoveAsync_ShouldSucceed_WhenExists()
        {
            var dto = new DeletePisoDTO { Id = pisoExistente.Id };

            var result = await _pisoService.RemoveAsync(dto);
            var pisoEnDb = await _pisoRepository.GetEntityByIdAsync(pisoExistente.Id);

            Assert.True(result.Success);
            Assert.NotNull(pisoEnDb);
            Assert.True(pisoEnDb.IsDeleted); 
        }


        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenNotFound()
        {

            var result = await _pisoService.GetByIdAsync(999);

            Assert.False(result.Success);
            Assert.Equal("Piso no encontrado.", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenPisoIsDeleted()
        {

            var pisoBorrado = new Piso { Numero = 99, Descripcion = "Piso Borrado" };
            await _pisoRepository.SaveEntityAsync(pisoBorrado);
            await _pisoRepository.DeleteEntityAsync(pisoBorrado); 

            var result = await _pisoService.GetByIdAsync(pisoBorrado.Id);

            Assert.False(result.Success);
            Assert.Equal("Piso no encontrado.", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_WhenExists()
        {

            var result = await _pisoService.GetByIdAsync(pisoExistente.Id);
            Assert.True(result.Success);
            Assert.NotNull(result.Data); 
            Assert.Equal(pisoExistente.Id, result.Data.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_OnlyNotDeleted()
        {

            var pisoBorrado = new Piso { Numero = 99, Descripcion = "Piso Borrado" };
            await _pisoRepository.SaveEntityAsync(pisoBorrado);
            await _pisoRepository.DeleteEntityAsync(pisoBorrado); 

            var result = await _pisoService.GetAllAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data); 
            Assert.Single(result.Data); 
            Assert.Equal(pisoExistente.Numero, result.Data[0].Numero);
        }
    }
}