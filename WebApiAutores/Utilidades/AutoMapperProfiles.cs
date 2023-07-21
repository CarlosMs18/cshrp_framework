using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
       
            CreateMap<Autor, AutorDTO>();

            //despues de la herencia
            CreateMap<Autor, AutorDTOConLibros>()
             .ForMember(autorDto => autorDto.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));
            //    ;
            //CreateMap<Autor, AutorDTO>()  //ANTES DE HERENCIA PARA EVITAR COLOCAR NULL
            //    .ForMember(autorDto => autorDto.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros))
            //    ;

            //CreateMap<Autor, AutorDTO>()
            //    .ForMember(autorDto => autorDto, opciones => opciones.MapFrom(MapAutorDTOLibros));

            CreateMap<LibroCreacionDTO, Libro>() //regla especifica para el mapeo de int a uno de muchos a  muchos
                .ForMember(libro => libro.AutoresLibros, //configuirando el mapeo hacia propiedad autores libro
                opciones => opciones.MapFrom(MapAutoresLibros)); //mapear de creamos una propiedad privada para la logica desde un entero hacia autores-libro


            CreateMap<Libro, LibroDTO>();

            //despues de la herencia
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(LibroDTO => LibroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));

            //CreateMap<Libro, LibroDTO>() //antes de la herencia
            //    .ForMember(LibroDTO => LibroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));

            CreateMap<ComentarioCreacionDto, Comentario>();
            CreateMap<Comentario, ComentarioDTO>(); 
            
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();
            if(autor.AutoresLibros == null)
            {
                return resultado;
            }
            foreach(var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }
            return resultado;
        }


        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();
            if (libro.AutoresLibros == null)
            {
                return resultado;
            }

            foreach(var autorlibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorlibro.AutorId,
                    Nombre = autorlibro.Autor.Nombre
                });
            }

                return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>(); //retornar el resultado de autorLibro  

            if(libroCreacionDTO.AutoresIds == null) //si se esta creando un libro sin autores retornamos el resultado tal cual
            {                                   //ya seria problema de una regla de validacion del controller
                return resultado;
            }

            foreach(var autorId in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }

            return resultado;
        }
    }
}
