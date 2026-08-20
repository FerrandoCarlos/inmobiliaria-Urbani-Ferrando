using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    /// <summary>
    /// Acceso a datos de Propietario mediante ADO.NET puro con MySqlConnector.
    /// Todas las consultas son parametrizadas: cero concatenación de
    /// strings SQL, cero riesgo de inyección SQL. Hereda connectionString
    /// de BaseRepository (DRY: la lectura de configuración vive en un solo lugar).
    /// </summary>
    public class PropietarioRepository : BaseRepository, IPropietarioRepository
    {
        public PropietarioRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Propietario entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Propietario (Dni, Nombre, Apellido, Telefono, Email, Activo, FechaCreacion)
                    VALUES (@dni, @nombre, @apellido, @telefono, @email, @activo, @fechaCreacion);
                    SELECT LAST_INSERT_ID();";

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
            // Baja lógica: nunca DELETE físico, para preservar integridad
            // referencial con Inmueble/Reserva en entregas futuras.
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Propietario SET Activo = 0 WHERE Id = @id";

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

        public int Modificacion(Propietario entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Propietario
                    SET Dni = @dni, Nombre = @nombre, Apellido = @apellido,
                        Telefono = @telefono, Email = @email
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

        public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Propietario> res = new List<Propietario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                // MySQL usa LIMIT/OFFSET (no OFFSET...FETCH NEXT como SQL Server).
                // Se listan solo los activos, siguiendo el criterio de baja lógica.
                string sql = @"
                    SELECT Id, Dni, Nombre, Apellido, Telefono, Email, Activo, FechaCreacion
                    FROM Propietario
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
                        res.Add(MapearPropietario(reader));
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
                string sql = "SELECT COUNT(Id) FROM Propietario WHERE Activo = 1";

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

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT Id, Dni, Nombre, Apellido, Telefono, Email, Activo, FechaCreacion
                    FROM Propietario
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = MapearPropietario(reader);
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public bool ExisteDni(string dni, int idExcluir = 0)
        {
            bool existe = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT COUNT(1) FROM Propietario
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


        /// Mapea una fila del DataReader a un objeto Propietario.
        /// Centralizado acá para evitar duplicar el mapeo en cada método (DRY).

        private static Propietario MapearPropietario(MySqlDataReader reader)
        {
            return new Propietario
            {
                Id = reader.GetInt32(nameof(Propietario.Id)),
                Dni = reader.GetString(nameof(Propietario.Dni)),
                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono)))
                    ? null : reader.GetString(nameof(Propietario.Telefono)),
                Email = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Email)))
                    ? null : reader.GetString(nameof(Propietario.Email)),
                Activo = reader.GetBoolean(nameof(Propietario.Activo)),
                FechaCreacion = reader.GetDateTime(nameof(Propietario.FechaCreacion))
            };
        }


        /// Agrega los parámetros comunes a INSERT y UPDATE, evitando
        /// duplicar la misma lista de parámetros en ambos métodos (DRY).

        private static void AgregarParametros(MySqlCommand command, Propietario propietario)
        {
            command.Parameters.AddWithValue("@dni", propietario.Dni);
            command.Parameters.AddWithValue("@nombre", propietario.Nombre);
            command.Parameters.AddWithValue("@apellido", propietario.Apellido);
            command.Parameters.AddWithValue("@telefono", (object?)propietario.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", (object?)propietario.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@activo", propietario.Activo);
        }
    }
}
