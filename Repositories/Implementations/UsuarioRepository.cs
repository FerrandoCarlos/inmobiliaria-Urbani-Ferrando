using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        public UsuarioRepository(IConfiguration configuration) : base(configuration) { }

        public int Alta(Usuario entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Usuario (Email, PasswordHash, Nombre, Apellido, Avatar, RolId, Activo, FechaCreacion)
                    VALUES (@email, @passwordHash, @nombre, @apellido, @avatar, @rolId, @activo, @fechaCreacion);
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
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Usuario SET Activo = 0 WHERE Id = @id";
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

        public int Modificacion(Usuario entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Usuario
                    SET Email = @email, Nombre = @nombre, Apellido = @apellido, Avatar = @avatar, RolId = @rolId
                    WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", entidad.Email);
                    command.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    command.Parameters.AddWithValue("@apellido", entidad.Apellido);
                    command.Parameters.AddWithValue("@avatar", (object?)entidad.Avatar);
                    command.Parameters.AddWithValue("@rolId", entidad.RolId);
                    command.Parameters.AddWithValue("@id", entidad.Id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int CambiarPassword(int id, string nuevoHash)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Usuario
                                SET PasswordHash = @hash
                                WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@hash", nuevoHash);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int ActualizarPerfil(int id, string nombre, string apellido, string? avatarUrl)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Usuario SET Nombre = @nombre, Apellido = @apellido, Avatar = @avatar WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@apellido", apellido);
                    command.Parameters.AddWithValue("@avatar", (object?)avatarUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Usuario> res = new List<Usuario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT u.Id, u.Email, u.PasswordHash, u.Nombre, u.Apellido, u.Avatar,
                                u.RolId, u.Activo, u.FechaCreacion, r.Nombre AS RolNombre
                            FROM Usuario u
                            INNER JOIN Rol r ON u.RolId = r.Id
                            WHERE u.Activo = 1
                            ORDER BY u.Apellido, u.Nombre
                            LIMIT  @tamPagina OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearUsuario(reader));
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
                string sql = @" SELECT COUNT(Id)
                                FROM USUARIO
                                WHERE Activo =1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read()) res = reader.GetInt32(0);
                    connection.Close();
                }
            }
            return res;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? u = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT u.Id, u.Email, u.PasswordHash, u.Nombre, u.Apellido, u.Avatar,
                                u.RolId, u.Activo, u.FechaCreacion, r.Nombre AS RolNombre
                                FROM Usuario u
                                INNER JOIN Rol r ON u.RolId = r.Id
                                WHERE u.Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read()) u = MapearUsuario(reader);
                    connection.Close();
                }
            }
            return u;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? u = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT u.Id, u.Email, u.PasswordHash, u.Nombre, u.Apellido, u.Avatar, u.RolId,
                    u.Activo, u.FechaCreacion, r.Nombre AS RolNombre
                    FROM Usuario u
                    INNER JOIN Rol r ON u.RolId = r.Id
                    WHERE u.Email = @email AND u.Activo = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read()) u = MapearUsuario(reader);
                    connection.Close();
                }
            }
            return u;
        }

        public bool ExisteEmail(string email, int idExcluir = 0)
        {
            bool existe = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(1)
                                FROM Usuario
                                WHERE Email = @email
                                AND Id <> @idExcluir ";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@idExcluir", idExcluir);
                    connection.Open();
                    existe = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    connection.Close();
                }
            }
            return existe;
        }

        public int EliminarAvatar(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Usuario SET Avatar = NULL WHERE Id = @id";
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

        private static Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32(nameof(Usuario.Id)),
                Email = reader.GetString(nameof(Usuario.Email)),
                PasswordHash = reader.GetString(nameof(Usuario.PasswordHash)),
                Nombre = reader.GetString(nameof(Usuario.Nombre)),
                Apellido = reader.GetString(nameof(Usuario.Apellido)),
                Avatar = reader.IsDBNull(reader.GetOrdinal(nameof(Usuario.Avatar)))
                    ? null : reader.GetString(nameof(Usuario.Avatar)),
                RolId = reader.GetInt32(nameof(Usuario.RolId)),
                Activo = reader.GetBoolean(nameof(Usuario.Activo)),
                FechaCreacion = reader.GetDateTime(nameof(Usuario.FechaCreacion)),
                Rol = new Rol
                {
                    Id = reader.GetInt32(nameof(Usuario.RolId)),
                    Nombre = reader.GetString("RolNombre")
                }
            };
        }
        private static void AgregarParametros(MySqlCommand command, Usuario usuario)
        {
            command.Parameters.AddWithValue("@email", usuario.Email);
            command.Parameters.AddWithValue("@passwordHash", usuario.PasswordHash);
            command.Parameters.AddWithValue("@nombre", usuario.Nombre);
            command.Parameters.AddWithValue("@apellido", usuario.Apellido);
            command.Parameters.AddWithValue("@avatar", (object?)usuario.Avatar ?? DBNull.Value);
            command.Parameters.AddWithValue("@rolId", usuario.RolId);
            command.Parameters.AddWithValue("@activo", usuario.Activo);
        }
    }
}
