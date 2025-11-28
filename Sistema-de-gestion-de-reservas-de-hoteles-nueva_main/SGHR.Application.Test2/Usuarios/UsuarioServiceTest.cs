using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Services.Usuarios;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Usuarios;

namespace SGHR.Application.Test2.Usuarios
{
    public class UsuarioServiceTest
    {
        private readonly SGHRContext _context;
        private readonly UsuarioService _usuarioService;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        private RolUsuario rolExistente;
        private Usuario usuarioExistente;

        public UsuarioServiceTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<UsuarioService>();
            var loggerRepo = loggerFactory.CreateLogger<UsuarioRepository>();

            _usuarioRepository = new UsuarioRepository(_context, loggerRepo);


            _usuarioService = new UsuarioService(_usuarioRepository, _logger);

            rolExistente = new RolUsuario { Id = 1, Nombre = "Administrador", Estado = true };
            _context.RolesUsuario.Add(rolExistente);
            _context.SaveChanges();

            usuarioExistente = new Usuario
            {
                Nombre = "Carlos",
                Email = "carlos@perez.com",
                Contrasena = "1234567",
                RolUsuarioId = rolExistente.Id,
                Estado = true
            };
            _usuarioRepository.SaveEntityAsync(usuarioExistente).Wait();
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNombreIsEmpty()
        {
            var dto = new UsuarioCreateDTO
            {
                Nombre = "", 
                Email = "test@test.com",
                Contrasena = "123456",
                RolUsuarioId = rolExistente.Id
            };

            var result = await _usuarioService.CreateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El campo 'Nombre' es obligatorio.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenPasswordIsShort()
        {
            var dto = new UsuarioCreateDTO
            {
                Nombre = "Test",
                Email = "test@test.com",
                Contrasena = "123", 
                RolUsuarioId = rolExistente.Id
            };

 
            var result = await _usuarioService.CreateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("La contraseña debe tener al menos 6 caracteres.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenEmailIsDuplicate()
        {
            var dto = new UsuarioCreateDTO
            {
                Nombre = "Ana",
                Email = "carlos@perez.com", 
                Contrasena = "12345678",
                RolUsuarioId = rolExistente.Id
            };

            var result = await _usuarioService.CreateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Ya existe un usuario con este correo electrónico.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldSucceed_WhenValidData()
        {
            var dto = new UsuarioCreateDTO
            {
                Nombre = "Ana",
                Email = "ana@ortega.com",
                Contrasena = "12345678",
                RolUsuarioId = rolExistente.Id
            };

            var result = await _usuarioService.CreateAsync(dto);

            Assert.True(result.Success);
            Assert.Equal("Usuario creado correctamente.", result.Message);
            Assert.NotNull(result.Data); 
            Assert.Equal("Ana", result.Data.Nombre);
            Assert.Equal("Administrador", result.Data.RolNombre);
        }


        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenNotFound()
        {
            var dto = new UsuarioUpdateDTO
            {
                Id = 999, 
                Nombre = "Desconocido",
                Email = "Desconocido@test.com",
                RolUsuarioId = 1,
                Activo = true
            };

            var result = await _usuarioService.UpdateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Usuario no encontrado.", result.Message); 
        }

        [Fact]
        public async Task UpdateAsync_ShouldSucceed_WhenValid()
        {
            var dto = new UsuarioUpdateDTO
            {
                Id = usuarioExistente.Id,
                Nombre = "Carlos Modificado",
                Email = "carlosPerez@fernandez.com",
                RolUsuarioId = rolExistente.Id,
                Activo = true
            };

            var result = await _usuarioService.UpdateAsync(dto);

            Assert.True(result.Success);
            Assert.Equal("Usuario actualizado correctamente.", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal("Carlos Modificado", result.Data.Nombre);
        }


        [Fact]
        public async Task RemoveAsync_ShouldFail_WhenNotFound()
        {
            var dto = new UsuarioDeleteDTO { Id = 999 };

            var result = await _usuarioService.RemoveAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Usuario no encontrado.", result.Message); 
        }

        [Fact]
        public async Task RemoveAsync_ShouldSucceed_WhenExists()
        {
            var dto = new UsuarioDeleteDTO { Id = usuarioExistente.Id };

            var result = await _usuarioService.RemoveAsync(dto);
            var userInDb = await _usuarioRepository.GetEntityByIdAsync(usuarioExistente.Id);

            Assert.True(result.Success);
            Assert.NotNull(userInDb);
            Assert.True(userInDb.IsDeleted); 
        }


        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenNotFound()
        {
            var result = await _usuarioService.GetByIdAsync(999);
            Assert.False(result.Success);
            Assert.Equal("Usuario no encontrado.", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenUserIsDeleted()
        {
            var deletedUser = new Usuario { Nombre = "Del", Email = "del@test.com", Contrasena = "123", RolUsuarioId = rolExistente.Id };
            await _usuarioRepository.SaveEntityAsync(deletedUser);
            await _usuarioRepository.DeleteEntityAsync(deletedUser); 

            var result = await _usuarioService.GetByIdAsync(deletedUser.Id);

            Assert.False(result.Success);
            Assert.Equal("Usuario no encontrado.", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_User_WhenExists()
        {
            var result = await _usuarioService.GetByIdAsync(usuarioExistente.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data); 
            Assert.Equal(usuarioExistente.Id, result.Data.Id);
            Assert.Equal("Administrador", result.Data.RolNombre); 
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldFail_WhenNotFound()
        {
            var result = await _usuarioService.GetByEmailAsync("noexiste@test.com");

            Assert.False(result.Success);
            Assert.Equal("Usuario no encontrado.", result.Message); 
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturn_User_WhenExists()
        {
            var result = await _usuarioService.GetByEmailAsync("carlos@perez.com");

            Assert.True(result.Success);
            Assert.NotNull(result.Data); 
            Assert.Equal("carlos@perez.com", result.Data.Email);
            Assert.Equal("Administrador", result.Data.RolNombre); 
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_OnlyActiveUsuarios()
        {

            var deletedUser = new Usuario { Nombre = "Del", Email = "del@test.com", Contrasena = "123", RolUsuarioId = rolExistente.Id };
            await _usuarioRepository.SaveEntityAsync(deletedUser);
            await _usuarioRepository.DeleteEntityAsync(deletedUser);

            var result = await _usuarioService.GetAllAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data); 
            Assert.Single(result.Data); 
            Assert.Equal("Carlos", result.Data[0].Nombre);
        }
    }
}