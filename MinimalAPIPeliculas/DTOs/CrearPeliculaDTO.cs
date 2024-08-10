using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.DTOs
{
    public class CrearPeliculaDTO
    {
        [StringLength(150)]
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        [Unicode]
        public IFormFile? Poster { get; set; }
    }
}
