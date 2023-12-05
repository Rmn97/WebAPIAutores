﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTO;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros
                .Include(buscandoLibro => buscandoLibro.AutoresLibros)
                .ThenInclude(buscandoAutor => buscandoAutor.Autor)
                .FirstOrDefaultAsync(buscandoLibro => buscandoLibro.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return mapper.Map<LibroDTOConAutores>(libro);
        }
        [HttpPost(Name = "crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }
            var autores = await context.Autores
                .Where(buscandoAutor => libroCreacionDTO.AutoresIds.Contains(buscandoAutor.Id))
                .Select(autorSeleccionado => autorSeleccionado.Id).ToListAsync();
            if (libroCreacionDTO.AutoresIds.Count != autores.Count)
            {
                return BadRequest("No existe uno de los autores");
            }
            var libro = mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);
            context.Add(libro);
            await context.SaveChangesAsync();
            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
        }
        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                .Include(buscandoLibro => buscandoLibro.AutoresLibros)
                .FirstOrDefaultAsync(buscandoLibro => buscandoLibro.Id == id);
            if (libroDB == null)
            {
                return NotFound();
            }
            libroDB = mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
        [HttpPatch("{id:int}", Name = "patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var libroDB = await context.Libros.FirstOrDefaultAsync(buscandoLibro => buscandoLibro.Id == id);
            if (libroDB == null)
            {
                return NotFound();
            }
            var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);
            patchDocument.ApplyTo(libroDTO, ModelState);
            var esValido = TryValidateModel(libroDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(libroDTO, libroDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}", Name = "borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(buscandoLibro => buscandoLibro.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
