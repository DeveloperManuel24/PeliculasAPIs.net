using AutoMapper;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;

namespace MinimalAPIPeliculas.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Mapeo de genero
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();
            //Mapeo de Actores
            CreateMap<CrearActorDTO, Actor>().ForMember(x => x.Foto, opciones => opciones.Ignore());//Como de crearActorDTO a la entidad Actor la foto es un IForm y el otro un string, asi le decis que ignore ese.
            CreateMap<Actor, ActorDTO>();
            //Mapeo de peliculas:
            CreateMap<CrearPeliculaDTO, Pelicula>().ForMember(x => x.Poster, opciones => opciones.Ignore());
            CreateMap<Pelicula, PeliculaDTO>()
                         .ForMember(p => p.Generos,
                             entidad => entidad.MapFrom(p => p.GenerosPeliculas.Select(gp =>
                                 new GeneroDTO { Id = gp.GeneroId, Nombre = gp.Genero.Nombre })))
                         .ForMember(p => p.Actores, entidad => entidad.MapFrom(p =>
                             p.ActoresPeliculas.Select(ap =>
                                 new ActorPeliculaDTO
                                 {
                                     Id = ap.ActorId,
                                     Nombre = ap.Actor.Nombre,
                                     Personaje = ap.Personaje
                                 })));

            //Mapeo de Comentarios
            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            //Asignar un actor a una pelicula:
            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();

        }
    }
}
