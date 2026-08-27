using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class InmuebleRepository : BaseRepository, IInmuebleRepository
    {
        public InmuebleRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inmueble entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO inmueble
                    (PropietarioId, Cupo, Direccion, Tipo, PrecioXDia, Estado, PorcentajeReserva, Latitud, Longitud)
                    VALUES (@propietarioid, @cupo, @direccion, @tipo, @precioxdia, @estado, @porcentajereserva, @latitud, @longitud);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@propietarioid", entidad.PropietarioId);
                    command.Parameters.AddWithValue("@cupo", entidad.Cupo);
                    command.Parameters.AddWithValue("@direccion", entidad.Direccion);
                    command.Parameters.AddWithValue("@tipo", entidad.Tipo);
                    command.Parameters.AddWithValue("@precioxdia", entidad.PrecioXDia);
                    command.Parameters.AddWithValue("@estado", entidad.Estado);
                    command.Parameters.AddWithValue("@porcentajereserva", entidad.PorcentajeReserva);
                    command.Parameters.AddWithValue("@latitud", entidad.Latitud);
                    command.Parameters.AddWithValue("@longitud", entidad.Longitud);

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
                string sql = "UPDATE inmueble SET Estado = 'Inactivo' WHERE Id = @id";
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

        public int Modificacion(Inmueble entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inmueble
                    SET PropietarioId=@propietarioid, Cupo=@cupo, Direccion=@direccion, Tipo=@tipo, 
                        PrecioXDia=@precioxdia, Estado=@estado, PorcentajeReserva=@porcentajereserva, 
                        Latitud=@latitud, Longitud=@longitud
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@propietarioid", entidad.PropietarioId);
                    command.Parameters.AddWithValue("@cupo", entidad.Cupo);
                    command.Parameters.AddWithValue("@direccion", entidad.Direccion);
                    command.Parameters.AddWithValue("@tipo", entidad.Tipo);
                    command.Parameters.AddWithValue("@precioxdia", entidad.PrecioXDia);
                    command.Parameters.AddWithValue("@estado", entidad.Estado);
                    command.Parameters.AddWithValue("@porcentajereserva", entidad.PorcentajeReserva);
                    command.Parameters.AddWithValue("@latitud", entidad.Latitud);
                    command.Parameters.AddWithValue("@longitud", entidad.Longitud);
                    command.Parameters.AddWithValue("@id", entidad.Id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int ModificarPortada(int id, string url)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inmueble
                    SET ImgPortadaURL=@imgportadaurl
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@imgportadaurl", string.IsNullOrEmpty(url) ? DBNull.Value : url);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inmueble> res = new List<Inmueble>();
            int offset = (paginaNro - 1) * tamPagina;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT i.Id, i.Cupo, i.Direccion, i.Tipo, i.PrecioXDia, i.Estado, 
                                      i.PorcentajeReserva, i.Latitud, i.Longitud, i.ImgPortadaURL, i.PropietarioId, 
                                      p.Nombre, p.Apellido, p.Dni
                               FROM inmueble i 
                               INNER JOIN propietario p ON i.PropietarioId = p.Id
                               ORDER BY i.Id
                               LIMIT @tamPagina OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", offset);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearInmueble(reader));
                        }
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(Id) FROM inmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT i.Id, i.Cupo, i.Direccion, i.Tipo, i.PrecioXDia, i.Estado, 
                                      i.PorcentajeReserva, i.Latitud, i.Longitud, i.ImgPortadaURL, i.PropietarioId, 
                                      p.Nombre, p.Apellido, p.Dni
                               FROM inmueble i 
                               INNER JOIN propietario p ON i.PropietarioId = p.Id
                               WHERE i.Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entidad = MapearInmueble(reader);
                        }
                    }
                }
            }
            return entidad;
        }

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            List<Inmueble> res = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT i.Id, i.Cupo, i.Direccion, i.Tipo, i.PrecioXDia, i.Estado, 
                                      i.PorcentajeReserva, i.Latitud, i.Longitud, i.ImgPortadaURL, i.PropietarioId, 
                                      p.Nombre, p.Apellido, p.Dni
                               FROM inmueble i 
                               INNER JOIN propietario p ON i.PropietarioId = p.Id
                               WHERE i.PropietarioId = @IdPropietario";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@IdPropietario", idPropietario);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearInmueble(reader));
                        }
                    }
                }
            }
            return res;
        }

        private static Inmueble MapearInmueble(MySqlDataReader reader)
        {
            return new Inmueble
            {
                Id = reader.GetInt32(nameof(Inmueble.Id)),
                Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Direccion)),
                Tipo = reader[nameof(Inmueble.Tipo)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Tipo)),
                PrecioXDia = reader.GetDecimal(nameof(Inmueble.PrecioXDia)),
                Estado = reader[nameof(Inmueble.Estado)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Estado)),
                PorcentajeReserva = reader.GetDecimal(nameof(Inmueble.PorcentajeReserva)),
                Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                ImgPortadaURL = reader[nameof(Inmueble.ImgPortadaURL)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.ImgPortadaURL)),
                PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
                Propietario = new Propietario
                {
                    Id = reader.GetInt32(nameof(Inmueble.PropietarioId)),
                    Nombre = reader.GetString(nameof(Propietario.Nombre)),
                    Apellido = reader.GetString(nameof(Propietario.Apellido)),
                    Dni = reader.GetString(nameof(Propietario.Dni))
                }
            };
        }
    }
}
