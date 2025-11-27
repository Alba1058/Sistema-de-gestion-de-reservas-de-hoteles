using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.Services.Reservas;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuration;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Repositories.Configuration;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.Application.Test2.Reservas
{
    public class HabitacionServiceTest
    {
        private readonly SGHRContext _contexto;
        private readonly IHabitacionRepository _repoHabitacion;
        private readonly ICategoriaRepository _repoCategoria;
        private readonly IPisoRepository _repoPiso;
        private readonly HabitacionService _servicio;
        private readonly ILoggerFactory _loggerFactory;

        private Categoria categoriaSeed;
        private Piso pisoSeed;

        public HabitacionServiceTest()
        {
            var opciones = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new SGHRContext(opciones);
            _contexto.Database.EnsureCreated();
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var loggerHabitacionRepo = _loggerFactory.CreateLogger<HabitacionRepository>();
            var loggerPisoRepo = _loggerFactory.CreateLogger<PisoRepository>();
            var loggerCategoriaRepo = _loggerFactory.CreateLogger<CategoriaRepository>();
            var loggerService = _loggerFactory.CreateLogger<HabitacionService>();

            _repoHabitacion = new HabitacionRepository(_contexto, loggerHabitacionRepo);
            _repoCategoria = new CategoriaRepository(_contexto, loggerCategoriaRepo);
            _repoPiso = new PisoRepository(_contexto, loggerPisoRepo);

            _servicio = new HabitacionService(_repoHabitacion, _repoCategoria, _repoPiso, loggerService);

            categoriaSeed = new Categoria { Id = 1, Nombre = "Suite" };
            pisoSeed = new Piso { Id = 1, Numero = 1 };
            _contexto.Categorias.Add(categoriaSeed);
            _contexto.Pisos.Add(pisoSeed);
            _contexto.SaveChanges();

            _contexto.Habitaciones.AddRange(
                new Habitacion { Numero = 101, IdCategoria = categoriaSeed.Id, IdPiso = pisoSeed.Id, PrecioBase = 1500, Estado = true, EstadoH = EstadoHabitacion.Disponible },
                new Habitacion { Numero = 102, IdCategoria = categoriaSeed.Id, IdPiso = pisoSeed.Id, PrecioBase = 2500, Estado = true, EstadoH = EstadoHabitacion.Disponible }
            );
            _contexto.SaveChanges();
        }

        //[Fact]
        //public async Task CreateAsync_ShouldCreateRoomSuccessfully()
        //{
        //    var dto = new CreateHabitacionDTO
        //    {
        //        Numero = 201,
        //        IdCategoria = categoriaSeed.Id,
        //        IdPiso = pisoSeed.Id,
        //        EstadoHabitacion = (int)EstadoHabitacion.Disponible,
        //        PrecioBase = 1000
        //    };

        //    var resultado = await _servicio.CreateAsync(dto);
        //    var habitacionEnDb = await _repoHabitacion.GetEntityByIdAsync(resultado.Data!.Id);

        //    Assert.True(resultado.Success);
        //    Assert.NotNull(resultado.Data);
        //    Assert.Equal(201, resultado.Data.Numero);
        //    Assert.NotNull(habitacionEnDb);
        //}

        //[Fact]
        //public async Task CreateAsync_ShouldFail_WhenCategoriaNotFound()
        //{
        //    var dto = new CreateHabitacionDTO
        //    {
        //        Numero = 201,
        //        IdCategoria = 99,
        //        IdPiso = pisoSeed.Id,
        //        EstadoHabitacion = (int)EstadoHabitacion.Disponible,
        //        PrecioBase = 1000
        //    };

        //    var resultado = await _servicio.CreateAsync(dto);

        //    Assert.False(resultado.Success);
        //    Assert.Equal("La categoría especificada no existe.", resultado.Message);
        //}

        //[Fact]
        //public async Task CreateAsync_ShouldFail_WhenPisoNotFound()
        //{
        //    var dto = new CreateHabitacionDTO
        //    {
        //        Numero = 201,
        //        IdCategoria = categoriaSeed.Id,
        //        IdPiso = 99,
        //        EstadoHabitacion = (int)EstadoHabitacion.Disponible,
        //        PrecioBase = 1000
        //    };

        //    var resultado = await _servicio.CreateAsync(dto);

        //    Assert.False(resultado.Success);
        //    Assert.Equal("El piso especificado no existe.", resultado.Message);
        //}

        //[Fact]
        //public async Task CreateAsync_ShouldFail_WhenNumeroIsDuplicateOnSamePiso()
        //{
        //    var dto = new CreateHabitacionDTO
        //    {
        //        Numero = 101,
        //        IdCategoria = categoriaSeed.Id,
        //        IdPiso = pisoSeed.Id,
        //        EstadoHabitacion = (int)EstadoHabitacion.Disponible,
        //        PrecioBase = 1000
        //    };

        //    var resultado = await _servicio.CreateAsync(dto);

        //    Assert.False(resultado.Success);
        //    Assert.Equal("Ya existe una habitación con ese número en el mismo piso.", resultado.Message);
        //}

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNumeroIsZero()
        {
            var dto = new CreateHabitacionDTO
            {
                Numero = 0,
                IdCategoria = categoriaSeed.Id,
                IdPiso = pisoSeed.Id,
                EstadoHabitacion = (int)EstadoHabitacion.Disponible,
                PrecioBase = 1000
            };

            var resultado = await _servicio.CreateAsync(dto);

            Assert.False(resultado.Success);
            Assert.Equal("El número de habitación debe ser mayor que 0.", resultado.Message);
        }

        [Fact]
        public async Task ShouldGetRoomByIdSuccessfully()
        {
            var habitacion = await _contexto.Habitaciones.FirstAsync();
            var resultado = await _servicio.GetByIdAsync(habitacion.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal(habitacion.Id, resultado.Data.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFail_WhenIsDeleted()
        {
            var habitacion = await _contexto.Habitaciones.FirstAsync();
            await _repoHabitacion.DeleteEntityAsync(habitacion);

            var resultado = await _servicio.GetByIdAsync(habitacion.Id);

            Assert.False(resultado.Success);
            Assert.Equal("Habitación no encontrada.", resultado.Message);
        }

        [Fact]
        public async Task ShouldGetAllRoomsSuccessfully()
        {
            var resultado = await _servicio.GetAllAsync();

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Equal(2, resultado.Data.Count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_OnlyNotDeleted()
        {
            var habitacion = await _contexto.Habitaciones.FirstAsync();
            await _repoHabitacion.DeleteEntityAsync(habitacion);

            var resultado = await _servicio.GetAllAsync();

            Assert.True(resultado.Success);
            Assert.NotNull(resultado.Data);
            Assert.Single(resultado.Data);
        }

        [Fact]
        public async Task ShouldDeleteRoomSuccessfully()
        {
            var habitacion = await _contexto.Habitaciones.FirstAsync();
            var dtoEliminar = new DeleteHabitacionDTO { Id = habitacion.Id };

            var resultado = await _servicio.RemoveAsync(dtoEliminar);
            var habitacionEnDb = await _repoHabitacion.GetEntityByIdAsync(habitacion.Id);

            Assert.True(resultado.Success);
            Assert.NotNull(habitacionEnDb);
            Assert.True(habitacionEnDb.IsDeleted);
        }
    }
}