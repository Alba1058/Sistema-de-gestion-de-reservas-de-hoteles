using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Clientes;

namespace SGHR.Application.Mappers.Clientes
{
    public static class ClienteMapper
    {
        public static ClienteDTO ToClienteDto(Cliente? entity)
        {
            if (entity == null)
                return new ClienteDTO();

            return new ClienteDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                Email = entity.Email,
                Telefono = entity.Telefono,
                Direccion = entity.Direccion,
                Identificacion = entity.Identificacion
            };
        }

        public static Cliente CreateClienteEntity(ClienteCreateDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Cliente
            {
                Nombre = MapperHelper.Clean(dto.Nombre),
                Apellido = MapperHelper.Clean(dto.Apellido),
                Email = MapperHelper.Clean(dto.Email),
                Telefono = MapperHelper.Clean(dto.Telefono),
                Direccion = MapperHelper.Clean(dto.Direccion),
                Identificacion = MapperHelper.Clean(dto.Identificacion)
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateClienteFromDto(Cliente entity, ClienteUpdateDTO dto, string? usuario = null)
        {
            if (entity == null || dto == null) return;

            entity.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? entity.Nombre : MapperHelper.Clean(dto.Nombre);
            entity.Apellido = string.IsNullOrWhiteSpace(dto.Apellido) ? entity.Apellido : MapperHelper.Clean(dto.Apellido);
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? entity.Email : MapperHelper.Clean(dto.Email);
            entity.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? entity.Telefono : MapperHelper.Clean(dto.Telefono);
            entity.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? entity.Direccion : MapperHelper.Clean(dto.Direccion);
            entity.Identificacion = string.IsNullOrWhiteSpace(dto.Identificacion) ? entity.Identificacion : MapperHelper.Clean(dto.Identificacion);

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
