using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.DTOs
{
    public class PeliculaDTO
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        [Unicode]
        public string? Poster { get; set; }
        public List<ComentarioDTO> Comentarios { get; set; } = new List<ComentarioDTO>();
        public List<GeneroDTO> Generos { get; set; } = new List<GeneroDTO>();
        public List<ActorPeliculaDTO> Actores { get; set; } = new List<ActorPeliculaDTO>();


    }
}
