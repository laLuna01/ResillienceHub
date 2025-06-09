using System.Data;
using Oracle.ManagedDataAccess.Client;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Infrastructure.Repositories;

public class AlertaRepository : IAlertaRepository
    {
        private readonly IDbConnection _connection;

        public AlertaRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Alerta>> GetAlertas()
        {
            var alertas = new List<Alerta>();
            
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ALERTA";
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        alertas.Add(MapToAlerta(reader));
                    }
                }
            }
            
            return alertas;
        }

        public async Task<Alerta> GetAlerta(int id)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ALERTA WHERE ALERTA_ID = :id";
                cmd.Parameters.Add(new OracleParameter("id", id));
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToAlerta(reader);
                    }
                }
            }
            
            return null;
        }

        public async Task<IEnumerable<Alerta>> GetAlertasAtivos()
        {
            var alertas = new List<Alerta>();
            
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ALERTA WHERE STATUS = 'ativo'";
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        alertas.Add(MapToAlerta(reader));
                    }
                }
            }
            
            return alertas;
        }

        public async Task<IEnumerable<Alerta>> GetAlertasPorTipo(string tipo)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ALERTA WHERE TIPO = :tipo";
                cmd.Parameters.Add(new OracleParameter("tipo", tipo));
                
                var alertas = new List<Alerta>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        alertas.Add(MapToAlerta(reader));
                    }
                }
                return alertas;
            }
        }

        public async Task<IEnumerable<Alerta>> GetAlertasPorSeveridade(string severidade)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ALERTA WHERE SEVERIDADE = :severidade";
                cmd.Parameters.Add(new OracleParameter("severidade", severidade));
                
                var alertas = new List<Alerta>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        alertas.Add(MapToAlerta(reader));
                    }
                }
                return alertas;
            }
        }

        public async Task<IEnumerable<Alerta>> GetAlertasPorStatus(string status)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "SELECT * FROM ALERTA WHERE STATUS = :status";
                cmd.Parameters.Add(new OracleParameter("status", status));
                
                var alertas = new List<Alerta>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        alertas.Add(MapToAlerta(reader));
                    }
                }
                return alertas;
            }
        }

        public async Task<Alerta> CreateAlerta(Alerta alerta)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = @"
                    INSERT INTO ALERTA (
                        TIPO, SEVERIDADE, AREA_GEOGRAFICA, LATITUDE, LONGITUDE, 
                        RAIO_AFETADO, DESCRICAO, DATA_HORA_INICIO, 
                        DATA_ESTIMADA_TERMINO, FONTE, STATUS
                    ) VALUES (
                        :tipo, :severidade, :areaGeo, :latitude, :longitude, 
                        :raio, :descricao, :dataInicio, 
                        :dataTermino, :fonte, :status
                    )
                    RETURNING ALERTA_ID INTO :id";

                cmd.Parameters.Add(new OracleParameter("tipo", alerta.Tipo));
                cmd.Parameters.Add(new OracleParameter("severidade", alerta.Severidade));
                cmd.Parameters.Add(new OracleParameter("areaGeo", alerta.AreaGeografica));
                cmd.Parameters.Add(new OracleParameter("latitude", alerta.Latitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("longitude", alerta.Longitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("raio", alerta.RaioAfetado ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("descricao", alerta.Descricao ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("dataInicio", alerta.DataHoraInicio));
                cmd.Parameters.Add(new OracleParameter("dataTermino", alerta.DataEstimadaTermino ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("fonte", alerta.Fonte));
                cmd.Parameters.Add(new OracleParameter("status", alerta.Status));

                var idParam = new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output);
                cmd.Parameters.Add(idParam);

                await cmd.ExecuteNonQueryAsync();
                
                alerta.AlertaId = Convert.ToInt32(idParam.Value.ToString());
                return alerta;
            }
        }

        public async Task UpdateAlerta(Alerta alerta)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = @"
                    UPDATE ALERTA SET
                        TIPO = :tipo,
                        SEVERIDADE = :severidade,
                        AREA_GEOGRAFICA = :areaGeo,
                        LATITUDE = :latitude,
                        LONGITUDE = :longitude,
                        RAIO_AFETADO = :raio,
                        DESCRICAO = :descricao,
                        DATA_HORA_INICIO = :dataInicio,
                        DATA_ESTIMADA_TERMINO = :dataTermino,
                        FONTE = :fonte,
                        STATUS = :status
                    WHERE ALERTA_ID = :id";

                cmd.Parameters.Add(new OracleParameter("tipo", alerta.Tipo));
                cmd.Parameters.Add(new OracleParameter("severidade", alerta.Severidade));
                cmd.Parameters.Add(new OracleParameter("areaGeo", alerta.AreaGeografica));
                cmd.Parameters.Add(new OracleParameter("latitude", alerta.Latitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("longitude", alerta.Longitude ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("raio", alerta.RaioAfetado ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("descricao", alerta.Descricao ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("dataInicio", alerta.DataHoraInicio));
                cmd.Parameters.Add(new OracleParameter("dataTermino", alerta.DataEstimadaTermino ?? (object)DBNull.Value));
                cmd.Parameters.Add(new OracleParameter("fonte", alerta.Fonte));
                cmd.Parameters.Add(new OracleParameter("status", alerta.Status));
                cmd.Parameters.Add(new OracleParameter("id", alerta.AlertaId));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAlertaStatus(int id, string status)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "UPDATE ALERTA SET STATUS = :status WHERE ALERTA_ID = :id";
                cmd.Parameters.Add(new OracleParameter("status", status));
                cmd.Parameters.Add(new OracleParameter("id", id));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAlerta(int id)
        {
            using (var cmd = _connection.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = "DELETE FROM ALERTA WHERE ALERTA_ID = :id";
                cmd.Parameters.Add(new OracleParameter("id", id));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private Alerta MapToAlerta(OracleDataReader reader)
        {
            return new Alerta
            {
                AlertaId = Convert.ToInt32(reader["ALERTA_ID"]),
                Tipo = reader["TIPO"].ToString(),
                Severidade = reader["SEVERIDADE"].ToString(),
                AreaGeografica = reader["AREA_GEOGRAFICA"].ToString(),
                Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LATITUDE"]) : (decimal?)null,
                Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LONGITUDE"]) : (decimal?)null,
                RaioAfetado = reader["RAIO_AFETADO"] != DBNull.Value ? Convert.ToDecimal(reader["RAIO_AFETADO"]) : (decimal?)null,
                Descricao = reader["DESCRICAO"] != DBNull.Value ? reader["DESCRICAO"].ToString() : null,
                DataHoraInicio = Convert.ToDateTime(reader["DATA_HORA_INICIO"]),
                DataEstimadaTermino = reader["DATA_ESTIMADA_TERMINO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ESTIMADA_TERMINO"]) : (DateTime?)null,
                Fonte = reader["FONTE"].ToString(),
                Status = reader["STATUS"].ToString()
            };
        }
    }