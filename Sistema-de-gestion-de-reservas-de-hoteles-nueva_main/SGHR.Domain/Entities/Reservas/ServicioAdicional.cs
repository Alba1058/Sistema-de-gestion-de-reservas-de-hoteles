using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class ServicioAdicional : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }

        public ICollection<ReservaServicio> ReservaServicios { get; set; } = []; 
        public ICollection<HabitacionServicio> HabitacionServicios { get; set; } = []; 


    }
}