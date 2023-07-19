using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class LibroCreacionDTO
    {
      
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Titulo { get; set; }
    }
}
