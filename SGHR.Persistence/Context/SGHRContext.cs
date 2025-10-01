using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Clientes;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Entities.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Context
{
    public class SGHRContext : DbContext
    {
        public SGHRContext(DbContextOptions<SGHRContext> options) : base(options) { }

        // Clientes y usuarios
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;

        // Reservas 
        public DbSet<Reserva> Reservas { get; set; } = null!;
        public DbSet<Habitacion> Habitaciones { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;
        public DbSet<ServicioAdicional> ServiciosAdicionales { get; set; } = null!;
        public DbSet<Tarifa> Tarifas { get; set; } = null!;

        // Configuración
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Piso> Pisos { get; set; } = null!;
        public DbSet<RolUsuario> RolesUsuario { get; set; } = null!;

    }
}