using System.Data;
using Oracle.ManagedDataAccess.Client;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnection _connection;

        public UsuarioRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
            var usuarios = new List<Usuario>();
            
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM USUARIO";
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        usuarios.Add(MapToUsuario(reader));
                    }
                }
            }
            
            return usuarios;
        }

        public async Task<Usuario> GetUsuario(int id)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM USUARIO WHERE USUARIO_ID = :id";
                cmd.Parameters.Add(new OracleParameter("id", id));
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToUsuario(reader);
                    }
                }
            }
            
            return null;
        }

        public async Task<Usuario> GetUsuarioByCpf(string cpf)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM USUARIO WHERE CPF = :cpf";
                cmd.Parameters.Add(new OracleParameter("cpf", cpf));
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToUsuario(reader);
                    }
                }
            }
            
            return null;
        }

        public async Task<Usuario> GetUsuarioByEmail(string email)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM USUARIO WHERE EMAIL = :email";
                cmd.Parameters.Add(new OracleParameter("email", email));
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToUsuario(reader);
                    }
                }
            }
            
            return null;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosPorTipo(string tipo)
        {
            var usuarios = new List<Usuario>();
            
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM USUARIO WHERE TIPO = :tipo";
                cmd.Parameters.Add(new OracleParameter("tipo", tipo));
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        usuarios.Add(MapToUsuario(reader));
                    }
                }
            }
            
            return usuarios;
        }

        public async Task<Usuario> CreateUsuario(Usuario usuario)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = @"
                    INSERT INTO USUARIO (
                        NOME, CPF, TIPO, EMAIL, SENHA, TELEFONE, 
                        LATITUDE, LONGITUDE, DATA_ULTIMA_LOCALIZACAO, TOKEN_NOTIFICACAO
                    ) VALUES (
                        :nome, :cpf, :tipo, :email, :senha, :telefone, 
                        :latitude, :longitude, :dataLocalizacao, :token
                    )
                    RETURNING USUARIO_ID INTO :id";

                cmd.Parameters.Add(new OracleParameter("nome", usuario.Nome));
                cmd.Parameters.Add(new OracleParameter("cpf", usuario.Cpf));
                cmd.Parameters.Add(new OracleParameter("tipo", usuario.Tipo));
                cmd.Parameters.Add(new OracleParameter("email", usuario.Email));
                cmd.Parameters.Add(new OracleParameter("senha", usuario.Senha));
                cmd.Parameters.Add(new OracleParameter("telefone", usuario.Telefone ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("latitude", usuario.Latitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("longitude", usuario.Longitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("dataLocalizacao", usuario.DataUltimaLocalizacao ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("token", usuario.TokenNotificacao ?? (object)DBNull.Value));

                var idParam = new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output);
                cmd.Parameters.Add(idParam);

                await cmd.ExecuteNonQueryAsync();
                
                usuario.UsuarioId = Convert.ToInt32(idParam.Value.ToString());
                return usuario;
            }
        }

        public async Task UpdateUsuario(Usuario usuario)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = @"
                    UPDATE USUARIO SET
                        NOME = :nome,
                        CPF = :cpf,
                        TIPO = :tipo,
                        EMAIL = :email,
                        SENHA = :senha,
                        TELEFONE = :telefone,
                        LATITUDE = :latitude,
                        LONGITUDE = :longitude,
                        DATA_ULTIMA_LOCALIZACAO = :dataLocalizacao,
                        TOKEN_NOTIFICACAO = :token
                    WHERE USUARIO_ID = :id";

                cmd.Parameters.Add(new OracleParameter("nome", usuario.Nome));
                cmd.Parameters.Add(new OracleParameter("cpf", usuario.Cpf));
                cmd.Parameters.Add(new OracleParameter("tipo", usuario.Tipo));
                cmd.Parameters.Add(new OracleParameter("email", usuario.Email));
                cmd.Parameters.Add(new OracleParameter("senha", usuario.Senha));
                cmd.Parameters.Add(new OracleParameter("telefone", usuario.Telefone ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("latitude", usuario.Latitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("longitude", usuario.Longitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("dataLocalizacao", usuario.DataUltimaLocalizacao ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("token", usuario.TokenNotificacao ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("id", usuario.UsuarioId));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateUsuarioToken(int id, string token)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "UPDATE USUARIO SET TOKEN_NOTIFICACAO = :token WHERE USUARIO_ID = :id";
                cmd.Parameters.Add(new OracleParameter("token", token));
                cmd.Parameters.Add(new OracleParameter("id", id));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteUsuario(int id)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "DELETE FROM USUARIO WHERE USUARIO_ID = :id";
                cmd.Parameters.Add(new OracleParameter("id", id));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private Usuario MapToUsuario(OracleDataReader reader)
        {
            return new Usuario
            {
                UsuarioId = Convert.ToInt32(reader["USUARIO_ID"]),
                Nome = reader["NOME"].ToString(),
                Cpf = reader["CPF"].ToString(),
                Tipo = reader["TIPO"].ToString(),
                Email = reader["EMAIL"].ToString(),
                Senha = reader["SENHA"].ToString(),
                Telefone = reader["TELEFONE"] != DBNull.Value ? reader["TELEFONE"].ToString() : null,
                Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LATITUDE"]) : (decimal?)null,
                Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LONGITUDE"]) : (decimal?)null,
                DataUltimaLocalizacao = reader["DATA_ULTIMA_LOCALIZACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ULTIMA_LOCALIZACAO"]) : (DateTime?)null,
                DataCadastro = Convert.ToDateTime(reader["DATA_CADASTRO"]),
                TokenNotificacao = reader["TOKEN_NOTIFICACAO"] != DBNull.Value ? reader["TOKEN_NOTIFICACAO"].ToString() : null
            };
        }
    }