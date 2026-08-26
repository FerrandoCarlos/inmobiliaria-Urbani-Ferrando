using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations

{
    /// <summary>
    /// Acceso a datos de Inquilino mediante ADO.NET puro con MySqlConnector.
    /// Todas las consultas son parametrizadas: cero concatenación de
    /// strings SQL, cero riesgo de inyección SQL. Hereda connectionString
    /// de BaseRepository (DRY: la lectura de configuración vive en un solo lugar).
    /// </summary>
    public class InquilinoRepository : BaseRepository, IInquilinoRepository
    {
        public InquilinoRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Inquilino entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO inquilino
                    (Dni, Nombre, Apellido, Telefono, Email, Activo, FechaCreacion)
                    VALUES (@dni, @nombre, @apellido, @telefono, @email, @activo, @fechaCreacion);
                    SELECT LAST_INSERT_ID()"; // Para devolver el ID insertado
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@dni", entidad.Dni);
                    command.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    command.Parameters.AddWithValue("@apellido", entidad.Apellido);
                    command.Parameters.AddWithValue("@telefono", entidad.Telefono);
                    command.Parameters.AddWithValue("@email", entidad.Email);
                    command.Parameters.AddWithValue("@activo", entidad.Activo);
                    command.Parameters.AddWithValue("@fechaCreacion", DateTime.Now);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = res; // Se asigna el ID devuelto al inquilino
                    connection.Close();
                }
            }

            return res;
        }

        public int Baja(int id)
        {
            // Baja lógica: nunca DELETE físico, para preservar integridad
            // referencial con Inmueble/Reserva en entregas futuras.
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE inquilino SET Activo = 0 WHERE Id = @id";
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

        public int Modificacion(Inquilino entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE inquilino
                    SET Dni=@dni, Nombre=@nombre, Apellido=@apellido, Telefono=@telefono, Email=@email
                    WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@dni", entidad.Dni);
                    command.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    command.Parameters.AddWithValue("@apellido", entidad.Apellido);
                    command.Parameters.AddWithValue("@telefono", entidad.Telefono);
                    command.Parameters.AddWithValue("@email", entidad.Email);
                    command.Parameters.AddWithValue("@activo", entidad.Activo);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Reactivar(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Inquilino SET Activo = 1 WHERE Id = @id";

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

        public IList<Inquilino> ObtenerLista(int paginaNro = -1, int tamPagina = 10)
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT Id, Dni, Nombre, Apellido, Telefono, Email, Activo, FechaCreacion
                FROM inquilino
                WHERE Activo = 1
                ORDER BY Apellido, Nombre
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
                        res.Add(MapearInquilino(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inquilino> ObtenerListaInactivos()
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT Id, Dni, Nombre, Apellido, Telefono, Email, Activo, FechaCreacion
                    FORM Inquilino
                    WHERE Activo = 0
                    ORDER BY Apellido, Nombre";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearInquilino(reader));
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
                string sql = "SELECT COUNT(Id) FROM inquilino WHERE Activo = 1";
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
        public int ObtenerCantidadInactivos()
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(Id) FROM Inquilino WHERE Activo = 0";

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
        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? i = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT Id, Dni, Nombre, Apellido, Telefono, Email, Activo, FechaCreacion
                FROM inquilino
                WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        i = MapearInquilino(reader);
                    }
                    connection.Close();
                }
            }
            return i;
        }

        public bool ExisteDni(string dni, int idExcluir = 0)
        {
            bool existe = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(1) FROM inquilino
                WHERE Dni = @dni AND Id <> @idExcluir";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@dni", dni);
                    command.Parameters.AddWithValue("@idExcluir", idExcluir);

                    connection.Open();
                    existe = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    connection.Close();
                }
            }
            return existe;
        }

        private static Inquilino MapearInquilino(MySqlDataReader reader)
        {
            return new Inquilino
            {
                Id = reader.GetInt32(nameof(Inquilino.Id)),
                Dni = reader.GetString(nameof(Inquilino.Dni)),
                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Telefono)))
                    ? null : reader.GetString(nameof(Inquilino.Telefono)),
                Email = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Email)))
                    ? null : reader.GetString(nameof(Inquilino.Email)),
                Activo = reader.GetBoolean(nameof(Inquilino.Activo)),
                FechaCreacion = reader.GetDateTime(nameof(Inquilino.FechaCreacion))
            };
        }
    }
}
