using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Clientes;
using System;

namespace SGHR.Persistence.Test2.Clientes
{
    public class ClienteRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly ClienteRepository _clienteRepository;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ClienteRepository>();

            _clienteRepository = new ClienteRepository(_context, _logger);
        }


        [Fact]
        public async Task SaveEntityAsync_When_ValidCliente_ShouldSaveSuccessfully()
        {
            var cliente = new Cliente
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Identificacion = "001-1234567-8",
                Telefono = "8095555555",
                Email = "juan@test.com",
                Direccion = "Calle 1 #2"
            };


            var result = await _clienteRepository.SaveEntityAsync(cliente);
            var clienteGuardado = await _context.Clientes.FindAsync(cliente.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Juan", result.Data.Nombre);
            Assert.NotNull(clienteGuardado); 
        }

        [Fact]
        public async Task SaveEntityAsync_When_IdentificacionExists_ShouldSaveSuccessfully()
        {
            var clienteExistente = new Cliente { Nombre = "Cliente", Apellido = "Uno", Identificacion = "ID-REPETIDA", Telefono = "111", Email = "c1@test.com" };
            await _clienteRepository.SaveEntityAsync(clienteExistente);

            var clienteNuevo = new Cliente { Nombre = "Cliente", Apellido = "Dos", Identificacion = "ID-REPETIDA", Telefono = "222", Email = "c2@test.com" };

            var result = await _clienteRepository.SaveEntityAsync(clienteNuevo);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_ValidCliente_ShouldUpdateSuccessfully()
        {
            var cliente = new Cliente { Nombre = "Maria", Apellido = "Lopez", Identificacion = "002-1234567-8", Email = "m@test.com", Telefono = "456" };
            await _clienteRepository.SaveEntityAsync(cliente); 

            cliente.Nombre = "Maria Actualizada";
            cliente.Email = "maria.actualizada@test.com";

            var result = await _clienteRepository.UpdateEntityAsync(cliente);
            var clienteActualizado = await _clienteRepository.GetEntityByIdAsync(cliente.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Maria Actualizada", result.Data.Nombre);
            Assert.Equal("maria.actualizada@test.com", result.Data.Email);
            Assert.Equal("Maria Actualizada", clienteActualizado?.Nombre); 
        }

        [Fact]
        public async Task UpdateEntityAsync_When_ClienteNotFound_ShouldReturnFail()
        {
            var cliente = new Cliente { Id = 99, Nombre = "Fantasma", Identificacion = "999" };

            var result = await _clienteRepository.UpdateEntityAsync(cliente);

            Assert.False(result.Success);
            Assert.Contains("Error al actualizar la entidad", result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_IdentificacionExists_ShouldUpdateSuccessfully()
        {
            var cliente1 = new Cliente { Nombre = "Cliente", Apellido = "Uno", Identificacion = "ID-1", Telefono = "111", Email = "c1@test.com" };
            var cliente2 = new Cliente { Nombre = "Cliente", Apellido = "Dos", Identificacion = "ID-2", Telefono = "222", Email = "c2@test.com" };
            await _clienteRepository.SaveEntityAsync(cliente1);
            await _clienteRepository.SaveEntityAsync(cliente2);

            cliente2.Identificacion = "ID-1";
            var result = await _clienteRepository.UpdateEntityAsync(cliente2);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("ID-1", result.Data.Identificacion);
        }


        [Fact]
        public async Task GetClienteByIdentificacionAsync_When_Exists_ShouldReturnCliente()
        {

            var cliente = new Cliente { Nombre = "Laura", Apellido = "Torres", Identificacion = "004-1234567-8", Email = "l@test.com", Telefono = "789" };
            var saveResult = await _clienteRepository.SaveEntityAsync(cliente);
            Assert.True(saveResult.Success, "El guardado en Arrange falló"); 

            var result = await _clienteRepository.GetClienteByIdentificacionAsync("004-1234567-8");

            Assert.NotNull(result);
            Assert.Equal("Laura", result.Nombre);
        }

        [Fact]
        public async Task GetClientesConReservasAsync_When_None_ShouldReturnEmptyList()
        {
            var clientes = await _clienteRepository.GetClientesConReservasAsync();

            Assert.NotNull(clientes);
        }

        [Fact]
        public async Task GetClientesConReservasAsync_When_DbFails_ShouldReturnEmptyList()
        {
            _context.Dispose();

            var clientes = await _clienteRepository.GetClientesConReservasAsync();

            Assert.NotNull(clientes);
            Assert.Empty(clientes); 
        }
    }
}