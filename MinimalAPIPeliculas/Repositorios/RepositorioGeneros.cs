﻿using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Entidades;

namespace MinimalAPIPeliculas.Repositorios
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly ApplicationDbContext context;

        public RepositorioGeneros(ApplicationDbContext context)
        {
            this.context = context;
        }

       
        public async Task<int> Crear(Genero genero)
        {
            context.Add(genero);
            await context.SaveChangesAsync();
            return genero.Id;
        }

       
        public async Task<Genero?> ObtenerPorId(int id)
        {
            return await context.Generos.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Genero>> ObtenerTodos()
        {
            return await context.Generos.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Generos.AnyAsync(x => x.Id == id);
        }

        public async Task Actualizar(Genero genero)
        {
            context.Update(genero);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<List<int>> Existen(List<int> ids)//Metodo que se complemeta con la asignacion de un genero
        {
            return await context.Generos.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync();
        }

        public async Task<bool> Existe(int id, string nombre)//Validacion especial para usarse con AddValidatorsFromAssemblyContaining
        {
            return await context.Generos.AnyAsync(g => g.Id != id && g.Nombre == nombre);
        }


    }

}
