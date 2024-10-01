using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Cuentas
    {


        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la cuenta es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "El nombre solo puede contener letras, números y espacios.")]
        public string Nombre { get; set; }


        [Display(Name = "Tipo de cuenta")]
        public int TipoCuentaId { get; set; }


        [Required(ErrorMessage = "El balance de la cuenta es obligatorio.")]       
        public string Balance { get; set; } // Cambio a string para manejar la entrada del usuario



        [Required(ErrorMessage = "La descripción de la cuenta es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string Descripcion { get; set; }


        // Nueva propiedad para mostrar el nombre del tipo de cuenta
        public string TipoCuenta { get; set; }

    }



    public class CuentasConTipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la cuenta es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "El nombre solo puede contener letras, números y espacios.")]
        public string Nombre { get; set; }

        public int TipoCuentaId { get; set; }
        public string TipoCuentaNombre { get; set; } // Nombre del tipo de cuenta

        [Required(ErrorMessage = "El balance de la cuenta es obligatorio.")]
        public string Balance { get; set; } // Cambio a string para manejar la entrada del usuario

        [Required(ErrorMessage = "La descripción de la cuenta es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string Descripcion { get; set; }
    }

}