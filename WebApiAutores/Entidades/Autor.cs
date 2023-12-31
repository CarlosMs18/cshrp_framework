﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        //[Range(18,120)]
        //[NotMapped]
        //public int Edad { get; set; }

        //public int Menor { get; set; }
        //public int Mayor { get; set; }

        public List<Libro> Libros { get; set; }

        public List<AutorLibro> AutoresLibros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)//validacione s anivel de modelo, para que se ejcute se tiene 
                                                                                    //que ´pasar primero las validaciones por attrivbute

       

        {
            if(!string.IsNullOrEmpty(Nombre)) {
                var primeraLetra = Nombre[0].ToString();
                if(primeraLetra != primeraLetra.ToUpper()) { 
                    yield return new ValidationResult("La primera letra debe de ser mayúscula"
                        , new string[] {nameof(Nombre)}); 
                }
            }

            //if(Menor > Mayor)
            //{
            //    yield return new ValidationResult("Este valor no puede ser mas grande que el campo Mayor",
            //        new string[] { nameof(Menor) });
            //}
        }
    }
}
