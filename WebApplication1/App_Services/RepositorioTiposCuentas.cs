using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.App_Services
{
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {


        private readonly string _connectionString;

        public object TiposCuentas { get; internal set; }

        public RepositorioTiposCuentas()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;
        }
        public async Task<IEnumerable<TiposCuentas>> ObtenerTiposCuentasAsync(int usuarioId)
        {
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    await conexion.OpenAsync(); // Abrir conexión de forma asíncrona

                    // Consulta SQL con filtro por UsuarioId y orden por el campo Orden
                    string sql = "SELECT Id, Nombre, UsuarioId, Orden FROM TiposCuentas WHERE UsuarioId = @UsuarioId ORDER BY Orden ASC";

                    // Ejecutar la consulta pasando el UsuarioId como parámetro
                    return await conexion.QueryAsync<TiposCuentas>(sql, new { UsuarioId = usuarioId });
                }
            }
            catch (SqlException ex)
            {
                // Captura el error de conexión a la base de datos
                throw new Exception("No se pudo conectar a la base de datos. Por favor, asegurese de que el servidor de base de datos esta disponible.", ex);
            }
        }




        public async Task InsertarTipoCuentaAsync(TiposCuentas tipoCuenta)
        {
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    await conexion.OpenAsync();

                    // Verificar si ya existe un registro con el mismo Nombre para el mismo UsuarioId
                    string sqlCheck = "SELECT COUNT(1) FROM TiposCuentas WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId";
                    int count = await conexion.ExecuteScalarAsync<int>(sqlCheck, new { Nombre = tipoCuenta.Nombre, UsuarioId = tipoCuenta.UsuarioId });

                    if (count > 0)
                    {
                        // Si ya existe una cuenta con el mismo nombre para el usuario, lanzar una excepción
                        throw new Exception("Ya tienes una cuenta con el mismo nombre.");
                    }

                    // Si no existe, proceder a insertar
                    var parametros = new
                    {
                        Nombre = tipoCuenta.Nombre,
                        UsuarioId = tipoCuenta.UsuarioId,
                        Orden = tipoCuenta.Orden
                    };

                    await conexion.ExecuteAsync("InsertarTipoCuenta", parametros, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                // Manejo de error de conexión a la base de datos
                throw new Exception("No se pudo conectar a la base de datos. Por favor, asegurese de que el servidor de base de datos esta disponible.");
            }
        }



        public async Task EliminarTipoCuentaAsync(int id, int usuarioId)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                await conexion.OpenAsync();

                // Verificar si existe la cuenta para el usuario
                string sqlCheck = "SELECT COUNT(1) FROM TiposCuentas WHERE Id = @Id AND UsuarioId = @UsuarioId";
                int count = await conexion.ExecuteScalarAsync<int>(sqlCheck, new { Id = id, UsuarioId = usuarioId });

                if (count == 0)
                {
                    throw new Exception("La cuenta no existe o no pertenece a este usuario.");
                }

                // Eliminar la cuenta si existe
                string sqlDelete = "DELETE FROM TiposCuentas WHERE Id = @Id AND UsuarioId = @UsuarioId";
                await conexion.ExecuteAsync(sqlDelete, new { Id = id, UsuarioId = usuarioId });
            }
        }

        internal async Task<bool> VerificarSiExisteCuentaAsync(string nombre, int usuarioId)
        {
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    await conexion.OpenAsync(); // Abre la conexión a la base de datos de forma asíncrona

                    // Consulta SQL que verifica si existe una cuenta con el nombre y usuarioId proporcionados
                    string sqlCheck = "SELECT COUNT(1) FROM TiposCuentas WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId";

                    // Ejecuta la consulta y obtiene el resultado
                    int count = await conexion.ExecuteScalarAsync<int>(sqlCheck, new { Nombre = nombre, UsuarioId = usuarioId });

                    // Retorna true si existe al menos una cuenta que coincide con los parámetros
                    return count > 0;
                }
            }
            catch (SqlException sqlEx)
            {
                // Registro o manejo de errores SQL (puedes personalizar cómo manejar estos errores)
                throw new Exception($"Error de SQL al verificar la cuenta: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Registro o manejo de otros errores
                throw new Exception($"Error general al verificar la cuenta: {ex.Message}");
            }
        }


        public async Task ActualizarTipoCuentaAsync(int id, string nombre, int orden)
        {
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    await conexion.OpenAsync();

                    // Verificar si la cuenta existe antes de intentar actualizar
                    string sqlCheck = "SELECT COUNT(1) FROM TiposCuentas WHERE Id = @Id";
                    int count = await conexion.ExecuteScalarAsync<int>(sqlCheck, new { Id = id });

                    if (count == 0)
                    {
                        throw new Exception("La cuenta que intentas actualizar no existe.");
                    }

                    // Parámetros para el procedimiento almacenado
                    var parametros = new
                    {
                        Id = id,
                        Nombre = nombre,
                        Orden = orden
                    };

                    // Ejecutar el procedimiento almacenado para actualizar la cuenta
                    await conexion.ExecuteAsync("ActualizarTipoCuenta", parametros, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                // Manejo de error de conexión a la base de datos
                throw new Exception("No se pudo conectar a la base de datos. Por favor, asegurese de que el servidor de base de datos esta disponible.", ex);
            }
        }

        internal async Task<WebApplication1.Models.TiposCuentas> ObtenerTipoCuentaPorIdAsync(int id)
        {
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    await conexion.OpenAsync();

                    // Definir el nombre del procedimiento almacenado
                    string procedimientoAlmacenado = "ObtenerTipoCuentaPorId";

                    // Definir los parámetros del procedimiento almacenado
                    var parametros = new { Id = id };

                    // Ejecutar el procedimiento almacenado y obtener el resultado
                    var tipoCuenta = await conexion.QueryFirstOrDefaultAsync<WebApplication1.Models.TiposCuentas>(
                        procedimientoAlmacenado,
                        parametros,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    return tipoCuenta;
                }
            }
   
            catch (SqlException ex)
            {
                // Manejo de error de conexión a la base de datos
                if (_connectionString == null)
                {
                    throw new Exception("La cadena de conexión es nula.");
                }
                else
                {
                    

                    throw new Exception("No se pudo conectar a la base de datos.", ex);
                }
            }
                              
            
        }

        internal void ActualizarOrden(TiposCuentas tipoCuenta)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();

                // Definir el comando SQL para actualizar el orden
                string sql = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id";

                // Ejecutar el comando SQL con los parámetros correspondientes
                conexion.Execute(sql, new { Orden = tipoCuenta.Orden, Id = tipoCuenta.Id });
            }
        }


    }

}


