using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Categoria
    {

        public int Id { get; set; }

        [Required( ErrorMessage ="el campo {0} es requerido")]
        [StringLength(maximumLength:50,ErrorMessage ="no puede ser mayor de {1} caracteres")]
        public string Nombre { get; set; }

        public TipoOperacion TipoOperacionId { get; set; }

        public int UsuarioId { get; set; }
    }
}