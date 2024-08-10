using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        [Unicode]
        public string? Poster { get; set; }
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();//Una Pelicula puede tener muchos comentarios  RELACION UNO A MUCHOS
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();//Relacion muchos a muchos, por medio de tabla intermedia
        public List<ActorPelicula> ActoresPeliculas { get; set; } = new List<ActorPelicula>();

    }

}
