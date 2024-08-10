using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Filtros;
using MinimalAPIPeliculas.Migrations;
using MinimalAPIPeliculas.Repositorios;

namespace MinimalAPIPeliculas.EndPoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            //Crear
            group.MapPost("/", CrearGenero).AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>().RequireAuthorization("esadmin"); ;

            //Obtener Todos
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get")).RequireAuthorization();//el tag significa caché, es el nombre que le decimos y ese 15 dice que cada 15 seg se va limpiar

            //Obtener por Id
            group.MapGet("/{id:int}", ObtenerGeneroPorId);
         
            //Actualizar:
            group.MapPut("/{id:int}", ActualizarGenero).AddEndpointFilter< FiltroValidaciones<CrearGeneroDTO>>().RequireAuthorization("esadmin"); ;

            //Eliminar
            group.MapDelete("/{id:int}", BorrarGenero).RequireAuthorization("esadmin"); ;

            return group;
        }

        static async Task<Results<Created<GeneroDTO>, ValidationProblem>> CrearGenero(CrearGeneroDTO crearGeneroDTO, 
                                                                          IRepositorioGeneros repositorio, 
                                                                          IOutputCacheStore outputCacheStore, 
                                                                          IMapper mapper)
        {
           

            //Definimos las propiedades de entrada que queremos que se metan, esque daria error si no hacemos un DTO, ya que tiene Entity el id implicito
            //var genero = new Genero
            //{
            //    Nombre = crearGeneroDTO.Nombre
            //};
            //Nos evitamos eso con:
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            //var generoDTO = new GeneroDTO
            //{
            //    Id = id,
            //    Nombre = genero.Nombre
            //};
            var generoDTO = mapper.Map<GeneroDTO>(genero);//SE HACE DOS VECES PORQUE ASI TE EVITAS COMPLETAMENTE DE QUE EL USER NO PUEDA O TENGA FORMA DE METER EL ID
            return TypedResults.Created($"/generos/{id}", generoDTO);

        }

        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodos();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDTO);
        }


        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO = mapper.Map<GeneroDTO>(genero);

            return TypedResults.Ok(generoDTO);
        }


        public static async Task<Results<NoContent, NotFound, ValidationProblem>> ActualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, 
                                                                                IRepositorioGeneros repositorio, 
                                                                                IOutputCacheStore outputCacheStore, 
                                                                                IMapper mapper)
        {

            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id;

            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            return TypedResults.NoContent();
        }



        static async Task<Results<NoContent, NotFound>> BorrarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            return TypedResults.NoContent();
        }



    }
}
