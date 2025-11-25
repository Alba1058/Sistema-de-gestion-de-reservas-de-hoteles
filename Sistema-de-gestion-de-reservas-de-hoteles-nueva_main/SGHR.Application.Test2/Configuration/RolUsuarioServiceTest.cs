using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.Services.Configuration; 
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuration; 
using SGHR.Persistence.Repositories.Configuration;

namespace SGHR.Application.Test2.Configuration
{
    public class RolUsuarioServiceTest
    {
        private readonly SGHRContext _context;
        private readonly IRolUsuarioRepository _repository; 
        private readonly RolUsuarioService _service;
        private readonly ILogger<RolUsuarioService> _logger;

        private readonly RolUsuario rolExistente;

        public RolUsuarioServiceTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: $"RolUsuarioDB_{Guid.NewGuid()}")
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var repoLogger = loggerFactory.CreateLogger<RolUsuarioRepository>();
            _logger = loggerFactory.CreateLogger<RolUsuarioService>();

            _repository = new RolUsuarioRepository(_context, repoLogger);

            _service = new RolUsuarioService(_repository, _logger);
            rolExistente = new RolUsuario { Nombre = "Rol_Existente", Estado = true };
            _repository.SaveEntityAsync(rolExistente).Wait();
        }


        [Fact]
        public async Task CreateAsync_ShouldCreateRoleSuccessfully()
        {
            var dto = new CreateRolUsuarioDTO
            {
                Nombre = "Administrador",
                Descripcion = "Rol con todos los permisos",
                Estado = true
            };

            var resultado = await _service.CreateAsync(dto);
            var rolEnDb = await _repository.GetEntityByIdAsync(resultado.Data!.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data); 
            Assert.Equal("Administrador", resultado.Data.Nombre);
            Assert.NotNull(rolEnDb);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNameIsDuplicate()
        {
            var dto = new CreateRolUsuarioDTO
            {
                Nombre = "Rol_Existente", 
                Descripcion = "Duplicado",
                Estado = true
            };

            var resultado = await _service.CreateAsync(dto);

            Assert.False(resultado.Success);
            Assert.Equal("Ya existe un rol con ese nombre.", resultado.Message);
        }


        [Fact]
        public async Task UpdateAsync_ShouldUpdateRoleSuccessfully()
        {
            var dtoActualizar = new UpdateRolUsuarioDTO
            {
                Id = rolExistente.Id,
                Nombre = "Usuario Actualizado",
                Descripcion = "Descripción actualizada",
                Estado = true
            };

            var resultado = await _service.UpdateAsync(dtoActualizar);
            var rolEnDb = await _repository.GetEntityByIdAsync(rolExistente.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data); 
            Assert.Equal("Usuario Actualizado", resultado.Data.Nombre);
            Assert.Equal("Usuario Actualizado", rolEnDb?.Nombre);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenNotFound()
        {
            var dto = new UpdateRolUsuarioDTO { Id = 999, Nombre = "Fantasma" };

            var result = await _service.UpdateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El rol no existe.", result.Message);
        }

        [Fact]
        public async Task RemoveAsync_ShouldDeleteRoleSuccessfully()
        {
            var dtoEliminar = new DeleteRolUsuarioDTO { Id = rolExistente.Id };

            var resultado = await _service.RemoveAsync(dtoEliminar);
            var rolEnDb = await _repository.GetEntityByIdAsync(rolExistente.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(rolEnDb);
            Assert.True(rolEnDb.IsDeleted); 
        }

        [Fact]
        public async Task RemoveAsync_ShouldFail_WhenNotFound()
        {
            var dto = new DeleteRolUsuarioDTO { Id = 999 };

            var result = await _service.RemoveAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El rol no existe.", result.Message);
        }


        [Fact]
        public async Task GetRolesActivosAsync_ShouldReturnOnlyActiveRoles()
        {
            await _service.CreateAsync(new CreateRolUsuarioDTO { Nombre = "Activo1", Estado = true });
            await _service.CreateAsync(new CreateRolUsuarioDTO { Nombre = "Inactivo1", Estado = false });

            var resultado = await _service.GetRolesActivosAsync();

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data); 
            Assert.Equal(2, resultado.Data.Count);
            Assert.All(resultado.Data, r => Assert.True(r.Estado));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenNotFound()
        {
            var result = await _service.GetByIdAsync(999);
            Assert.False(result.Success);
            Assert.Equal("El rol no existe.", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenRolIsDeleted()
        {

            var rolBorrado = new RolUsuario { Nombre = "Rol Borrado" };
            await _repository.SaveEntityAsync(rolBorrado);
            await _repository.DeleteEntityAsync(rolBorrado); 

            var result = await _service.GetByIdAsync(rolBorrado.Id);

            Assert.False(result.Success);
            Assert.Equal("El rol no existe.", result.Message);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_OnlyNotDeleted()
        {
            var rolBorrado = new RolUsuario { Nombre = "Rol Borrado" };
            await _repository.SaveEntityAsync(rolBorrado);
            await _repository.DeleteEntityAsync(rolBorrado); 

            var result = await _service.GetAllAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data); 
            Assert.Single(result.Data); 
            Assert.Equal(rolExistente.Nombre, result.Data[0].Nombre);
        }
    }
}