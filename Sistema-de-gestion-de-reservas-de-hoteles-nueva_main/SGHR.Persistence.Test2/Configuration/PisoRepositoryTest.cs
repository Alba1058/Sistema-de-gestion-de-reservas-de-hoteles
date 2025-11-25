using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Configuration;


namespace SGHR.Persistence.Test2.Configuration
{
    public class PisoRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly PisoRepository _pisoRepository;
        private readonly ILogger<PisoRepository> _logger;

        public PisoRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<PisoRepository>();

            _pisoRepository = new PisoRepository(_context, _logger);
        }

        [Fact]
        public async Task SaveEntityAsync_When_ValidPiso_ShouldSaveSuccessfully()
        {
            // Arrange
            var piso = new Piso
            {
                Numero = 1,
                Descripcion = "Primer Nivel"
            };

            // Act
            var result = await _pisoRepository.SaveEntityAsync(piso);
            var pisoGuardado = await _context.Pisos.FindAsync(piso.Id);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Id > 0);
            Assert.NotNull(pisoGuardado);
            Assert.Equal(1, pisoGuardado.Numero);
        }


        [Fact]
        public async Task DeleteEntityAsync_ShouldSetIsDeletedTrue() 
        {
            // Arrange
            var piso = new Piso { Numero = 10, Descripcion = "Piso a Eliminar" };
            await _pisoRepository.SaveEntityAsync(piso);

            // Act
            var deleteResult = await _pisoRepository.DeleteEntityAsync(piso);

            var retrievedPiso = await _pisoRepository.GetEntityByIdAsync(piso.Id);

            // Assert
            Assert.True(deleteResult.Success);

            Assert.NotNull(retrievedPiso);
            Assert.True(retrievedPiso.IsDeleted);
        }

        [Fact]
        public async Task RestoreEntityAsync_When_Deleted_ShouldSetIsDeletedFalse()
        {
            // Arrange
            var piso = new Piso { Numero = 12, Descripcion = "Piso Restaurado" };
            await _pisoRepository.SaveEntityAsync(piso);
            await _pisoRepository.DeleteEntityAsync(piso); 

            // Act
            var restoreResult = await _pisoRepository.RestoreEntityAsync(piso); 
            var retrievedPiso = await _pisoRepository.GetEntityByIdAsync(piso.Id);

            // Assert
            Assert.True(restoreResult.Success);
            Assert.False(piso.IsDeleted); 
            Assert.NotNull(retrievedPiso);
            Assert.False(retrievedPiso.IsDeleted);
            Assert.Equal(12, retrievedPiso.Numero);
        }
    }
}