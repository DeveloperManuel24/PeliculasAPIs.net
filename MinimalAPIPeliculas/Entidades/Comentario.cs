using Microsoft.AspNetCore.Identity;

namespace MinimalAPIPeliculas.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Cuerpo { get; set; } = null!;
        public int PeliculaId { get; set; }//Muchos comentarios a una sola pelicula
        //Seccion para que se vea que usaurio dijo tal comentario
        public string UsuarioId { get; set; } = null!;
        public IdentityUser Usuario { get; set; } = null!;
    }

}
