using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class CuentaCreacionViewModel: Cuentas   
    {
      //  public IEnumerable<SelectListItem> TiposCuentas { get; set; }

        

        // Nueva propiedad para las cuentas existentes
       // public IEnumerable<Cuentas> CuentasExistentes { get; set; }

        public IEnumerable<CuentasConTipo> CuentasExistentes { get; set; }
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
        public string Nombre { get; set; }
        public int TipoCuentaId { get; set; }
        public string Balance { get; set; }
        public string Descripcion { get; set; }
    }

}