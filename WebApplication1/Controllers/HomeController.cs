using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.App_Services;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        private readonly RepositorioTiposCuentas _repositorio;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly string _connectionString;


        public HomeController(RepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            _repositorio = repositorioTiposCuentas;
            _servicioUsuarios = servicioUsuarios;
        }


        public HomeController()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;
            _repositorio = new RepositorioTiposCuentas();
            _servicioUsuarios = new ServicioUsuarios();
        }


        //public HomeController(RepositorioTiposCuentas   repositorioTiposCuentas)
        //{
        //    _repositorio = repositorioTiposCuentas;
        //}


        public ActionResult Index()
        {

            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TiposDeCuentas()
        {
            try
            {
                var usuarioId = _servicioUsuarios.ObtenerUsuarioId();//  Obtiene el ID del usuario actual (dependiendo de tu lógica de autenticación)
                var tiposCuentas = await _repositorio.ObtenerTiposCuentasAsync(usuarioId);
                ViewBag.TiposCuentas = tiposCuentas; // Pasar la lista a la vista

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; // Guardar el mensaje de error para mostrar en la vista
                return View(); // Retornar la vista aunque ocurra un error
            }
        }


        [HttpPost]
        public async Task<ActionResult> TiposDeCuentas(TiposCuentas tiposCuentas)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tiposCuentas.UsuarioId = _servicioUsuarios.ObtenerUsuarioId(); // Obtener el UsuarioId actual del usuario autenticado
                    await _repositorio.InsertarTipoCuentaAsync(tiposCuentas);
                    TempData["SuccessMessage"] = "Tipo de cuenta guardado exitosamente.";
                    return RedirectToAction("TiposDeCuentas");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message; // Guardar el mensaje de error
                    return RedirectToAction("TiposDeCuentas");
                }
            }

            return View(tiposCuentas);
        }


        [HttpPost]
        public async Task<ActionResult> EliminarCuenta(int id)
        {
            try
            {
                int usuarioId = _servicioUsuarios.ObtenerUsuarioId();//   Obtener el UsuarioId actual del usuario autenticado

                await _repositorio.EliminarTipoCuentaAsync(id, usuarioId);
                TempData["SuccessMessage"] = "Cuenta eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("TiposDeCuentas");
        }


        [HttpGet]
        public async Task<JsonResult> VerificarSiExisteCuenta(string nombre)
        {
            var UsuarioId = _servicioUsuarios.ObtenerUsuarioId(); // Suponiendo que obtienes el ID del usuario autenticado.

            try
            {
                // Verifica si existe la cuenta
                var existe = await _repositorio.VerificarSiExisteCuentaAsync(nombre, UsuarioId);

                if (existe)
                {
                    // Devuelve un mensaje de error si la cuenta ya existe
                    return Json($"La cuenta con el nombre '{nombre}' ya existe.", JsonRequestBehavior.AllowGet);
                }

                // Si la cuenta no existe, devuelve true (lo cual indica que la validación es exitosa)
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Maneja el error y devuelve un mensaje genérico
                string mensajeUsuario = "Se ha producido un error al procesar su solicitud. Por favor, intente nuevamente más tarde.";
              
                return Json($"Error al verificar la cuenta: { mensajeUsuario}", JsonRequestBehavior.AllowGet);

            }

        }    


        // Mostrar el formulario de edición para un tipo de cuenta específico
        [HttpGet]
        public async Task<ActionResult> EditarCuenta(int id)
        {
            try
            {
                var tipoCuenta = await _repositorio.ObtenerTipoCuentaPorIdAsync(id);
                if (tipoCuenta == null)
                {
                    TempData["ErrorMessage"] = "El tipo de cuenta no se encontró.";
                    return RedirectToAction("TiposDeCuentas");
                }
                ViewBag.TipoCuentaParaActualizar = tipoCuenta;
                return View("TiposDeCuentas");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("TiposDeCuentas");
            }
        }


        // Actualizar el tipo de cuenta
        [HttpPost]
        public async Task<ActionResult> ActualizarCuenta(int Id, string Nombre, int Orden)
        {
            try
            {
                await _repositorio.ActualizarTipoCuentaAsync(Id, Nombre, Orden);
                TempData["SuccessMessage"] = "Cuenta actualizada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("TiposDeCuentas");
        }


        [HttpPost]
        public async Task<ActionResult> ActualizarOrden()
        {
            try
            {
                // Lee el cuerpo de la solicitud como JSON
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    var json = await reader.ReadToEndAsync();
                    var order = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrdenDTO>>(json);

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        var query = @"
                UPDATE TiposCuentas
                SET Orden = @Orden
                WHERE Id = @Id";

                        // Ejecuta la consulta para cada elemento en la lista de órdenes
                        foreach (var item in order)
                        {
                            await connection.ExecuteAsync(query, new { Id = item.Id, Orden = item.Orden });
                        }
                    }
                }

                // Retorna un mensaje de éxito
                return Json(new { success = true, message = "Orden actualizado correctamente" });
            }
            catch (Exception ex)
            {
                // Retorna un mensaje de error
                return Json(new { success = false, message = "Error al actualizar el orden", error = ex.Message });
            }
        }


      
    }

    public class OrdenDTO
    {
        public int Id { get; set; }
        public int Orden { get; set; }
    }

}