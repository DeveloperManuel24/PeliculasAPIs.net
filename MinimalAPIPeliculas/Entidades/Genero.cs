using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.Entidades
{
    public class Genero
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
        public List<GeneroPelicula> GeneroPeliculaS { get; set; } = new List<GeneroPelicula>();//Relacion muchos a muchos, por medio de tabla intermedia
    }
}
