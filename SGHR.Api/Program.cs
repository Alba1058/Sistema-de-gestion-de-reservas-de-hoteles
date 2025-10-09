using Microsoft.EntityFrameworkCore;
using SGHR.Api.Extends;
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

// agrega servicios
builder.Services.AddPersistenceServices();
builder.Services.AddApplicationServices();


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