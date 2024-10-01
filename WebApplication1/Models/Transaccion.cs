using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        public int UsuarioId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la transacción")]
        [Required(ErrorMessage = "La fecha de la transacción es obligatoria.")]
        public DateTime FechaTransaccion { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]       
        public decimal Monto { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int CategoriaId { get; set; }

        public string CategoriaNombre { get; set; }

        [StringLength(500, ErrorMessage = "La nota no puede exceder los 500 caracteres.")]
        public string Nota { get; set; }

        [Required(ErrorMessage = "El ID de la cuenta es obligatorio.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Debe seleccionar una cuenta válida.")]
        public int CuentasId { get; set; }  // Ajustado para coincidir con la tabla

        public string CuentaNombre { get; set; }  // Propiedad adicional para mostrar el nombre de la cuenta

        [Required(ErrorMessage = "El tipo de operación es obligatorio.")]
        public TipoOperacion TipoOperacionesId { get; set; }  // Cambiado a enum
        public string DigVerificador { get; set; } // Campo calculado automáticamente

        public Transaccion() { }
    }
}
