using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.App_Services;
using WebApplication1.Models;

public class TransaccionController : Controller
{
    private readonly IRepositorioTransacciones _repositorioTransacciones;
    private readonly IServicioUsuarios _servicioUsuarios;
    private readonly IRepositorioCategorias _repositorioCategorias;
    private readonly IRepositorioTiposCuentas _repositorioCuentas;

    public TransaccionController(IRepositorioTransacciones repositorioTransacciones,
                                 IServicioUsuarios servicioUsuarios,
                                 IRepositorioCategorias repositorioCategorias,
                                 IRepositorioTiposCuentas repositorioTiposCuentas)
    {
        _repositorioTransacciones = repositorioTransacciones;
        _servicioUsuarios = servicioUsuarios;
        _repositorioCategorias = repositorioCategorias;
        _repositorioCuentas = repositorioTiposCuentas;
    }

    public TransaccionController()
    {
        _repositorioTransacciones = new RepositorioTransacciones();
        _servicioUsuarios = new ServicioUsuarios();
        _repositorioCategorias = new RepositorioCategorias();
        _repositorioCuentas = new RepositorioTiposCuentas();
    }

    // GET: Mostrar el formulario de creación de una nueva transacción
    [HttpGet]
    public async Task<ActionResult> CrearTransaccion()
    {
        var model = new Transaccion();
        // Obtener el UsuarioId del servicio de usuarios
        int UsuarioId = _servicioUsuarios.ObtenerUsuarioId();

        // Cargar las categorías y cuentas desde el repositorio o base de datos
        ViewBag.Categorias = await _repositorioCategorias.ObtenerTodasCategoriasAsync(); // Obtén las categorías
        ViewBag.Cuentas = await _repositorioCuentas.ObtenerTiposCuentasAsync(UsuarioId);
        return View(model);
    }

    // POST: Procesar el formulario de creación de una nueva transacción
    [HttpPost]
    public async Task<ActionResult> CrearTransaccion(Transaccion model)
    {
        if (ModelState.IsValid)
        {
            // Obtener el UsuarioId del servicio de usuarios
            model.UsuarioId = _servicioUsuarios.ObtenerUsuarioId();

            // Crear la nueva transacción
            await _repositorioTransacciones.CrearTransaccionAsync(model);

            // Redirige a la lista de transacciones
            return RedirectToAction("ListarTransacciones");
        }

        return View(model);  // Si hay errores, volver a mostrar el formulario con los errores
    }



    [HttpGet]
    public async Task<JsonResult> ObtenerCategoriasPorTipoOperacion(int tipoOperacionId)
    {
        var categorias = await _repositorioCategorias.ObtenerCategoriasPorTipoOperacionAsync(tipoOperacionId);
        return Json(categorias, JsonRequestBehavior.AllowGet);
    }


    // GET: Listar Transacciones
    [HttpGet]
    public async Task<ActionResult> ListarTransacciones()
    {
        // Obtener todas las transacciones usando el repositorio
        var transacciones = await _repositorioTransacciones.ObtenerTodasTransaccionesAsync();

        // Asegúrate de que las transacciones se devuelvan correctamente a la vista
        return View(transacciones);
    }

    [HttpGet]
    public async Task<ActionResult> EditarTransaccion(int id)
    {
        var transaccion = await _repositorioTransacciones.ObtenerTransaccionPorIdAsync(id);

        if (transaccion == null)
        {
            return HttpNotFound();
        }

        // Convertir el enum TipoOperacion en una lista de SelectListItem
        var tipoOperaciones = Enum.GetValues(typeof(TipoOperacion))
                                  .Cast<TipoOperacion>()
                                  .Select(e => new SelectListItem
                                  {
                                      Value = ((int)e).ToString(),
                                      Text = e.ToString()
                                  })
                                  .ToList();

        // Pasar la lista al ViewBag
        ViewBag.TipoOperaciones = tipoOperaciones;

        // Cargar categorías y cuentas
        ViewBag.Categorias = await _repositorioCategorias.ObtenerTodasCategoriasAsync();
        ViewBag.Cuentas = await _repositorioCuentas.ObtenerTiposCuentasAsync(transaccion.UsuarioId);

        return View(transaccion);
    }




    [HttpPost]
    public async Task<ActionResult> EditarTransaccion(Transaccion model)
    {
        if (ModelState.IsValid)
        {
            // Obtener la transacción anterior
            var transaccionAnterior = await _repositorioTransacciones.ObtenerTransaccionPorIdAsync(model.Id);

            // Obtener el UsuarioId del servicio de usuarios
            model.UsuarioId = _servicioUsuarios.ObtenerUsuarioId();

            if (transaccionAnterior == null)
            {
                return HttpNotFound();
            }

            // Actualizar la transacción
            await _repositorioTransacciones.ActualizarTransaccionAsync(model);

            // Redirige a la lista de transacciones
            return RedirectToAction("ListarTransacciones");
        }

        // Si hay errores en el modelo, vuelve a cargar las categorías y cuentas
        ViewBag.Categorias = await _repositorioCategorias.ObtenerTodasCategoriasAsync();
        ViewBag.Cuentas = await _repositorioCuentas.ObtenerTiposCuentasAsync(model.UsuarioId);
        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> EliminarTransaccion(int id)
    {
        // Llamar al método del repositorio para eliminar la transacción
        await _repositorioTransacciones.EliminarTransaccionAsync(id);

        // Redirigir a la lista de transacciones
        return RedirectToAction("ListarTransacciones");
    }




}


