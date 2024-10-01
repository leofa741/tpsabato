using System;

namespace WebApplication1.Models
{
    public class Bitacora
    {
        public int BitacoraID { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioID { get; set; }
        public string TipoOperacion { get; set; }
        public string TablaAfectada { get; set; }
        public int RegistroID { get; set; }
        public string Detalles { get; set; }
        public string DigVerificador { get; set; }
    }
}
