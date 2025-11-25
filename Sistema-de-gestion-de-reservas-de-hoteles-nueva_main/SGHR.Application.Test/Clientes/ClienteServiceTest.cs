using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.Services.Clientes;
using SGHR.Application.Validators.Clientes;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Interfaces.Clientes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SGHR.Application.Test.Clientes
{
    public class ClienteServiceTest
    {
        private readonly Mock<IClienteRepository> _clienteRepoMock;
        private readonly Mock<IValidator<Cliente>> _validatorMock;
        private readonly Mock<ILogger<ClienteService>> _loggerMock;
        private readonly ClienteService _clienteService;

        public ClienteServiceTest()
        {
            _clienteRepoMock = new Mock<IClienteRepository>();
            _validatorMock = new Mock<IValidator<Cliente>>();
            _loggerMock = new Mock<ILogger<ClienteService>>();

            _clienteService = new ClienteService(
                _clienteRepoMock.Object,
                _validatorMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenClienteIsValid()
        {
            // Arrange
            var dto = new ClienteCreateDTO
            {
                Nombre = "Alba",
                Apellido = "Then",
                Email = "alba@email.com",
                Identificacion = "001-1234567-8",
                Telefono = "8095551234",
                Direccion = "Calle ejemplo 10"
            };

            var entity = new Cliente { Id = 1, Nombre = dto.Nombre };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Cliente>()))
                .ReturnsAsync(new ValidationResult { IsValid = true });

            _clienteRepoMock.Setup(r => r.SaveEntityAsync(It.IsAny<Cliente>()))
                .ReturnsAsync(OperationResult<Cliente>.Ok(entity));

            // Act
            var result = await _clienteService.CreateAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Alba", result.Data.Nombre);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenValidationFails()
        {
            // Arrange
            var dto = new ClienteCreateDTO { Nombre = "", Apellido = "" };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Cliente>()))
                .ReturnsAsync(new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { "Nombre requerido" }
                });

            // Act
            var result = await _clienteService.CreateAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Nombre requerido", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Arrange
            var dto = new ClienteUpdateDTO { Id = 1, Nombre = "Actualizado" };
            var entity = new Cliente { Id = 1, Nombre = "Original" };

            _clienteRepoMock.Setup(r => r.GetEntityByIdAsync(dto.Id))
                .ReturnsAsync(entity);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Cliente>()))
                .ReturnsAsync(new ValidationResult { IsValid = true });

            _clienteRepoMock.Setup(r => r.UpdateEntityAsync(It.IsAny<Cliente>()))
                .ReturnsAsync(OperationResult<Cliente>.Ok(entity));

            // Act
            var result = await _clienteService.UpdateAsync(dto);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCliente_WhenExists()
        {
            // Arrange
            var cliente = new Cliente { Id = 1, Nombre = "Carlos" };
            _clienteRepoMock.Setup(r => r.GetEntityByIdAsync(1))
                .ReturnsAsync(cliente);

            // Act
            var result = await _clienteService.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Carlos", result.Data.Nombre);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList_WhenClientesExist()
        {
            // Arrange
            var clientes = new List<Cliente> {
                new Cliente { Id = 1, Nombre = "Jose" },
                new Cliente { Id = 2, Nombre = "Maria" }
            };

            _clienteRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(clientes);

            // Act
            var result = await _clienteService.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task RemoveAsync_ShouldFail_WhenClienteNotFound()
        {
            // Arrange
            var dto = new ClienteDeleteDTO { Id = 999 };
            _clienteRepoMock.Setup(r => r.GetEntityByIdAsync(dto.Id))
                .ReturnsAsync((Cliente?)null);

            // Act
            var result = await _clienteService.RemoveAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cliente no encontrado para eliminar.", result.Message);
        }

        [Fact]
        public async Task GetClientesConReservasAsync_ShouldReturnClientes()
        {
            // Arrange
            var clientes = new List<Cliente>
            {
                new Cliente { Id = 1, Nombre = "Alba" },
                new Cliente { Id = 2, Nombre = "Pedro" }
            };

            _clienteRepoMock.Setup(r => r.GetClientesConReservasAsync())
                .ReturnsAsync(clientes);

            // Act
            var result = await _clienteService.GetClientesConReservasAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }
    }
}
