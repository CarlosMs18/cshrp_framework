using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(
            ApplicationDbContext context,
            IMapper mapper
            )
        {
            this.context = context;
            this.mapper = mapper;
        }

        //con herencia
        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            //return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync( x => x.Id == id);   
            //var libro =  await context.Libros
            //   .Include(libroDB => libroDB.Comentarios)
            //   .FirstOrDefaultAsync(x => x.Id == id);    
            var libro = await context.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor) //entra a libroDB.AUTORESLIBROS E INCLUYE ALGO QUE SE EMNCUENTRA AHI
                .FirstOrDefaultAsync(x => x.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return mapper.Map<LibroDTOConAutores>(libro);
        }

        //[HttpGet("{id:int}")]   //antes de la herencia
        //public async Task<ActionResult<LibroDTO>> Get(int id)
        //{
        //    //return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync( x => x.Id == id);   
        //     //var libro =  await context.Libros
        //     //   .Include(libroDB => libroDB.Comentarios)
        //     //   .FirstOrDefaultAsync(x => x.Id == id);    
        //    var libro = await context.Libros
        //        .Include(libroDB => libroDB.AutoresLibros)
        //        .ThenInclude(autorLibroDB => autorLibroDB.Autor) //entra a libroDB.AUTORESLIBROS E INCLUYE ALGO QUE SE EMNCUENTRA AHI
        //        .FirstOrDefaultAsync(x => x.Id == id);

        //    libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();
        //    return mapper.Map<LibroDTO>(libro);
        //}

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            //var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            //if(!existeAutor) {
            //    return BadRequest($"No se ha encontrado el autor con el id {libro.AutorId}");
            //}

            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }


            var autoresIds = await context.Autores.Where(autorBD =>
            libroCreacionDTO.AutoresIds.Contains(autorBD.Id))
             .Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);

            AsignarOrdenAutores(libro);
            //if (libro.AutoresLibros != null)
            //{
            //    for (int i = 0; i < libro.AutoresLibros.Count; i++) {
            //        libro.AutoresLibros[i].Orden = i;
            //    }
            //}

            context.Add(libro);
            await context.SaveChangesAsync();
            //return Ok();

            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{int:id}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                            .Include(x => x.AutoresLibros)
                            .FirstOrDefaultAsync(x => x.Id == id);

            if(libroDB == null) {
                return NotFound();
            }
            libroDB  = mapper.Map(libroCreacionDTO, libroDB);//significa vamos a usar automapper para llevar las propiedades de
                                                             //libroCreacionDTO hacia libroDB y  por lo tanto se hara una actualizacion 
                                                             //de librodb sin embargo estamos asignando el resultado de la operacion a librodb
                                                             //lo que me permite mantenr la misma instancia que lña que fue creada lineas mas arriba
                                                             //eso me permite hacer un actualizacion de la entidad libro sy autoreslibros sin tanta magia
                                                             //pq cuando instancias el libro de arriba no es un simple libro ENTITYFRAMEWORK, matniene un registrode las entirdades
                                                             //cargadadas en memtoria, entonces este libro esta guardado en la memoria sabe si la hacemos un cambio
                                                             //a este libro deberiamosd e tener la capacidad de persistir esos cambios en la BBDD
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
    }
}
