using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class TipoInmuebleRepository : BaseRepository, ITipoInmuebleRepository
    {
        public TipoInmuebleRepository(IConfiguration configuration) : base (configuration)
        {
            
        }
        public int Alta(TipoInmueble entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO tipoInmueble (Tipo)
                    VALUES (@tipo);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tipo", entidad.Tipo);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = res;
                }
            }
            return res;
        }
        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE tipoInmueble SET Activo = 0 WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
            
        
        public int Modificacion(TipoInmueble entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE tipoInmueble
                                SET Tipo = @tipo
                                WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tipo", entidad.Tipo);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public IList<TipoInmueble> ObtenerLista(int paginaNro, int tamPagina) {
            List<TipoInmueble> res = new List<TipoInmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =  @"SELECT Id, Tipo FROM tipoInmueble
                                WHERE Activo = 1 
                                LIMIT @limit OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@limit", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearTipoInmueble(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(Id) FROM tipoInmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT Id, Tipo FROM tipoInmueble
                            WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entidad = MapearTipoInmueble(reader);
                        }
                    }
                }
            }
            return entidad;
        }

        private static TipoInmueble MapearTipoInmueble(MySqlDataReader reader)
        {
            return new TipoInmueble
            {
                Id = reader.GetInt32("Id"),
                Tipo = reader[nameof(TipoInmueble.Tipo)] == DBNull.Value ? "" : reader.GetString(nameof(TipoInmueble.Tipo)),
                Activo = reader.GetBoolean(nameof(TipoInmueble.Activo)),
            };
        }
    }
}