using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class ReservaRepository : BaseRepository, IReservaRepository
    {
        public ReservaRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Reserva entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO reserva (InquilinoId, InmuebleId, FechaDesde, FechaHasta, MontoPorDia, Estado, FechaCreacion)
                    VALUES (@inquilinoId, @inmuebleId,@fechaDesde,@fechaHasta,@montoPorDia,@estado,@fechaCreacion);
                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    AgregarParametros(command, entidad);
                    command.Parameters.AddWithValue("@fechaCreacion", DateTime.Now);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE reserva SET Estado = 'Finalizada' WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Reserva entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE reserva
                    SET InquilinoId = @inquilinoId, InmuebleId = @inmuebleId,
                        FechaDesde = @fechaDesde, FechaHasta = @fechaHasta,
                        MontoPorDia = @montoPorDia, Estado = @estado
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    AgregarParametros(command, entidad);
                    command.Parameters.AddWithValue("@id", entidad.Id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Reserva> res = new List<Reserva>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT r.Id, r.InquilinoId, r.InmuebleId, r.FechaDesde, r.FechaHasta,
                           r.FechaTerminacion, r.MontoPorDia, r.Multa, r.Estado, r.FechaCreacion,
                           i.Nombre as InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                           m.Direccion AS InmuebleDireccion
                    From reserva r
                    INNER JOIN inquilino i ON r.InquilinoId = i.Id
                    INNER JOIN inmueble m ON r.InmuebleId = m.Id
                    ORDER BY r.FechaDesde DESC
                    LIMIT @tamPagina OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearReserva(reader));
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
                string sql = "SELECT COUNT(Id) FROM reserva";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = reader.GetInt32(0);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? r = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT r.Id, r.InquilinoId, r.InmuebleId, r.FechaDesde, r.FechaHasta,
                           r.FechaTerminacion, r.MontoPorDia, r.Multa, r.Estado, r.FechaCreacion,
                           i.Nombre as InquilinoNombre, i.Apellido AS InquilinoApellido, i.Dni AS InquilinoDni,
                           m.Direccion AS InmuebleDireccion
                    From reserva r
                    INNER JOIN inquilino i ON r.InquilinoId = i.Id
                    INNER JOIN inmueble m ON r.InmuebleId = m.Id
                    WHERE r.Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        r = MapearReserva(reader);
                    }
                    connection.Close();
                }
            }
            return r;
        }

        public bool ExisteSolapamiento(int inmuebleId, DateTime fechaDesde, DateTime fechaHasta, int idExcluir = 0)
        {
            bool existe = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT COUNT(1) FROM reserva
                    WHERE InmuebleId = @inmuebleId
                        AND Estado = 'Vigente'
                        AND Id <> @idExcluir
                        AND FechaDesde <= @fechaHasta
                        AND FechaHasta >= @fechaDesde";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    command.Parameters.AddWithValue("@idExcluir", idExcluir);
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta);

                    connection.Open();
                    existe = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    connection.Close();
                }
            }
            return existe;
        }
        private static Reserva MapearReserva(MySqlDataReader reader)
        {
            return new Reserva
            {
                Id = reader.GetInt32(nameof(Reserva.Id)),
                InquilinoId = reader.GetInt32(nameof(Reserva.InquilinoId)),
                InmuebleId = reader.GetInt32(nameof(Reserva.InmuebleId)),
                FechaDesde = reader.GetDateTime(nameof(Reserva.FechaDesde)),
                FechaHasta = reader.GetDateTime(nameof(Reserva.FechaHasta)),
                FechaTerminacion = reader.IsDBNull(reader.GetOrdinal(nameof(Reserva.FechaTerminacion)))
                    ? null : reader.GetDateTime(nameof(Reserva.FechaTerminacion)),
                Estado = reader.GetString(nameof(Reserva.Estado)),
                FechaCreacion = reader.GetDateTime(nameof(Reserva.FechaCreacion)),
                Inquilino = new Inquilino
                {
                    Id = reader.GetInt32(nameof(Reserva.InquilinoId)),
                    Nombre = reader.GetString("InquilinoNombre"),
                    Apellido = reader.GetString("InquilinoApellido"),
                    Dni = reader.GetString("InquilinoDni")
                },
                Inmueble = new Inmueble
                {
                    Id = reader.GetInt32(nameof(Reserva.InmuebleId)),
                    Direccion = reader.GetString("InmuebleDireccion")
                }
            };
        }
        private static void AgregarParametros(MySqlCommand command, Reserva reserva)
        {
            command.Parameters.AddWithValue("@inquilinoId", reserva.InquilinoId);
            command.Parameters.AddWithValue("@inmuebleId", reserva.InmuebleId);
            command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
            command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);
            command.Parameters.AddWithValue("@montoPorDia", reserva.MontoPorDia);
            command.Parameters.AddWithValue("@estado", reserva.Estado);
        }
    }
}
