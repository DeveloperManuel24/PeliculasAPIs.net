using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.Entidades
{
    public class Actor
    {
        public int Id { get; set; }
        [StringLength(150)]

        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        [Unicode]
        public string? Foto { get; set; }
        public List<ActorPelicula> ActoresPeliculas { get; set; } = new List<ActorPelicula>();//RELACION MUCHOS A MUCHOS

    }
}
