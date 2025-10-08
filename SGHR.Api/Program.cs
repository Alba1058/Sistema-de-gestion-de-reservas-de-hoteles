using Microsoft.EntityFrameworkCore;
using SGHR.Application.Interfaces.Clientes;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Interfaces.Usuarios;
using SGHR.Application.Services.Clientes;
using SGHR.Application.Services.Configuracion;
using SGHR.Application.Services.Reservas;
using SGHR.Application.Services.Usuarios;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Clientes;
using SGHR.Persistence.Interfaces.Configuracion;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Interfaces.Usuarios;
using SGHR.Persistence.Repositories.Clientes;
using SGHR.Persistence.Repositories.Configuration;
using SGHR.Persistence.Repositories.Reservas;
using SGHR.Persistence.Repositories.Usuarios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SGHRContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SGHRConnection")));


builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepositoryADO>();
builder.Services.AddScoped<IPisoRepository, PisoRepository>();
builder.Services.AddScoped<IRolUsuarioRepository, RolUsuarioRepository>();
builder.Services.AddScoped<IHabitacionRepository, HabitacionRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IServicioAdicionalRepository, ServiciosAdicionalesRepository>();
builder.Services.AddScoped<ITarifaRepository, TarifaRepositoryADO>();
builder.Services.AddScoped<IPagoRepository, PagoRepositoryADO>();


builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IPisoService, PisoService>();
builder.Services.AddScoped<IRolUsuarioService, RolUsuarioService>();
builder.Services.AddScoped<IHabitacionService, HabitacionService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IServicioAdicionalService, ServicioAdicionalService>();
builder.Services.AddScoped<ITarifaService, TarifaService>();
builder.Services.AddScoped<IPagoService, PagoService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
