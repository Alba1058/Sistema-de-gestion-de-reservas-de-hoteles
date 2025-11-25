using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Application.Services.Reservas;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.Application.Test2.Reservas
{
    public class ServicioAdicionalServiceTest
    {
        private readonly SGHRContext _contexto;
        private readonly IServicioAdicionalRepository _repositorio;
        private readonly ServicioAdicionalService _servicio;
        private readonly ILogger<ServicioAdicionalService> _loggerServicio;

        private readonly ServicioAdicional servicioExistente;

        public ServicioAdicionalServiceTest()
        {
            var opciones = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new SGHRContext(opciones);
            _contexto.Database.EnsureCreated();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var loggerRepositorio = loggerFactory.CreateLogger<ServicioAdicionalRepository>();
            _loggerServicio = loggerFactory.CreateLogger<ServicioAdicionalService>();

            _repositorio = new ServicioAdicionalRepository(_contexto, loggerRepositorio);
            _servicio = new ServicioAdicionalService(_repositorio, _loggerServicio);

            servicioExistente = new ServicioAdicional { Nombre = "WiFi Básico", Precio = 100, Estado = true };
            _repositorio.SaveEntityAsync(servicioExistente).Wait();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateServiceSuccessfully()
        {
            var dto = new CreateServicioAdicionalDTO
            {
                Nombre = "Servicio de Transporte",
                Precio = 500,
                Descripcion = "Transporte desde el aeropuerto",
                Estado = true
            };

            var resultado = await _servicio.CreateAsync(dto);
            var servicioEnDb = await _repositorio.GetEntityByIdAsync(resultado.Data!.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal("Servicio de Transporte", resultado.Data.Nombre);
            Assert.NotNull(servicioEnDb);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNombreIsEmpty()
        {
            var dto = new CreateServicioAdicionalDTO
            {
                Nombre = "",
                Precio = 100
            };

            var resultado = await _servicio.CreateAsync(dto);

            Assert.False(resultado.Success);
            Assert.Equal("El campo 'Nombre' es obligatorio.", resultado.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenPrecioIsNegative()
        {
            var dto = new CreateServicioAdicionalDTO
            {
                Nombre = "Servicio Gratis",
                Precio = -50
            };

            var resultado = await _servicio.CreateAsync(dto);

            Assert.False(resultado.Success);
            Assert.Equal("El precio no puede ser negativo.", resultado.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNombreIsDuplicate()
        {
            var dto = new CreateServicioAdicionalDTO
            {
                Nombre = "WiFi Básico",
                Precio = 100
            };

            var resultado = await _servicio.CreateAsync(dto);

            Assert.False(resultado.Success);
            Assert.Equal("Ya existe un servicio con ese nombre.", resultado.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateServiceSuccessfully()
        {
            var dtoActualizar = new UpdateServicioAdicionalDTO
            {
                Id = servicioExistente.Id,
                Nombre = "Spa Premium",
                Precio = 500,
                Descripcion = "Acceso completo al spa",
                Estado = true
            };

            var resultado = await _servicio.UpdateAsync(dtoActualizar);
            var servicioEnDb = await _repositorio.GetEntityByIdAsync(servicioExistente.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal("Spa Premium", resultado.Data.Nombre);
            Assert.Equal(500, servicioEnDb?.Precio);
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenNotFound()
        {
            var dto = new UpdateServicioAdicionalDTO { Id = 999, Nombre = "Fantasma" };
            var result = await _servicio.UpdateAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El servicio adicional no existe.", result.Message);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveServiceSuccessfully()
        {
            var dtoEliminar = new DeleteServicioAdicionalDTO { Id = servicioExistente.Id };

            var resultado = await _servicio.RemoveAsync(dtoEliminar);
            var servicioEnDb = await _repositorio.GetEntityByIdAsync(servicioExistente.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(servicioEnDb);
            Assert.True(servicioEnDb.IsDeleted);
        }

        [Fact]
        public async Task RemoveAsync_ShouldFail_WhenNotFound()
        {
            var dto = new DeleteServicioAdicionalDTO { Id = 999 };
            var result = await _servicio.RemoveAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El servicio adicional no existe.", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetServiceSuccessfully()
        {
            var resultado = await _servicio.GetByIdAsync(servicioExistente.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal(servicioExistente.Nombre, resultado.Data.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenNotFound()
        {
            var result = await _servicio.GetByIdAsync(999);

            Assert.False(result.Success);
            Assert.Equal("No se encontró el servicio adicional.", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenIsDeleted()
        {
            var servicioBorrado = new ServicioAdicional { Nombre = "Borrado", Precio = 1 };
            await _repositorio.SaveEntityAsync(servicioBorrado);
            await _repositorio.DeleteEntityAsync(servicioBorrado);

            var result = await _servicio.GetByIdAsync(servicioBorrado.Id);

            Assert.False(result.Success);
            Assert.Equal("No se encontró el servicio adicional.", result.Message);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_OnlyNotDeleted()
        {
            var servicioBorrado = new ServicioAdicional { Nombre = "Borrado", Precio = 1 };
            await _repositorio.SaveEntityAsync(servicioBorrado);
            await _repositorio.DeleteEntityAsync(servicioBorrado);

            var result = await _servicio.GetAllAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal(servicioExistente.Nombre, result.Data[0].Nombre);
        }

        [Fact]
        public async Task GetServiciosDisponiblesAsync_ShouldReturnOnlyActiveAndNotDeleted()
        {
            await _repositorio.SaveEntityAsync(new ServicioAdicional { Nombre = "Activo 2", Precio = 1, Estado = true, IsDeleted = false });
            await _repositorio.SaveEntityAsync(new ServicioAdicional { Nombre = "Inactivo", Precio = 1, Estado = false, IsDeleted = false });

            var result = await _servicio.GetServiciosDisponiblesAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }
    }
}