using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(
            ApplicationDbContext context,
            IMapper mapper
            )
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AutorDTO>>>  Get()
        {
            //return await context.Autores.Include(x => x.Libros).ToListAsync();
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);   
        }


        [HttpGet("{nombre}")]
         public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorDb => autorDb.Nombre.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);    
        }

        //DESPUES DE USAR EHRENCIA

        [HttpGet("{id:int}", Name = "obtenerPorAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorDB => autorDB.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDTOConLibros>(autor);


        }
        //antes de herencia
        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<AutorDTO>> Get(int id)
        //{
        //    var autor = await context.Autores
        //        .Include(autorDB => autorDB.AutoresLibros)
        //        .ThenInclude(autorLibroDB => autorLibroDB.Libro)
        //        .FirstOrDefaultAsync(autorDB => autorDB.Id == id); 
        //    if(autor == null)
        //    {
        //        return NotFound();
        //    }
        //    return mapper.Map<AutorDTO>(autor);


        //}

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>( autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            //return Ok();

            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerPorAutor", new {id = autor.Id}, autorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            //if (autor.Id != id)
            //{
            //    return BadRequest("El id del autor no coincide con el id de la URL");
            //}
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            //return Ok();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x =>x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
