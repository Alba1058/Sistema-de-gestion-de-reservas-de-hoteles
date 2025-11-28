using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Usuarios;

namespace SGHR.Persistence.Test2.Usuarios
{
    public class UsuarioRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<UsuarioRepository>();

            _usuarioRepository = new UsuarioRepository(_context, _logger);
        }

        private async Task<RolUsuario> SeedRolAsync()
        {
            var rol = new RolUsuario { Id = 1, Nombre = "Admin" };
            _context.RolesUsuario.Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        [Fact]
        public async Task SaveEntityAsync_When_ValidUsuario_ShouldSaveAndReturnWithRole()
        {
            var rol = await SeedRolAsync();
            var usuario = new Usuario
            {
                Nombre = "Admin User",
                Email = "admin@test.com",
                Contrasena = "hashedpassword",
                RolUsuarioId = rol.Id
            };

            var result = await _usuarioRepository.SaveEntityAsync(usuario);
            var usuarioGuardado = await _context.Usuarios.FindAsync(usuario.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Admin User", result.Data.Nombre);
            Assert.NotNull(usuarioGuardado); 

            Assert.NotNull(result.Data.RolUsuario);
            Assert.Equal("Admin", result.Data.RolUsuario.Nombre);
        }

        [Fact]
        public async Task SaveEntityAsync_When_EmailExists_ShouldSaveSuccessfully()
        {
            var rol = await SeedRolAsync();
            var usuario1 = new Usuario { Nombre = "User 1", Email = "email@repetido.com", Contrasena = "123", RolUsuarioId = rol.Id };
            await _usuarioRepository.SaveEntityAsync(usuario1);

            var usuario2 = new Usuario { Nombre = "User 2", Email = "email@repetido.com", Contrasena = "456", RolUsuarioId = rol.Id };

            var result = await _usuarioRepository.SaveEntityAsync(usuario2);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_ValidUsuario_ShouldUpdateSuccessfully()
        {
            var rol = await SeedRolAsync();
            var usuario = new Usuario { Nombre = "Usuario Original", Email = "original@test.com", Contrasena = "123", RolUsuarioId = rol.Id };
            await _usuarioRepository.SaveEntityAsync(usuario);

            usuario.Nombre = "Usuario Actualizado";
            var result = await _usuarioRepository.UpdateEntityAsync(usuario);
            var usuarioActualizado = await _context.Usuarios.FindAsync(usuario.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Usuario Actualizado", result.Data.Nombre);
            Assert.Equal("Usuario Actualizado", usuarioActualizado?.Nombre);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_UsuarioNotFound_ShouldReturnFail()
        {
            var usuario = new Usuario { Id = 99, Nombre = "Fantasma", Email = "fantasma@test.com", Contrasena = "123", RolUsuarioId = 1 };

            var result = await _usuarioRepository.UpdateEntityAsync(usuario);

            Assert.False(result.Success);
            Assert.Contains("Error al actualizar la entidad", result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_EmailExists_ShouldUpdateSuccessfully()
        {
            var rol = await SeedRolAsync();
            var usuario1 = new Usuario { Nombre = "User 1", Email = "user1@test.com", Contrasena = "123", RolUsuarioId = rol.Id };
            var usuario2 = new Usuario { Nombre = "User 2", Email = "user2@test.com", Contrasena = "456", RolUsuarioId = rol.Id };
            await _usuarioRepository.SaveEntityAsync(usuario1);
            await _usuarioRepository.SaveEntityAsync(usuario2);

            usuario2.Email = "user1@test.com";
            var result = await _usuarioRepository.UpdateEntityAsync(usuario2);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("user1@test.com", result.Data.Email);
        }


        [Fact]
        public async Task GetUsuarioByCorreoAsync_When_Exists_ShouldReturnUsuarioWithRole()
        {
            var rol = await SeedRolAsync();
            var emailBuscado = "test@correo.com";
            var usuario = new Usuario { Nombre = "Usuario Test", Email = emailBuscado, Contrasena = "123", RolUsuarioId = rol.Id };
            await _usuarioRepository.SaveEntityAsync(usuario);

            var result = await _usuarioRepository.GetUsuarioByCorreoAsync(emailBuscado);

            Assert.NotNull(result);
            Assert.Equal("Usuario Test", result.Nombre);

            Assert.NotNull(result.RolUsuario);
            Assert.Equal("Admin", result.RolUsuario.Nombre);
        }

        [Fact]
        public async Task GetUsuarioByCorreoAsync_When_Deleted_ShouldReturnNull()
        {

            var rol = await SeedRolAsync();
            var emailBuscado = "deleted@correo.com";
            var usuario = new Usuario { Nombre = "Usuario Borrado", Email = emailBuscado, Contrasena = "123", RolUsuarioId = rol.Id, IsDeleted = true };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();


            var result = await _usuarioRepository.GetUsuarioByCorreoAsync(emailBuscado);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnActiveUsersWithRoles()
        {
            var rol = await SeedRolAsync();
            var usuario1 = new Usuario { Nombre = "User 1", Email = "user1@test.com", Contrasena = "123", RolUsuarioId = rol.Id };
            var usuario2 = new Usuario { Nombre = "User 2", Email = "user2@test.com", Contrasena = "456", RolUsuarioId = rol.Id };
            var usuarioBorrado = new Usuario { Nombre = "User 3", Email = "user3@test.com", Contrasena = "789", RolUsuarioId = rol.Id, IsDeleted = true };

            _context.Usuarios.AddRange(usuario1, usuario2, usuarioBorrado);
            await _context.SaveChangesAsync();

            var result = await _usuarioRepository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count); 
            Assert.DoesNotContain(result, u => u.Nombre == "User 3"); 
            Assert.All(result, u => Assert.NotNull(u.RolUsuario)); 
        }

        [Fact]
        public async Task GetEntityByIdAsync_Should_ReturnUserWithRole_EvenIfDeleted()
        {

            var rol = await SeedRolAsync();
            var usuario = new Usuario { Nombre = "Usuario Borrado", Email = "del@test.com", Contrasena = "123", RolUsuarioId = rol.Id, IsDeleted = true };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var retrievedUsuario = await _usuarioRepository.GetEntityByIdAsync(usuario.Id);

            Assert.NotNull(retrievedUsuario); 
            Assert.True(retrievedUsuario.IsDeleted); 
            Assert.NotNull(retrievedUsuario.RolUsuario); 
        }
    }
}