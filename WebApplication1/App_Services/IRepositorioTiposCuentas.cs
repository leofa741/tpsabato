using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.App_Services
{
   public interface IRepositorioTiposCuentas
           {

        Task<IEnumerable<TiposCuentas>> ObtenerTiposCuentasAsync(int usuarioId);

        Task InsertarTipoCuentaAsync(TiposCuentas tipoCuenta);






    }
}
