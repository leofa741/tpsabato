using System;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Data;
using System.Collections.Generic;

namespace WebApplication1.App_Services
{
    public interface IRepositorioTransacciones
    {
        Task<int> CrearTransaccionAsync(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerTodasTransaccionesAsync();

        Task<Transaccion> ObtenerTransaccionPorIdAsync(int id);
        Task ActualizarTransaccionAsync(Transaccion transaccion);

        Task<IEnumerable<TipoOperacion>> ObtenerTodosTiposOperacionesAsync();

        Task EliminarTransaccionAsync(int id);


    }


    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string _connectionString;

        public RepositorioTransacciones()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerTodasTransaccionesAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Aquí se llama al procedimiento almacenado
                string sql = "usp_ObtenerTransaccionesConCategoria";

                // Llamar al procedimiento almacenado
                var transacciones = await db.QueryAsync<Transaccion>(sql, commandType: CommandType.StoredProcedure);

                return transacciones;
            }
        }




        // Método para crear una transacción
        public async Task<int> CrearTransaccionAsync(Transaccion transaccion)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    UsuarioId = transaccion.UsuarioId,
                    FechaTransaccion = transaccion.FechaTransaccion,
                    Monto = transaccion.Monto,
                    TipoOperacionesId = transaccion.TipoOperacionesId, // Asegúrate de que esté en el modelo
                    Nota = transaccion.Nota,
                    CuentasId = transaccion.CuentasId,
                    CategoriaId = transaccion.CategoriaId
                };

                // Ejecuta el procedimiento almacenado para crear la transacción
                string sql = "usp_CrearTransaccion";
                var id = await db.QuerySingleAsync<int>(sql, parameters, commandType: CommandType.StoredProcedure);

                return id;  // Devuelve el ID de la transacción recién creada
            }
        }


        public async Task<Transaccion> ObtenerTransaccionPorIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Transacciones WHERE Id = @Id";
                return await db.QuerySingleOrDefaultAsync<Transaccion>(sql, new { Id = id });
            }
        }

        public async Task ActualizarTransaccionAsync(Transaccion transaccion)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Definir los parámetros que se van a pasar al procedimiento almacenado
                var parameters = new
                {
                    transaccion.Id,
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    TipoOperacionesId = (int)transaccion.TipoOperacionesId,  // Convertir el enum a int
                    transaccion.Nota,
                    transaccion.CuentasId,
                    transaccion.CategoriaId
                };

                // Llamar al procedimiento almacenado
                string sql = "sp_ActualizarTransaccion";
                await db.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<TipoOperacion>> ObtenerTodosTiposOperacionesAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Descripcion FROM TipoOperaciones";
                return await db.QueryAsync<TipoOperacion>(sql);
            }
        }

        public async Task EliminarTransaccionAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "sp_EliminarTransaccion";
                await db.ExecuteAsync(sql, new { Id = id }, commandType: CommandType.StoredProcedure);
            }
        }





    }
}
