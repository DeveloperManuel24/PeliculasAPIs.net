using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Migrations;
using MinimalAPIPeliculas.Utilidades;

namespace MinimalAPIPeliculas.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioActores(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }


        public async Task<int> Crear(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }


        public async Task<Actor?> ObtenerPorId(int id)//Importante poner AsNoTracking
        {
            return await context.Actores.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(a => a.Nombre).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Actores.AnyAsync(x => x.Id == id);
        }


        public async Task<List<int>> Existen(List<int> ids)//Complementaria para poder asignar actor a una pelicula
        {
            return await context.Actores.Where(a => ids.Contains(a.Id)).Select(a => a.Id).ToListAsync();
        }


        public async Task Actualizar(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Actores.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<List<Actor>> ObtenerPorNombre(string nombre)
        {
            return await context.Actores
                .Where(a => a.Nombre.Contains(nombre))
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

      
    }
}
