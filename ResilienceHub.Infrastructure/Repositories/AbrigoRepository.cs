using System.Data;
using Oracle.ManagedDataAccess.Client;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Infrastructure.Repositories;

public class AbrigoRepository : IAbrigoRepository
    {
        private readonly IDbConnection _connection;

        public AbrigoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Abrigo>> GetAbrigos()
        {
            var abrigos = new List<Abrigo>();

            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ABRIGO WHERE ATIVO = 1";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        abrigos.Add(new Abrigo
                        {
                            AbrigoId = Convert.ToInt32(reader["ABRIGO_ID"]),
                            Nome = reader["NOME"].ToString(),
                            Localizacao = reader["LOCALIZACAO"].ToString(),
                            Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LATITUDE"]) : null,
                            Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LONGITUDE"]) : null,
                            Capacidade = Convert.ToInt32(reader["CAPACIDADE"]),
                            OcupacaoAtual = Convert.ToInt32(reader["OCUPACAO_ATUAL"]),
                            DataCadastro = Convert.ToDateTime(reader["DATA_CADASTRO"]),
                            Ativo = Convert.ToInt32(reader["ATIVO"]) == 1
                        });
                    }
                }
            }

            return abrigos;
        }

        public async Task<Abrigo> GetAbrigo(int id)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ABRIGO WHERE ABRIGO_ID = :id AND ATIVO = 1";
                cmd.Parameters.Add(new OracleParameter("id", id));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Abrigo
                        {
                            AbrigoId = Convert.ToInt32(reader["ABRIGO_ID"]),
                            Nome = reader["NOME"].ToString(),
                            Localizacao = reader["LOCALIZACAO"].ToString(),
                            Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LATITUDE"]) : null,
                            Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LONGITUDE"]) : null,
                            Capacidade = Convert.ToInt32(reader["CAPACIDADE"]),
                            OcupacaoAtual = Convert.ToInt32(reader["OCUPACAO_ATUAL"]),
                            DataCadastro = Convert.ToDateTime(reader["DATA_CADASTRO"]),
                            Ativo = Convert.ToInt32(reader["ATIVO"]) == 1
                        };
                    }
                }
            }

            return null;
        }

        public async Task<Abrigo> CreateAbrigo(Abrigo abrigo)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = @"
                    INSERT INTO ABRIGO (NOME, LOCALIZACAO, LATITUDE, LONGITUDE, CAPACIDADE, OCUPACAO_ATUAL, DATA_CADASTRO, ATIVO) 
                    VALUES (:nome, :localizacao, :latitude, :longitude, :capacidade, :ocupacaoAtual, :dataCadastro, :ativo)
                    RETURNING ABRIGO_ID INTO :id";

                cmd.Parameters.Add(new OracleParameter("nome", abrigo.Nome));
                cmd.Parameters.Add(new OracleParameter("localizacao", abrigo.Localizacao));
                cmd.Parameters.Add(new OracleParameter("latitude", abrigo.Latitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("longitude", abrigo.Longitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("capacidade", abrigo.Capacidade));
                cmd.Parameters.Add(new OracleParameter("ocupacaoAtual", abrigo.OcupacaoAtual));
                cmd.Parameters.Add(new OracleParameter("dataCadastro", abrigo.DataCadastro));
                cmd.Parameters.Add(new OracleParameter("ativo", abrigo.Ativo ? 1 : 0));

                var idParam = new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output);
                cmd.Parameters.Add(idParam);

                await cmd.ExecuteNonQueryAsync();

                abrigo.AbrigoId = Convert.ToInt32(idParam.Value.ToString());
                return abrigo;
            }
        }

        public async Task UpdateAbrigo(Abrigo abrigo)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = @"
                    UPDATE ABRIGO 
                    SET NOME = :nome, 
                        LOCALIZACAO = :localizacao, 
                        LATITUDE = :latitude, 
                        LONGITUDE = :longitude, 
                        CAPACIDADE = :capacidade, 
                        OCUPACAO_ATUAL = :ocupacaoAtual, 
                        ATIVO = :ativo 
                    WHERE ABRIGO_ID = :id";

                cmd.Parameters.Add(new OracleParameter("nome", abrigo.Nome));
                cmd.Parameters.Add(new OracleParameter("localizacao", abrigo.Localizacao));
                cmd.Parameters.Add(new OracleParameter("latitude", abrigo.Latitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("longitude", abrigo.Longitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("capacidade", abrigo.Capacidade));
                cmd.Parameters.Add(new OracleParameter("ocupacaoAtual", abrigo.OcupacaoAtual));
                cmd.Parameters.Add(new OracleParameter("ativo", abrigo.Ativo ? 1 : 0));
                cmd.Parameters.Add(new OracleParameter("id", abrigo.AbrigoId));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAbrigo(int id)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "UPDATE ABRIGO SET ATIVO = 0 WHERE ABRIGO_ID = :id";
                cmd.Parameters.Add(new OracleParameter("id", id));
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }