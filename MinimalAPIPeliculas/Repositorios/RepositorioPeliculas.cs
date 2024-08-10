using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Utilidades;

namespace MinimalAPIPeliculas.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }

        async Task<List<Pelicula>> IRepositorioPeliculas.ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(p => p.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas
                .Include(p => p.Comentarios)
                .Include(p => p.GenerosPeliculas)
                    .ThenInclude(gp => gp.Genero)//GENEROS PELICUAS => GENERO(TE TRAES ESA DATA)
                .Include(p => p.ActoresPeliculas.OrderBy(a => a.Orden))
                    .ThenInclude(ap => ap.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        async Task<int> IRepositorioPeliculas.Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }
        async Task IRepositorioPeliculas.Actualizar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }
        async Task IRepositorioPeliculas.Borrar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        async Task<bool> IRepositorioPeliculas.Existe(int id)
        {
            return await context.Peliculas.AnyAsync(p => p.Id == id);
        }



        async Task IRepositorioPeliculas.AsignarGeneros(int id, List<int> generosIds)
        {
            var pelicula = await context.Peliculas.Include(p => p.GenerosPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe una película con el id {id}");
            }

            var generosPeliculas = generosIds.Select(generoId => new GeneroPelicula() { GeneroId = generoId });

            pelicula.GenerosPeliculas = mapper.Map(generosPeliculas, pelicula.GenerosPeliculas);

            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int id, List<ActorPelicula> actores)
        {
            for (int i = 1; i <= actores.Count; i++)
            {
                actores[i - 1].Orden = i;
            }

            var pelicula = await context.Peliculas
                .Include(p => p.ActoresPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe la película con id: {id}");
            }

            pelicula.ActoresPeliculas = mapper.Map(actores, pelicula.ActoresPeliculas);
            await context.SaveChangesAsync();
        }


    }

}
