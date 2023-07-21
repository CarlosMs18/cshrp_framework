using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Libro
    {

        public int Id { get; set; }
        [Required]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 255)]
        public string Titulo { get; set; }
        //public int AutorId { get; set; }
        //public Autor Autor { get; set; }

        public List<Comentario> Comentarios { get; set; }   

        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
 