using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApplication1.Models;

public class BitacoraRepositorio
{
    private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MiConnectionString"].ConnectionString;

    public async Task<IEnumerable<Bitacora>> ObtenerBitacoraAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<Bitacora>("sp_ObtenerBitacora", commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
