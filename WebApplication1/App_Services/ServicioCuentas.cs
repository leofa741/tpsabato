using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.App_Services
{
    public class ServicioCuentas
    {
        private readonly string _connectionString;

        public ServicioCuentas(string connectionString)
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;
        }


        public async Task<IEnumerable<CuentasConTipo>> ObtenerCuentasConTipoAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string procedimientoAlmacenado = "ObtenerCuentasConTipo";
                return await db.QueryAsync<CuentasConTipo>(procedimientoAlmacenado, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task<IEnumerable<Cuentas>> ObtenerCuentasAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string procedimientoAlmacenado = "ObtenerCuentas"; // Asegúrate de que el procedimiento almacene la columna TipoCuenta
                return await db.QueryAsync<Cuentas>(procedimientoAlmacenado, commandType: CommandType.StoredProcedure);
            }
        }




        // Obtener tipos de cuentas
        public async Task<IEnumerable<TiposCuentas>> ObtenerTiposCuentasAsync(int usuarioId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "usp_ObtenerTiposCuentas"; // Procedimiento almacenado para tipos de cuentas
                return await db.QueryAsync<TiposCuentas>(sql, new { UsuarioId = usuarioId }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task CrearCuentaAsync(Cuentas cuenta, int usuarioId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "usp_CrearCuenta"; // Procedimiento almacenado para crear una cuenta
                var parameters = new
                {
                    cuenta.Nombre,
                    cuenta.TipoCuentaId,
                    cuenta.Balance,
                    cuenta.Descripcion,
                    UsuarioID = usuarioId // Pasar el UsuarioID
                };

                await db.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task ActualizarCuentaAsync(Cuentas cuenta)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "usp_ActualizarCuenta"; // Procedimiento almacenado para actualizar la cuenta
                var parameters = new
                {
                    cuenta.Id,
                    cuenta.Nombre,
                    cuenta.TipoCuentaId,
                    cuenta.Balance,
                    cuenta.Descripcion
                };

                await db.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task EliminarCuentaAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string procedimientoAlmacenado = "EliminarCuenta";
                await db.ExecuteAsync(procedimientoAlmacenado, new { Id = id }, commandType: CommandType.StoredProcedure);
            }
        }



        public async Task<Cuentas> ObtenerCuentaPorIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "usp_ObtenerCuentaPorId"; // Procedimiento almacenado para obtener una cuenta por ID
                var parameters = new { Id = id };

                // Ejecuta la consulta y obtiene la cuenta
                return await db.QueryFirstOrDefaultAsync<Cuentas>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task<IEnumerable<SelectListItem>> ObtenerTiposDeCuentaAsync(int usuarioId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "usp_ObtenerTiposCuentas"; // Procedimiento almacenado que retorna los tipos de cuentas

                var tiposCuentas = await db.QueryAsync<TiposCuentas>(sql, new { UsuarioId = usuarioId }, commandType: CommandType.StoredProcedure);

                // Convertir los resultados en SelectListItem
                return tiposCuentas.Select(tc => new SelectListItem
                {
                    Value = tc.Id.ToString(),
                    Text = tc.Nombre
                }).ToList();
            }
        }




    }


}






