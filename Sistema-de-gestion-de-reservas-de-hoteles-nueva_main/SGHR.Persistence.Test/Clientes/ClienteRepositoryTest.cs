using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Clientes;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SGHR.Persistence.Test.Clientes
{
    public class ClienteRepositoryTest
    {
        private readonly SGHRContext _context;
        private readonly ClienteRepository _clienteRepository;
        private readonly ILogger<ClienteRepository> _logger;
        private readonly IConfiguration _configuration;

        public ClienteRepositoryTest()
        {

            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ClienteRepository>();

            _configuration = new ConfigurationBuilder().Build();

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
                Direccion = "Calle 1 #2"
            };

            var result = await _clienteRepository.SaveEntityAsync(cliente);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Juan", result.Data.Nombre);
        }

        [Fact]
        public async Task SaveEntityAsync_When_DbFails_ShouldReturnFail()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nombre = "Error",
                Apellido = "DB",
                Identificacion = "001-1234567-8"
            };

            _context.Dispose();

            // Act
            var result = await _clienteRepository.SaveEntityAsync(cliente);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Error interno al guardar cliente.", result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_ValidCliente_ShouldUpdateSuccessfully()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nombre = "Maria",
                Apellido = "Lopez",
                Identificacion = "002-1234567-8"
            };

            await _clienteRepository.SaveEntityAsync(cliente);

            cliente.Nombre = "Maria Actualizada";

            // Act
            var result = await _clienteRepository.UpdateEntityAsync(cliente);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Maria Actualizada", result.Data.Nombre);
        }

        [Fact]
        public async Task UpdateEntityAsync_When_DbFails_ShouldReturnFail()
        {
            var cliente = new Cliente
            {
                Nombre = "Pedro",
                Apellido = "Error",
                Identificacion = "003-1234567-8"
            };

            _context.Dispose(); 

            var result = await _clienteRepository.UpdateEntityAsync(cliente);

            Assert.False(result.Success);
            Assert.Equal("Error interno al actualizar cliente.", result.Message);
        }


        [Fact]
        public async Task GetClienteByIdentificacionAsync_When_Exists_ShouldReturnCliente()
        {
            var cliente = new Cliente
            {
                Nombre = "Laura",
                Apellido = "Torres",
                Identificacion = "004-1234567-8"
            };

            await _clienteRepository.SaveEntityAsync(cliente);

            var result = await _clienteRepository.GetClienteByIdentificacionAsync("004-1234567-8");

            Assert.NotNull(result);
            Assert.Equal("Laura", result.Nombre);
        }

        [Fact]
        public async Task GetClienteByIdentificacionAsync_When_NotExists_ShouldReturnNull()
        {
            var result = await _clienteRepository.GetClienteByIdentificacionAsync("999-9999999-9");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetClientesConReservasAsync_When_None_ShouldReturnEmptyList()
        {
            var clientes = await _clienteRepository.GetClientesConReservasAsync();

            Assert.NotNull(clientes);
            Assert.Empty(clientes);
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
