using System.Data;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Collections.Generic;

namespace WebApplication1.App_Services
{
    public interface IRepositorioCategorias
    {
        Task<int> CrearCategoriaAsync(Categoria categoria);

        Task<Categoria> ObtenerCategoriaPorIdAsync(int id);
        Task ActualizarCategoriaAsync(Categoria categoria);

        Task EliminarCategoriaAsync(int id);

        Task<IEnumerable<Categoria>> ObtenerTodasCategoriasAsync();

        Task<IEnumerable<Categoria>> ObtenerCategoriasPorTipoOperacionAsync(int tipoOperacionId);


    }

    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string _connectionString;
      

        // Modificar el constructor para inyectar IServicioUsuarios
        public RepositorioCategorias()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;
           
        }


        public async Task<IEnumerable<Categoria>> ObtenerCategoriasPorTipoOperacionAsync(int tipoOperacionId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Nombre, TipoOperacionId FROM Categorias WHERE TipoOperacionId = @TipoOperacionId";
                var categorias = await db.QueryAsync<Categoria>(sql, new { TipoOperacionId = tipoOperacionId });
                return categorias;
            }
        }



        public async Task<int> CrearCategoriaAsync(Categoria categoria)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Obtener el UsuarioId desde el servicio de usuarios
              

                var parameters = new
                {
                    Nombre = categoria.Nombre,
                    TipoOperacionId = (int)categoria.TipoOperacionId,
                    UsuarioId = (int)categoria.UsuarioId // Usar el UsuarioId del servicio
                };

                string sql = "usp_CrearCategoria";  // El nombre del procedimiento almacenado

                // Ejecuta el procedimiento almacenado con Dapper
                var id = await db.QuerySingleAsync<int>(sql, parameters, commandType: CommandType.StoredProcedure);

                return id;  // Devuelve el ID de la nueva categoría insertada
            }
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasCategoriasAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "usp_ObtenerTodasCategorias";  // Procedimiento almacenado
                var categorias = await db.QueryAsync<Categoria>(sql, commandType: CommandType.StoredProcedure);
                return categorias;  // Devuelve la lista de categorías
            }
        }

        public async Task ActualizarCategoriaAsync(Categoria categoria)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    TipoOperacionId = categoria.TipoOperacionId,
                    UsuarioId = categoria.UsuarioId
                };

                await db.ExecuteAsync("usp_ActualizarCategoria", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task EliminarCategoriaAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync("usp_EliminarCategoria", new { Id = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Categoria> ObtenerCategoriaPorIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var parameters = new { Id = id };
                string sql = "usp_ObtenerCategoriaPorId";
                var categoria = await db.QuerySingleOrDefaultAsync<Categoria>(sql, parameters, commandType: CommandType.StoredProcedure);
                return categoria;
            }
        }

    }
}
