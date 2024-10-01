using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;

public class BitacoraController : Controller
{
    private readonly BitacoraRepositorio _repositorio = new BitacoraRepositorio();


public async Task<ActionResult> ListarBitacora(string sortOrder, int? page)
{
    var bitacora = await _repositorio.ObtenerBitacoraAsync();

    // Manejar la ordenación
    switch (sortOrder)
    {
        case "fecha_desc":
            bitacora = bitacora.OrderByDescending(b => b.Fecha).ToList();
            break;
        case "usuario_asc":
            bitacora = bitacora.OrderBy(b => b.UsuarioID).ToList();
            break;
        case "usuario_desc":
            bitacora = bitacora.OrderByDescending(b => b.UsuarioID).ToList();
            break;
        case "tipo_asc":
            bitacora = bitacora.OrderBy(b => b.TipoOperacion).ToList();
            break;
        case "tipo_desc":
            bitacora = bitacora.OrderByDescending(b => b.TipoOperacion).ToList();
            break;
        case "tabla_asc":
            bitacora = bitacora.OrderBy(b => b.TablaAfectada).ToList();
            break;
        case "tabla_desc":
            bitacora = bitacora.OrderByDescending(b => b.TablaAfectada).ToList();
            break;
        case "registro_asc":
            bitacora = bitacora.OrderBy(b => b.RegistroID).ToList();
            break;
        case "registro_desc":
            bitacora = bitacora.OrderByDescending(b => b.RegistroID).ToList();
            break;
        default:
            bitacora = bitacora.OrderBy(b => b.Fecha).ToList();  // Orden por defecto
            break;
    }

    ViewBag.CurrentSort = sortOrder;

    // Configurar la paginación
    int pageSize = 10; // Número de registros por página
    int pageNumber = (page ?? 1);

    return View(bitacora.ToPagedList(pageNumber, pageSize));
}

  
}
