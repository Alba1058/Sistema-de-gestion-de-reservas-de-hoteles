using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Configuration;

namespace SGHR.Persistence.Test2.Configuration
{
    public class RolUsuarioRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly RolUsuarioRepository _rolUsuarioRepository;
        private readonly ILogger<RolUsuarioRepository> _logger;

        public RolUsuarioRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<RolUsuarioRepository>();

            _rolUsuarioRepository = new RolUsuarioRepository(_context, _logger);
        }


        [Fact]
        public async Task SaveEntityAsync_When_ValidRol_ShouldSaveSuccessfully()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Nombre = "Administrador",
                Descripcion = "Rol con todos los permisos"
            };

            // Act
            var result = await _rolUsuarioRepository.SaveEntityAsync(rol);
            var rolGuardado = await _context.RolesUsuario.FindAsync(rol.Id);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Id > 0);
            Assert.NotNull(rolGuardado);
            Assert.Equal("Administrador", rolGuardado!.Nombre);
        }

        [Fact]
        public async Task SaveEntityAsync_When_NombreExists_ShouldSaveSuccessfully()
        {
            var rolExistente = new RolUsuario { Nombre = "RolRepetido" };
            await _rolUsuarioRepository.SaveEntityAsync(rolExistente);

            var rolNuevo = new RolUsuario { Nombre = "RolRepetido" };

            var result = await _rolUsuarioRepository.SaveEntityAsync(rolNuevo);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_ValidRol_ShouldUpdateSuccessfully()
        {
            // Arrange
            var rol = new RolUsuario { Nombre = "Rol Original" };
            await _rolUsuarioRepository.SaveEntityAsync(rol);

            // Act
            rol.Nombre = "Rol Actualizado";
            rol.Descripcion = "Descripción Nueva";
            var result = await _rolUsuarioRepository.UpdateEntityAsync(rol);
            var rolActualizado = await _context.RolesUsuario.FindAsync(rol.Id);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(rolActualizado);
            Assert.Equal("Rol Actualizado", rolActualizado.Nombre);
            Assert.Equal("Descripción Nueva", rolActualizado.Descripcion);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_RolNotFound_ShouldReturnFail()
        {
            var rol = new RolUsuario { Id = 99, Nombre = "Rol Fantasma" };

            var result = await _rolUsuarioRepository.UpdateEntityAsync(rol);

            Assert.False(result.Success);
            Assert.Contains("Error al actualizar la entidad", result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_NombreExists_ShouldUpdateSuccessfully()
        {
            var rol1 = new RolUsuario { Nombre = "Admin" };
            var rol2 = new RolUsuario { Nombre = "Invitado" };
            await _rolUsuarioRepository.SaveEntityAsync(rol1);
            await _rolUsuarioRepository.SaveEntityAsync(rol2);

            rol2.Nombre = "Admin";
            var result = await _rolUsuarioRepository.UpdateEntityAsync(rol2);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Admin", result.Data.Nombre);
        }


        [Fact]
        public async Task DeleteEntityAsync_ShouldSetIsDeletedTrue()
        {
            // Arrange
            var rol = new RolUsuario { Nombre = "Rol a Eliminar" };
            await _rolUsuarioRepository.SaveEntityAsync(rol);

            // Act
            var deleteResult = await _rolUsuarioRepository.DeleteEntityAsync(rol);
            var retrievedRol = await _rolUsuarioRepository.GetEntityByIdAsync(rol.Id);

            // Assert
            Assert.True(deleteResult.Success);

            Assert.NotNull(retrievedRol);
            Assert.True(retrievedRol.IsDeleted);
        }

        [Fact]
        public async Task GetRolesActivosAsync_ShouldReturnOnlyActiveAndNotDeleted()
        {
            _context.RolesUsuario.AddRange(new List<RolUsuario>
            {
                new RolUsuario { Nombre = "Admin", Estado = true, IsDeleted = false },
                new RolUsuario { Nombre = "Cajero", Estado = true, IsDeleted = false },
                new RolUsuario { Nombre = "Invitado", Estado = false, IsDeleted = false }, // Inactivo
                new RolUsuario { Nombre = "Ex-Empleado", Estado = true, IsDeleted = true } // Borrado
            });
            await _context.SaveChangesAsync();

            // Act
            var rolesActivos = await _rolUsuarioRepository.GetRolesActivosAsync();

            // Assert
            Assert.NotNull(rolesActivos);
            Assert.Equal(2, rolesActivos.Count); 
            Assert.Contains(rolesActivos, r => r.Nombre == "Admin");
            Assert.Contains(rolesActivos, r => r.Nombre == "Cajero");
        }
    }
}