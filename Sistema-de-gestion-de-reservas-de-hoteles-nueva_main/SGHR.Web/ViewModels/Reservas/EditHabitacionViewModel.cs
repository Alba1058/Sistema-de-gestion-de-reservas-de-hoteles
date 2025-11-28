using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.DTOs.Reservas.Habitacion;

namespace SGHR.Web.ViewModels.Reservas
{
    public class EditHabitacionViewModel
    {
        public UpdateHabitacionDTO Habitacion { get; set; } = new();
        public List<CategoriaDTO> Categorias { get; set; } = new();
        public List<PisoDTO> Pisos { get; set; } = new();
        public Dictionary<int, string> EstadosHabitacion { get; set; } = new()
        {
            { 1, "Disponible" },
            { 2, "Ocupada" },
            { 3, "Mantenimiento" },
            { 4, "Reservada" }
        };
    }
}

