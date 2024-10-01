using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.App_Services;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CuentasController : Controller
    {

        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly string _connectionString;
        private readonly ServicioCuentas _servicioCuentas;

        public CuentasController(RepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;
            _servicioCuentas = new ServicioCuentas(_connectionString);
            _servicioUsuarios = servicioUsuarios;
        }


        public CuentasController()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;
            _servicioCuentas = new ServicioCuentas(_connectionString);
            _servicioUsuarios = new ServicioUsuarios();
        }

        // Acción para mostrar una lista de cuentas



        public async Task<ActionResult> Cuentas()
        {
            try
            {
                var modelo = new CuentaCreacionViewModel
                {
                    CuentasExistentes = await _servicioCuentas.ObtenerCuentasConTipoAsync(),
                    TiposCuentas = await ObtenerTiposCuentasSelectListAsync()
                };

                return View(modelo);
            }
            catch (SqlException ex) // Manejo de excepciones relacionadas con la base de datos
            {
                // Puedes personalizar el mensaje de error para el usuario
                ViewBag.DatabaseError = "No se pudo conectar a la base de datos. Por favor, intentelo mas tarde.";

                // Devolver una vista con un modelo vacío si la base de datos falla
                return View(new CuentaCreacionViewModel());
            }
            catch (Exception ex) // Manejo general de otras excepciones
            {
                ViewBag.DatabaseError = "Ocurrio un error inesperado. Por favor, intentelo mas tarde.";
                return View(new CuentaCreacionViewModel());
            }
        }






        // Mostrar formulario de creación de cuenta
        [HttpGet]
        public async Task<ActionResult> CrearCuentas()
        {
            var viewModel = new CuentaCreacionViewModel
            {
                TiposCuentas = await ObtenerTiposCuentasSelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CrearCuentas(CuentaCreacionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
                    decimal balance;

                    // Verifica si el valor se puede convertir a decimal
                    if (decimal.TryParse(model.Balance.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out balance))
                    {
                        var cuenta = new Cuentas
                        {
                            Nombre = model.Nombre,
                            TipoCuentaId = model.TipoCuentaId,
                            Balance = balance.ToString(CultureInfo.InvariantCulture),
                            Descripcion = model.Descripcion
                        };

                        // Llama al servicio para crear la cuenta y pasa el UsuarioID
                        await _servicioCuentas.CrearCuentaAsync(cuenta, usuarioId);

                        // Establece un mensaje en el ViewBag
                        ViewBag.Message = "La cuenta se ha creado exitosamente.";

                        return RedirectToAction("Cuentas", "Cuentas");
                    }
                    else
                    {
                        ModelState.AddModelError("Balance", "El valor ingresado no es válido.");
                    }
                }

                model.TiposCuentas = await ObtenerTiposCuentasSelectListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado.";
                model.TiposCuentas = new List<SelectListItem>();
                return View(model);
            }
        }


        // Método auxiliar para cargar la lista de tipos de cuentas en formato SelectList
        private async Task<SelectList> ObtenerTiposCuentasSelectListAsync()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            // Llamada al servicio para obtener los tipos de cuentas
            var tiposDeCuenta = await _servicioCuentas.ObtenerTiposDeCuentaAsync(usuarioId);

            // Si es null, se inicializa como una lista vacía
            if (tiposDeCuenta == null)
            {
                tiposDeCuenta = new List<SelectListItem>();
            }

            // Devolver la lista de SelectListItem
            return new SelectList(tiposDeCuenta, "Value", "Text");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarCuenta(int id)
        {
            if (id > 0)
            {
                await _servicioCuentas.EliminarCuentaAsync(id);
                ViewBag.Message = "La cuenta se ha eliminado exitosamente.";
            }
            else
            {
                ViewBag.Message = "Error: La cuenta no pudo ser eliminada.";
            }

            // Redirigir a la vista de cuentas después de eliminar
            return RedirectToAction("Cuentas");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarCuentas(int id)
        {
            await _servicioCuentas.EliminarCuentaAsync(id);
            return RedirectToAction("Cuentas");
        }


   

        [HttpGet]
        public async Task<ActionResult> EditarCuenta(int id)
        {
            var cuenta = await _servicioCuentas.ObtenerCuentaPorIdAsync(id);
            if (cuenta == null)
            {
                return HttpNotFound();
            }

            var model = new CuentaCreacionViewModel
            {
                Id = cuenta.Id,
                Nombre = cuenta.Nombre,
                TipoCuentaId = cuenta.TipoCuentaId,
                Balance = cuenta.Balance,
                Descripcion = cuenta.Descripcion,
                TiposCuentas = await ObtenerTiposCuentasSelectListAsync() // Recarga los tipos de cuentas para el dropdown
            };

            return View("EditarCuenta", model);  // Asegúrate de pasar el nombre correcto de la vista aquí
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarCuenta(CuentaCreacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cuenta = new Cuentas
                {
                    Id = model.Id,
                    Nombre = model.Nombre,
                    TipoCuentaId = model.TipoCuentaId,
                    Balance = model.Balance,
                    Descripcion = model.Descripcion
                };

                await _servicioCuentas.ActualizarCuentaAsync(cuenta);

                // Redirige a la lista de cuentas
                return RedirectToAction("Cuentas");
            }

            // Si hay errores, recarga los tipos de cuentas
            model.TiposCuentas = await ObtenerTiposCuentasSelectListAsync();
            return View("EditarCuenta", model);
        }



    }
}