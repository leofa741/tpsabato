using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.App_Services;
using WebApplication1.Models;

public class CategoriaController : Controller
{
    private readonly IRepositorioCategorias _repositorioCategorias;
    private readonly IServicioUsuarios _servicioUsuarios;

    public CategoriaController(IRepositorioCategorias repositorioCategorias, IServicioUsuarios servicioUsuarios)
    {
        _repositorioCategorias = repositorioCategorias;
        _servicioUsuarios = servicioUsuarios;
    }

    public CategoriaController()
    {

        _repositorioCategorias = new RepositorioCategorias();
        _servicioUsuarios = new ServicioUsuarios();
    }

    // GET: Mostrar el formulario de creación de nueva categoría
    [HttpGet]
    public ActionResult CrearCategoria()
    {
        var model = new Categoria();  // Modelo vacío para la vista
        return View(model);
    }

    // POST: Procesa el formulario de creación de nueva categoría
    [HttpPost]
    public async Task<ActionResult> CrearCategoria(Categoria model)
    {
        if (ModelState.IsValid)
        {
            // Obtener el UsuarioId desde el servicio de usuarios
            model.UsuarioId = _servicioUsuarios.ObtenerUsuarioId();

            // Crear la nueva categoría
            await _repositorioCategorias.CrearCategoriaAsync(model);

            // Redirige a la lista de categorías (o la misma página si prefieres)
            return RedirectToAction("ListarCategorias");
        }

        return View(model);  // Si hay un error en el modelo, vuelve a mostrar el formulario
    }

    // GET: Lista de Categorías
    public async Task<ActionResult> ListarCategorias()
    {
        var categorias = await _repositorioCategorias.ObtenerTodasCategoriasAsync();
        return View(categorias); // Asegúrate de pasar una colección de categorías
    }

    // Método para editar categorías
    [HttpGet]
    public async Task<ActionResult> EditarCategoria(int id)
    {
        var categoria = await _repositorioCategorias.ObtenerCategoriaPorIdAsync(id);
        if (categoria == null)
        {
            return HttpNotFound();
        }
        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditarCategoria(Categoria categoria)
    {
        if (ModelState.IsValid) 
        {
            // Obtener el UsuarioId desde el servicio de usuarios
            categoria.UsuarioId = _servicioUsuarios.ObtenerUsuarioId();
            await _repositorioCategorias.ActualizarCategoriaAsync(categoria);
            return RedirectToAction("ListarCategorias");
        }
        return View(categoria);
    }

    // Método para eliminar categorías
    [HttpPost]
    public async Task<ActionResult> EliminarCategoria(int id)
    {
        await _repositorioCategorias.EliminarCategoriaAsync(id);
        return RedirectToAction("ListarCategorias");
    }


}
