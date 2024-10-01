using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class TiposCuentas
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la cuenta es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "El nombre solo puede contener letras, números y espacios.")]
        [Remote(action: "VerificarSiExisteCuenta", controller: "Home", ErrorMessage = "El nombre de la cuenta ya existe.")]
        public string Nombre { get; set; }



        public int UsuarioId { get; set; }
        public int Orden { get; set; }



    }
}