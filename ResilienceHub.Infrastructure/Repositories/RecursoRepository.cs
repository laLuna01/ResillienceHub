using System.Data;
using Oracle.ManagedDataAccess.Client;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Infrastructure.Repositories;

public class RecursoRepository : IRecursoRepository {
    
    private readonly IDbConnection _connection;

    public RecursoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Recurso>> GetRecursos()
    {
        var recursos = new List<Recurso>();
        
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "SELECT * FROM RECURSO";
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    recursos.Add(MapToRecurso(reader));
                }
            }
        }
        
        return recursos;
    }

    public async Task<Recurso> GetRecurso(int id)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "SELECT * FROM RECURSO WHERE RECURSO_ID = :id";
            cmd.Parameters.Add(new OracleParameter("id", id));
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return MapToRecurso(reader);
                }
            }
        }
        
        return null;
    }

    public async Task<IEnumerable<Recurso>> GetRecursosPorTipo(string tipo)
    {
        var recursos = new List<Recurso>();
        
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "SELECT * FROM RECURSO WHERE TIPO = :tipo";
            cmd.Parameters.Add(new OracleParameter("tipo", tipo));
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    recursos.Add(MapToRecurso(reader));
                }
            }
        }
        
        return recursos;
    }

    public async Task<IEnumerable<Recurso>> GetRecursosVencidos()
    {
        var recursos = new List<Recurso>();
        
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "SELECT * FROM RECURSO WHERE VALIDADE IS NOT NULL AND VALIDADE < SYSDATE";
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    recursos.Add(MapToRecurso(reader));
                }
            }
        }
        
        return recursos;
    }

    public async Task<IEnumerable<Recurso>> GetRecursosBaixoEstoque(int quantidadeMinima = 5)
    {
        var recursos = new List<Recurso>();
        
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "SELECT * FROM RECURSO WHERE QUANTIDADE <= :quantidadeMinima";
            cmd.Parameters.Add(new OracleParameter("quantidadeMinima", quantidadeMinima));
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    recursos.Add(MapToRecurso(reader));
                }
            }
        }
        
        return recursos;
    }

    public async Task<Recurso> CreateRecurso(Recurso recurso)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                INSERT INTO RECURSO (
                    TIPO, DESCRICAO, QUANTIDADE, VALIDADE, UNIDADE_MEDIDA
                ) VALUES (
                    :tipo, :descricao, :quantidade, :validade, :unidadeMedida
                )
                RETURNING RECURSO_ID INTO :id";

            cmd.Parameters.Add(new OracleParameter("tipo", recurso.Tipo));
            cmd.Parameters.Add(new OracleParameter("descricao", recurso.Descricao ?? (object)DBNull.Value));
            cmd.Parameters.Add(new OracleParameter("quantidade", recurso.Quantidade));
            cmd.Parameters.Add(new OracleParameter("validade", recurso.Validade ?? (object)DBNull.Value));
            cmd.Parameters.Add(new OracleParameter("unidadeMedida", recurso.UnidadeMedida));

            var idParam = new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add(idParam);

            await cmd.ExecuteNonQueryAsync();
            
            recurso.RecursoId = Convert.ToInt32(idParam.Value.ToString());
            return recurso;
        }
    }

    public async Task UpdateRecurso(Recurso recurso)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                UPDATE RECURSO SET
                    TIPO = :tipo,
                    DESCRICAO = :descricao,
                    QUANTIDADE = :quantidade,
                    VALIDADE = :validade,
                    UNIDADE_MEDIDA = :unidadeMedida
                WHERE RECURSO_ID = :id";

            cmd.Parameters.Add(new OracleParameter("tipo", recurso.Tipo));
            cmd.Parameters.Add(new OracleParameter("descricao", recurso.Descricao ?? (object)DBNull.Value));
            cmd.Parameters.Add(new OracleParameter("quantidade", recurso.Quantidade));
            cmd.Parameters.Add(new OracleParameter("validade", recurso.Validade ?? (object)DBNull.Value));
            cmd.Parameters.Add(new OracleParameter("unidadeMedida", recurso.UnidadeMedida));
            cmd.Parameters.Add(new OracleParameter("id", recurso.RecursoId));

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task AjustarQuantidadeRecurso(int id, int quantidade)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "UPDATE RECURSO SET QUANTIDADE = QUANTIDADE + :quantidade WHERE RECURSO_ID = :id";
            cmd.Parameters.Add(new OracleParameter("quantidade", quantidade));
            cmd.Parameters.Add(new OracleParameter("id", id));

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteRecurso(int id)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "DELETE FROM RECURSO WHERE RECURSO_ID = :id";
            cmd.Parameters.Add(new OracleParameter("id", id));

            await cmd.ExecuteNonQueryAsync();
        }
    }

    private Recurso MapToRecurso(OracleDataReader reader)
    {
        return new Recurso
        {
            RecursoId = Convert.ToInt32(reader["RECURSO_ID"]),
            Tipo = reader["TIPO"].ToString(),
            Descricao = reader["DESCRICAO"] != DBNull.Value ? reader["DESCRICAO"].ToString() : null,
            Quantidade = Convert.ToInt32(reader["QUANTIDADE"]),
            Validade = reader["VALIDADE"] != DBNull.Value ? Convert.ToDateTime(reader["VALIDADE"]) : (DateTime?)null,
            UnidadeMedida = reader["UNIDADE_MEDIDA"].ToString()
        };
    }
}