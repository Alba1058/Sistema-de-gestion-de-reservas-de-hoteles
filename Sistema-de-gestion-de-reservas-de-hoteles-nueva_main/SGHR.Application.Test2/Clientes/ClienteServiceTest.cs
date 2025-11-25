using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.Services.Clientes;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories.Clientes;

namespace SGHR.Application.Test2.Clientes
{
    public class ClienteServiceTest
    {
        private readonly SGHRContext _context;
        private readonly ClienteRepository _clienteRepository;
        private readonly ClienteService _clienteService;
        private readonly ILogger<ClienteService> _logger;

        private readonly Cliente clienteExistente = new Cliente
        {
            Nombre = "Alba",
            Apellido = "Then",
            Email = "alba@email.com",
            Identificacion = "001-1234567-8",
            Telefono = "8095551234",
            Direccion = "Calle ejemplo 10"
        };

        public ClienteServiceTest()
        {

            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _context.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ClienteService>();
            var loggerRepo = loggerFactory.CreateLogger<ClienteRepository>();

            _clienteRepository = new ClienteRepository(_context, loggerRepo);

          
            _clienteService = new ClienteService(_clienteRepository, _logger);

            _clienteRepository.SaveEntityAsync(clienteExistente).Wait();

            _clienteRepository.SaveEntityAsync(new Cliente
            {
                Nombre = "Pedro",
                Apellido = "Gómez",
                Email = "pedro@email.com",
                Identificacion = "002-7654321-0",
                Telefono = "8095554321"
            }).Wait();
        }


        [Fact]
        public async Task CreateAsync_When_Valid_Returns_Success()
        {
            var dto = new ClienteCreateDTO
            {
                Nombre = "Maria",
                Apellido = "Lopez",
                Email = "maria@email.com",
                Identificacion = "003-4567890-1",
                Telefono = "8095556789"
            };

            var result = await _clienteService.CreateAsync(dto);
            var clienteEnDb = await _clienteRepository.GetClienteByIdentificacionAsync("003-4567890-1");

            Assert.True(result.Success);
            Assert.NotNull(result.Data); 
            Assert.Equal("Maria", result.Data.Nombre);
            Assert.NotNull(clienteEnDb); 
        }

        [Fact]
        public async Task CreateAsync_When_EmailExists_ShouldReturnFail()
        {
            var dto = new ClienteCreateDTO
            {
                Nombre = "Test",
                Apellido = "Duplicado",
                Email = "alba@email.com", 
                Identificacion = "111-1111111-1",
                Telefono = "111111"
            };

            var result = await _clienteService.CreateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Ya existe un cliente con este correo.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_When_IdentificacionExists_ShouldReturnFail()
        {
            var dto = new ClienteCreateDTO
            {
                Nombre = "Test",
                Apellido = "Duplicado",
                Email = "nuevo@email.com",
                Identificacion = "001-1234567-8", 
                Telefono = "111111"
            };

            var result = await _clienteService.CreateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Ya existe un cliente con esta identificación.", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_When_Valid_Returns_Success()
        {
            var dto = new ClienteUpdateDTO
            {
                Id = clienteExistente.Id, 
                Nombre = "Alba Actualizada",
                Apellido = clienteExistente.Apellido,
                Email = clienteExistente.Email,
                Identificacion = clienteExistente.Identificacion,
                Telefono = clienteExistente.Telefono
            };

            var result = await _clienteService.UpdateAsync(dto);
            var clienteEnDb = await _clienteRepository.GetEntityByIdAsync(clienteExistente.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Alba Actualizada", result.Data.Nombre);
            Assert.Equal("Alba Actualizada", clienteEnDb?.Nombre); 
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCliente_WhenExists()
        {
            int idBuscado = clienteExistente.Id; 

            var result = await _clienteService.GetByIdAsync(idBuscado);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(clienteExistente.Nombre, result.Data.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFail_WhenClienteIsDeleted()
        {
            var clienteBorrado = new Cliente { Nombre = "Cliente", Apellido = "Borrado", Email = "b@b.com", Telefono = "123", Identificacion = "borrado" };
            await _clienteRepository.SaveEntityAsync(clienteBorrado);
            await _clienteRepository.DeleteEntityAsync(clienteBorrado); 

            var result = await _clienteService.GetByIdAsync(clienteBorrado.Id);

            Assert.False(result.Success);
            Assert.Equal("Cliente no encontrado.", result.Message);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyNotDeleted()
        {

            var clienteBorrado = new Cliente { Nombre = "Cliente", Apellido = "Borrado", Email = "b@b.com", Telefono = "123", Identificacion = "borrado" };
            await _clienteRepository.SaveEntityAsync(clienteBorrado);
            await _clienteRepository.DeleteEntityAsync(clienteBorrado); 

            var result = await _clienteService.GetAllAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.DoesNotContain(result.Data, c => c.Nombre == "Borrado");
        }


        [Fact]
        public async Task RemoveAsync_ShouldFail_WhenClienteNotFound()
        {
            var dto = new ClienteDeleteDTO { Id = 999 };

            var result = await _clienteService.RemoveAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Cliente no encontrado.", result.Message);
        }

        [Fact]
        public async Task RemoveAsync_ShouldSuccess_WhenClienteExists()
        {
            var dto = new ClienteDeleteDTO { Id = clienteExistente.Id };

            var result = await _clienteService.RemoveAsync(dto);
            var clienteEnDb = await _clienteRepository.GetEntityByIdAsync(clienteExistente.Id);

            Assert.True(result.Success);
            Assert.NotNull(clienteEnDb); 
            Assert.True(clienteEnDb.IsDeleted); 
        }
    }
}