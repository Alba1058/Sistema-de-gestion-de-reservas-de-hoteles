using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Entities.Clientes;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Domain.Enums;
using System;

namespace SGHR.Application.Mappers
{
    public static class ConfigurationMappers
    {
        public static CategoriaDTO ToCategoriaDto(Categoria c) =>
            new CategoriaDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Estado = !c.IsDeleted
            };

        public static Categoria CreateCategoriaEntity(CreateCategoriaDTO dto) =>
            new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                IsDeleted = false,
                FechaCreacion = DateTime.UtcNow
            };

        public static PisoDTO ToPisoDto(Piso p) =>
            new PisoDTO
            {
                Id = p.Id,
                Numero = p.Numero,
                Descripcion = p.Descripcion,
                Estado = !p.IsDeleted
            };

        public static Piso CreatePisoEntity(CreatePisoDTO dto) =>
            new Piso
            {
                Numero = dto.Numero,
                Descripcion = dto.Descripcion,
                IsDeleted = false,
                FechaCreacion = DateTime.UtcNow
            };

        public static RolUsuarioDTO ToRolUsuarioDto(RolUsuario r) =>
            new RolUsuarioDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Estado = !r.IsDeleted
            };

        public static RolUsuario CreateRolUsuarioEntity(CreateRolUsuarioDTO dto) =>
            new RolUsuario
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                IsDeleted = false,
                FechaCreacion = DateTime.UtcNow
            };

        public static UsuarioDTO ToUsuarioDto(Usuario u) =>
            new UsuarioDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                RolUsuarioId = u.RolUsuarioId,
                RolNombre = u.RolUsuario?.Nombre ?? string.Empty,
                Activo = u.Estado
            };

        public static Usuario CreateUsuarioEntity(UsuarioCreateDTO dto) =>
            new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Contrasena = dto.Contrasena, 
                RolUsuarioId = dto.RolUsuarioId,
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

        public static void UpdateUsuarioFromDto(Usuario entity, UsuarioUpdateDTO dto)
        {
            entity.Nombre = dto.Nombre ?? entity.Nombre;
            entity.Email = dto.Email ?? entity.Email;
            entity.Estado = dto.Activo;
            entity.RolUsuarioId = dto.RolUsuarioId;
            entity.FechaModificacion = DateTime.UtcNow;
        }

  
        public static ClienteDTO ToClienteDto(Cliente c)
        {
           
            string documento = string.Empty;
            var propDoc = typeof(Cliente).GetProperty("DocumentoDeIdentidad");
            if (propDoc != null)
            {
                var val = propDoc.GetValue(c, null);
                documento = val != null ? val.ToString() ?? string.Empty : string.Empty;
            }

            return new ClienteDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                DocumentoDeIdentidad = documento,
                Email = c.Email,
                Telefono = c.Telefono
            };
        }

        public static Cliente CreateClienteEntity(ClienteCreateDTO dto)
        {
            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Direccion = null,
                IsDeleted = false,
                FechaCreacion = DateTime.UtcNow
            };

           
            var propDoc = typeof(Cliente).GetProperty("DocumentoDeIdentidad");
            if (propDoc != null && !string.IsNullOrWhiteSpace(dto.DocumentoDeIdentidad))
            {
                propDoc.SetValue(cliente, dto.DocumentoDeIdentidad);
            }

            return cliente;
        }

        public static void UpdateClienteFromDto(Cliente entity, ClienteUpdateDTO dto)
        {
            if (entity == null || dto == null) return;

            entity.Nombre = dto.Nombre ?? entity.Nombre;
            entity.Email = dto.Email ?? entity.Email;
            entity.Telefono = dto.Telefono ?? entity.Telefono;
            entity.FechaModificacion = DateTime.UtcNow;
           
        }

        public static TarifaDTO ToTarifaDto(Tarifa t) =>
            new TarifaDTO
            {
                Id = t.Id,
                Tipo = t.Tipo,
                Monto = t.Monto,
                FechaInicio = t.FechaInicio,
                FechaFin = t.FechaFin,
                PrecioPorNoche = t.PrecioPorNoche,
                Descuento = t.Descuento,
                Descripcion = t.Descripcion,
                IdHabitacion = t.IdHabitacion,
                Estado = !t.IsDeleted
            };

        public static Tarifa CreateTarifaEntity(CreateTarifaDTO dto) =>
            new Tarifa
            {
                Tipo = dto.Tipo,
                Monto = dto.Monto,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                PrecioPorNoche = dto.PrecioPorNoche,
                Descuento = dto.Descuento,
                Descripcion = dto.Descripcion,
                IdHabitacion = dto.IdHabitacion,
                IsDeleted = false,
                FechaCreacion = DateTime.UtcNow
            };
        public static void UpdateTarifaFromDto(Tarifa entity, UpdateTarifaDTO dto)
        {
            if (entity == null || dto == null) return;

            entity.IdHabitacion = dto.IdHabitacion;
            entity.FechaInicio = DateOnly.FromDateTime(dto.FechaInicio);
            entity.FechaFin = DateOnly.FromDateTime(dto.FechaFin);
            entity.FechaModificacion = DateTime.UtcNow;
            entity.IsDeleted = !dto.Estado;
        }


        public static ServicioAdicionalDTO ToServicioAdicionalDto(ServicioAdicional s) =>
            new ServicioAdicionalDTO
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Precio = s.Precio,
                Descripcion = s.Descripcion,
                Estado = !s.IsDeleted
            };

        public static ServicioAdicional CreateServicioAdicionalEntity(CreateServicioAdicionalDTO dto) =>
            new ServicioAdicional
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Descripcion = dto.Descripcion,
                IsDeleted = !dto.Estado,
                FechaCreacion = DateTime.UtcNow
            };

        public static void UpdateServicioAdicionalFromDto(ServicioAdicional entity, UpdateServicioAdicionalDTO dto)
        {
            if (entity == null || dto == null) return;
            entity.Nombre = dto.Nombre;
            entity.Precio = dto.Precio;
            entity.Descripcion = dto.Descripcion;
            entity.IsDeleted = !dto.Estado;
            entity.FechaModificacion = DateTime.UtcNow;
        }

        public static ReservaDTO ToReservaDto(Reserva r) =>
            new ReservaDTO
            {
                Id = r.Id,
                IdCliente = r.IdCliente,
                IdHabitacion = r.IdHabitacion,
                FechaInicio = r.FechaInicio,
                FechaFin = r.FechaFin,
                NumeroHuespedes = r.NumeroHuespedes,
                Total = r.Total,
                EstadoReserva = (int)r.EstadoReserva,
                Estado = true 
            };

        public static Reserva CreateReservaEntity(CreateReservaDTO dto)
        {
            var entidad = new Reserva
            {
                IdCliente = dto.IdCliente,
                IdHabitacion = dto.IdHabitacion,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                NumeroHuespedes = dto.NumeroHuespedes,
                Total = dto.Total,

                EstadoReserva = Enum.IsDefined(typeof(EstadoReserva), dto.EstadoReserva)
                    ? (EstadoReserva)dto.EstadoReserva
                    : EstadoReserva.Activa,
                FechaCreacion = DateTime.UtcNow
            };
            return entidad;
        }

        public static void UpdateReservaFromDto(Reserva entity, UpdateReservaDTO dto)
        {
            if (entity == null || dto == null) return;
            entity.IdCliente = dto.IdCliente;
            entity.IdHabitacion = dto.IdHabitacion;
            entity.FechaInicio = dto.FechaInicio;
            entity.FechaFin = dto.FechaFin;
            entity.NumeroHuespedes = dto.NumeroHuespedes;
            entity.Total = dto.Total;
            if (Enum.IsDefined(typeof(EstadoReserva), dto.EstadoReserva))
                entity.EstadoReserva = (EstadoReserva)dto.EstadoReserva;
            entity.FechaModificacion = DateTime.UtcNow;
        }

        public static HabitacionDTO ToHabitacionDto(Habitacion h) =>
            new HabitacionDTO
            {
                Id = h.Id,
                Numero = h.Numero,
                IdCategoria = h.IdCategoria,
                IdPiso = h.IdPiso,
                EstadoHabitacion = (int)h.EstadoH,
                PrecioBase = h.PrecioBase,
                Descripcion = h.Descripcion,
                Estado = true 
            };

        public static Habitacion CreateHabitacionEntity(CreateHabitacionDTO dto) =>
            new Habitacion
            {
                Numero = dto.Numero,
                IdCategoria = dto.IdCategoria,
                IdPiso = dto.IdPiso,
                EstadoH = Enum.IsDefined(typeof(EstadoHabitacion), dto.EstadoHabitacion)
                            ? (EstadoHabitacion)dto.EstadoHabitacion
                            : EstadoHabitacion.Disponible,
                PrecioBase = dto.PrecioBase,
                Descripcion = dto.Descripcion,
                FechaCreacion = DateTime.UtcNow
            };

        public static void UpdateHabitacionFromDto(Habitacion entity, UpdateHabitacionDTO dto)
        {
            if (entity == null || dto == null) return;
            entity.Numero = dto.Numero ?? entity.Numero;
            entity.IdCategoria = dto.IdCategoria;
            entity.IdPiso = dto.IdPiso;
            if (Enum.IsDefined(typeof(EstadoHabitacion), dto.EstadoHabitacion))
                entity.EstadoH = (EstadoHabitacion)dto.EstadoHabitacion;
            entity.PrecioBase = dto.PrecioBase;
            entity.Descripcion = dto.Descripcion ?? entity.Descripcion;
            entity.FechaModificacion = DateTime.UtcNow;
        }

        public static PagoDTO ToPagoDto(Pago p) =>
            new PagoDTO
            {
                Id = p.Id,
                IdReserva = p.IdReserva,
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                Metodo = p.Metodo,
                Confirmado = p.Confirmado,
                Estado = true
            };

        public static Pago CreatePagoEntity(CreatePagoDTO dto) =>
            new Pago
            {
                IdReserva = dto.IdReserva,
                Monto = dto.Monto,
                FechaPago = dto.FechaPago,
                Metodo = dto.Metodo,
                Confirmado = dto.Confirmado,
                FechaCreacion = DateTime.UtcNow
            };

        public static void UpdatePagoFromDto(Pago entity, UpdatePagoDTO dto)
        {
            if (entity == null || dto == null) return;
            entity.IdReserva = dto.IdReserva;
            entity.Monto = dto.Monto;
            entity.FechaPago = dto.FechaPago;
            entity.Metodo = dto.Metodo;
            entity.Confirmado = dto.Confirmado;
            entity.FechaModificacion = DateTime.UtcNow;
        }
    }
}