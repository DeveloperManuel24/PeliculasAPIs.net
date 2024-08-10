using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Filtros;
using MinimalAPIPeliculas.Repositorios;
using MinimalAPIPeliculas.Servicios;

namespace MinimalAPIPeliculas.EndPoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
            .CacheOutput(c =>
             c.Expire(TimeSpan.FromSeconds(60))
             .Tag("comentarios-get")
             .SetVaryByRouteValue(new string[] { "peliculaId" }));//Esto es vital!!! ya que sino  se almacena en cache los comentarios de la peli anterior
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization();
            group.MapPut("/{id:int}", Actualizar).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization();
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization();


            return group;

        }


        static async Task<Results<Created<ComentarioDTO>, NotFound,BadRequest<string>>> Crear(
                                                            int peliculaId,
                                                            CrearComentarioDTO crearComentarioDTO,
                                                            IRepositorioComentarios repositorioComentarios,
                                                            IRepositorioPeliculas repositorioPeliculas,
                                                            IMapper mapper,
                                                            IOutputCacheStore outputCacheStore,
                                                            IServicioUsuarios servicioUsuarios)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;

            //Colocando del servicio de httpContext el email del usuario de manera segura en el atributo comentario(JAMAS RECIBIRLO COMO PARAMETRO)!!!
            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            comentario.UsuarioId = usuario.Id;


            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"comentario/{id}", comentarioDTO);
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(
                                                                    int peliculaId,
                                                                    IRepositorioComentarios repositorioComentarios,
                                                                    IRepositorioPeliculas repositorioPeliculas,
                                                                    IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);
        }


        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(
                                                                int peliculaId, int id,
                                                                IRepositorioComentarios repositorio,
                                                                IMapper mapper)
        {
            var comentario = await repositorio.ObtenerPorId(id);

            if (comentario is null)
            {
                return TypedResults.NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comentarioDTO);
        }


        static async Task<Results<NoContent, NotFound,ForbidHttpResult>> Actualizar(int peliculaId, int id, 
                                                        CrearComentarioDTO crearComentarioDTO, 
                                                        IOutputCacheStore outputCacheStore, 
                                                        IRepositorioComentarios repositorioComentarios, 
                                                        IRepositorioPeliculas repositorioPeliculas,                                                        
                                                        IServicioUsuarios servicioUsuarios)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            //Luego de verificar que la pelicula exista, vamos a proceder a ver como actualizar un comen
            var comentarioBD = await repositorioComentarios.ObtenerPorId(id);
            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioBD.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }

           comentarioBD.Cuerpo = crearComentarioDTO.Cuerpo;

            await repositorioComentarios.Actualizar(comentarioBD);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }




        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Borrar(int peliculaId, 
                                                        int id, 
                                                        IRepositorioComentarios repositorio, 
                                                        IOutputCacheStore outputCacheStore,
                                                        IServicioUsuarios servicioUsuarios)
        {
            //Luego de verificar que la pelicula exista, vamos a proceder a ver como actualizar un comen
            var comentarioBD = await repositorio.ObtenerPorId(id);
            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioBD.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }


            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }


    }
}
