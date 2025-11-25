using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Mappers.Configuration
{
    public static class PisoMapper
    {
        public static PisoDTO ToPisoDto(Piso p)
        {
            ArgumentNullException.ThrowIfNull(p);

            return new PisoDTO
            {
                Id = p.Id,
                Numero = p.Numero,
                Descripcion = MapperHelper.Clean(p.Descripcion),
                Estado = !p.IsDeleted
            };
        }

        public static Piso CreatePisoEntity(CreatePisoDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Piso
            {
                Numero = dto.Numero,
                Descripcion = MapperHelper.Clean(dto.Descripcion)
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdatePisoFromDto(Piso entity, UpdatePisoDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);

            entity.Numero = dto.Numero;
            entity.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion)
                ? entity.Descripcion
                : MapperHelper.Clean(dto.Descripcion);
            entity.Estado = dto.Estado;
            entity.IsDeleted = !dto.Estado;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
