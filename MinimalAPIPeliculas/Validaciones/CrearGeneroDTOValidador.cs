﻿using FluentValidation;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositorios;

namespace MinimalAPIPeliculas.Validaciones
{
    public class CrearGeneroDTOValidador : AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador(IRepositorioGeneros repositorioGeneros, IHttpContextAccessor httpContextAccessor)
        {
            var valorDeRutaId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];//Variables de ruta, de esta manera puedo usarlo en actualizar tambien
            var id = 0;

            if (valorDeRutaId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)//Reutilizando codigo
                .MaximumLength(50).WithMessage(Utilidades.MaximumLengthMensaje)
                .Must(Utilidades.PrimeraLetraEnMayusculas).WithMessage(Utilidades.PrimeraLetraMayuscula)
                .MustAsync(async (nombre, _) =>
                {
                    var existe = await repositorioGeneros.Existe(id, nombre);
                    return !existe;
                }).WithMessage(g => $"Ya existe un género con el nombre {g.Nombre}");
        }




        

    }
}
