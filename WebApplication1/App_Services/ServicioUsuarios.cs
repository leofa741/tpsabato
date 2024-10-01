using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.App_Services
{

    public interface IServicioUsuarios
    {

        int ObtenerUsuarioId();


    }



    public class ServicioUsuarios : IServicioUsuarios
    {

        public int ObtenerUsuarioId()
        {
            // Lógica para obtener el Id del usuario
            return 2;
        }


    }
}