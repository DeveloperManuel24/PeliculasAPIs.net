using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.DTOs
{
    public class GeneroDTO
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
    }
}
