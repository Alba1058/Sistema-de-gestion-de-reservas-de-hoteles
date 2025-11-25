using Microsoft.EntityFrameworkCore;
using SGHR.IOC.Dependencias;
using SGHR.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SGHRContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SghrConnString")));

builder.Services.AddDependences();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins("http://localhost:5167", "http://localhost:40266", "https://localhost:7041", "http://localhost:7041")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowWebApp");

app.UseAuthorization();
app.MapControllers();
app.Run();