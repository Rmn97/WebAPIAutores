﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTO;
using WebAPIAutores.Entidades;
using WebAPIAutores.Filtros;
using WebAPIAutores.Utilidades;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    //[Route("api/v1/autores")]
    [Route("api/autores")]
    [CabeceraEstaPresente("x-version", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }
        [HttpGet(Name = "obtenerAutoresv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);

        }
        [HttpGet("{id:int}", Name = "obtenerAutorv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(buscandoAutor => buscandoAutor.AutoresLibros)
                .ThenInclude(buscandoLibro => buscandoLibro.Libro)
                .FirstOrDefaultAsync(buscandoAutor => buscandoAutor.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);
            //var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            //GenerarEnlaces(dto, esAdmin.Succeeded);
            return dto;
        }
        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev1")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorExistente => autorExistente.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }
        [HttpPost(Name = "crearAutorv1")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var mismoAutor = await context.Autores.AnyAsync(nuevoAutor => nuevoAutor.Nombre == autorCreacionDTO.Nombre);

            if (mismoAutor)
            {
                return BadRequest($"Ya existe un autor con el mismo nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);

            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutorv1", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutorv1")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await context.Autores.AnyAsync(autorExistente => autorExistente.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            autor.Id = id;

            context.Update(autor);

            await context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Borra un autor
        /// </summary>
        /// <param name="id">Id del autor a borrar</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "borrarAutorv1")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(autorExistente => autorExistente.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
// 4,379 MXN
//