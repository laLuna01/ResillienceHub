using System.Data;
using Oracle.ManagedDataAccess.Client;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Infrastructure.Repositories;

public class AbrigoUsuarioRepository : IAbrigoUsuarioRepository {
    
    private readonly IDbConnection _connection;

    public AbrigoUsuarioRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<AbrigoUsuario>> GetRegistros()
    {
        var registros = new List<AbrigoUsuario>();

        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                SELECT au.*, a.nome as nome_abrigo, u.nome as nome_usuario 
                FROM ABRIGO_USUARIO au
                JOIN ABRIGO a ON au.abrigo_id = a.abrigo_id
                JOIN USUARIO u ON au.usuario_id = u.usuario_id";

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    registros.Add(MapToAbrigoUsuario(reader));
                }
            }
        }

        return registros;
    }

    public async Task<AbrigoUsuario> GetRegistro(int id)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                SELECT au.*, a.nome as nome_abrigo, u.nome as nome_usuario 
                FROM ABRIGO_USUARIO au
                JOIN ABRIGO a ON au.abrigo_id = a.abrigo_id
                JOIN USUARIO u ON au.usuario_id = u.usuario_id
                WHERE au.registro_id = :id";

            cmd.Parameters.Add(new OracleParameter("id", id));

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return MapToAbrigoUsuario(reader);
                }
            }
        }

        return null;
    }

    public async Task<IEnumerable<AbrigoUsuario>> GetRegistrosAtivosPorAbrigo(int abrigoId)
    {
        var registros = new List<AbrigoUsuario>();

        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                SELECT au.*, a.nome as nome_abrigo, u.nome as nome_usuario 
                FROM ABRIGO_USUARIO au
                JOIN ABRIGO a ON au.abrigo_id = a.abrigo_id
                JOIN USUARIO u ON au.usuario_id = u.usuario_id
                WHERE au.abrigo_id = :abrigoId AND au.status = 'ativo'";

            cmd.Parameters.Add(new OracleParameter("abrigoId", abrigoId));

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    registros.Add(MapToAbrigoUsuario(reader));
                }
            }
        }

        return registros;
    }

    public async Task<IEnumerable<AbrigoUsuario>> GetRegistrosPorUsuario(int usuarioId)
    {
        var registros = new List<AbrigoUsuario>();

        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                SELECT au.*, a.nome as nome_abrigo, u.nome as nome_usuario 
                FROM ABRIGO_USUARIO au
                JOIN ABRIGO a ON au.abrigo_id = a.abrigo_id
                JOIN USUARIO u ON au.usuario_id = u.usuario_id
                WHERE au.usuario_id = :usuarioId";

            cmd.Parameters.Add(new OracleParameter("usuarioId", usuarioId));

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    registros.Add(MapToAbrigoUsuario(reader));
                }
            }
        }

        return registros;
    }

    public async Task<AbrigoUsuario> CreateCheckIn(AbrigoUsuario registro)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                INSERT INTO ABRIGO_USUARIO (abrigo_id, usuario_id, data_checkin, status) 
                VALUES (:abrigoId, :usuarioId, :dataCheckin, :status)
                RETURNING registro_id INTO :id";

            cmd.Parameters.Add(new OracleParameter("abrigoId", registro.AbrigoId));
            cmd.Parameters.Add(new OracleParameter("usuarioId", registro.UsuarioId));
            cmd.Parameters.Add(new OracleParameter("dataCheckin", registro.DataCheckin));
            cmd.Parameters.Add(new OracleParameter("status", registro.Status));

            var idParam = new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add(idParam);

            await cmd.ExecuteNonQueryAsync();

            registro.RegistroId = Convert.ToInt32(idParam.Value.ToString());

            // Atualiza a ocupação do abrigo
            await AtualizarOcupacaoAbrigo(registro.AbrigoId, 1);

            return registro;
        }
    }

    public async Task<AbrigoUsuario> UpdateCheckOut(int registroId)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            // Primeiro obtemos o registro para pegar o abrigo_id
            var registro = await GetRegistro(registroId);
            if (registro == null) return null;

            cmd.CommandText = @"
                UPDATE ABRIGO_USUARIO 
                SET data_checkout = :dataCheckout, 
                    status = 'finalizado' 
                WHERE registro_id = :registroId
                RETURNING abrigo_id INTO :abrigoId";

            cmd.Parameters.Add(new OracleParameter("dataCheckout", DateTime.Now));
            cmd.Parameters.Add(new OracleParameter("registroId", registroId));
            var abrigoIdParam = new OracleParameter("abrigoId", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add(abrigoIdParam);

            await cmd.ExecuteNonQueryAsync();

            // Atualiza a ocupação do abrigo
            await AtualizarOcupacaoAbrigo(registro.AbrigoId, -1);

            return await GetRegistro(registroId);
        }
    }

    public async Task DeleteRegistro(int id)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = "DELETE FROM ABRIGO_USUARIO WHERE registro_id = :id";
            cmd.Parameters.Add(new OracleParameter("id", id));
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task AtualizarOcupacaoAbrigo(int abrigoId, int incremento)
    {
        using (var cmd = _connection.CreateCommand() as OracleCommand)
        {
            cmd.CommandText = @"
                UPDATE ABRIGO 
                SET ocupacao_atual = ocupacao_atual + :incremento 
                WHERE abrigo_id = :abrigoId";

            cmd.Parameters.Add(new OracleParameter("incremento", incremento));
            cmd.Parameters.Add(new OracleParameter("abrigoId", abrigoId));

            await cmd.ExecuteNonQueryAsync();
        }
    }

    private AbrigoUsuario MapToAbrigoUsuario(OracleDataReader reader)
    {
        return new AbrigoUsuario
        {
            RegistroId = Convert.ToInt32(reader["registro_id"]),
            AbrigoId = Convert.ToInt32(reader["abrigo_id"]),
            UsuarioId = Convert.ToInt32(reader["usuario_id"]),
            DataCheckin = Convert.ToDateTime(reader["data_checkin"]),
            DataCheckout = reader["data_checkout"] != DBNull.Value ? Convert.ToDateTime(reader["data_checkout"]) : null,
            Status = reader["status"].ToString()
        };
    }
}