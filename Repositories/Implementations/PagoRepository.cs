using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class PagoRepository : BaseRepository, IPagoRepository
    {
        public PagoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                if (entidad.Fecha == default(DateTime))
                {
                    entidad.Fecha = DateTime.Now;
                }
                string sql = @"
                    INSERT INTO pago (ReservaId, Monto, Concepto, Estado, Activo, Fecha, CreadoPorId)
                    VALUES (@reservaId, @monto, @concepto, @estado, @activo, @fecha, @creadoPorId);
                    SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@reservaId", entidad.ReservaId);
                    command.Parameters.AddWithValue("@monto", entidad.Monto);
                    command.Parameters.AddWithValue("@concepto", entidad.Concepto);
                    command.Parameters.AddWithValue("@estado", entidad.Estado);
                    command.Parameters.AddWithValue("@activo", entidad.Activo);
                    command.Parameters.AddWithValue("@fecha", entidad.Fecha);
                    command.Parameters.AddWithValue("@creadoPorId", entidad.CreadoPorId);
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
                string sql = "UPDATE pago SET Activo = 0 WHERE Id = @id";
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

        public int ModificacionEstado(string estado, int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE pago SET Estado = @estado WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@estado", estado);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Pago entidad)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE pago
                            SET ReservaId = @reservaId,
                                Monto = @monto,
                                Concepto = @concepto,
                                Estado = @estado
                            WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@reservaId", entidad.ReservaId);
                    command.Parameters.AddWithValue("@monto", entidad.Monto);
                    command.Parameters.AddWithValue("@concepto", entidad.Concepto);
                    command.Parameters.AddWithValue("@estado", entidad.Estado);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int ModificacionConcepto(int id, string nuevoConcepto)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE pago
                                SET Concepto = @concepto
                                WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@concepto", nuevoConcepto);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Reactivar(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE pago SET Activo = 1 WHERE Id = @id";
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

        public Pago? ObtenerPorId(int id)
        {
            Pago? entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = ObtenerSelectBase() + " WHERE p.Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entidad = MapearPago(reader);
                        }
                    }
                }
            }
            return entidad;
        }

        public IList<Pago> ObtenerListaPendientes(string estado, int PaginaNro, int tamPagina)
        {
            List<Pago> res = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = ObtenerSelectBase() + " WHERE p.Estado = @estado LIMIT @limit OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@estado", estado);
                    command.Parameters.AddWithValue("@limit", tamPagina);
                    command.Parameters.AddWithValue("@offset", (PaginaNro - 1) * tamPagina);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return res;
        }

        public IList<Pago> ObtenerListaMultas(int PaginaNro, int tamPagina)
        {
            List<Pago> res = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = ObtenerSelectBase() + " WHERE p.Concepto = 'Multa' LIMIT @limit OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@limit", tamPagina);
                    command.Parameters.AddWithValue("@offset", (PaginaNro - 1) * tamPagina);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return res;
        }

        public IList<Pago> ObtenerListaCancelados(int PaginaNro, int tamPagina)
        {
            List<Pago> res = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = ObtenerSelectBase() + " WHERE p.Activo = 0 LIMIT @limit OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@limit", tamPagina);
                    command.Parameters.AddWithValue("@offset", (PaginaNro - 1) * tamPagina);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return res;
        }

        public IList<Pago> ObtenerListaPagados(int PaginaNro, int tamPagina)
        {
            List<Pago> res = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = ObtenerSelectBase() + " WHERE p.Estado = 'Pagado' LIMIT @limit OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@limit", tamPagina);
                    command.Parameters.AddWithValue("@offset", (PaginaNro - 1) * tamPagina);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return res;
        }

        public IList<Pago> ObtenerLista(int PaginaNro, int tamPagina)
        {
            List<Pago> res = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = ObtenerSelectBase() + " WHERE Activo = 1 LIMIT @limit OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@limit", tamPagina);
                    command.Parameters.AddWithValue("@offset", (PaginaNro - 1) * tamPagina);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearPago(reader));
                        }
                    }
                }
            }
            return res;
        }

        public Pago? BuscarPorReserva(int idReserva)
        {
            Pago? entidad = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = ObtenerSelectBase() + " WHERE p.ReservaId = @idReserva";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entidad = MapearPago(reader);
                        }
                    }
                }
            }
            return entidad;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(Id) FROM pago";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

        public int CancelarSaldoRestantePendiente(int reservaId)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE pago
                               SET Estado = 'Cancelado'
                               WHERE ReservaId = @reservaId
                                AND Concepto = 'Saldo Restante'
                                AND Estado = 'Pendiente'";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@reservaId", reservaId);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        private static string ObtenerSelectBase()
        {
            return @"SELECT
                p.Id AS PagoId, p.ReservaId, p.Monto, p.Concepto, p.Estado, p.Activo, p.Fecha,
                p.CreadoPorId, p.AnuladoPorId,
                r.InmuebleId, r.InquilinoId, r.FechaDesde, r.FechaHasta, r.FechaTerminacion, r.MontoPorDia, r.Multa,
                uc.Nombre AS CreadoPorNombre, uc.Apellido AS CreadoPorApellido,
                ua.Nombre AS AnuladoPorNombre, ua.Apellido AS AnuladoPorApellido
                FROM pago p
                INNER JOIN reserva r ON p.ReservaId = r.Id
                LEFT JOIN usuario uc ON p.CreadoPorId = uc.Id
                LEFT JOIN usuario ua ON p.AnuladoPorId = ua.Id";
        }

        private static Pago MapearPago(MySqlDataReader reader)
        {
            return new Pago
            {
                Id = reader.GetInt32("PagoId"),
                ReservaId = reader.GetInt32(nameof(Pago.ReservaId)),
                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                Concepto = reader[nameof(Pago.Concepto)] == DBNull.Value ? "" : reader.GetString(nameof(Pago.Concepto)),
                Estado = reader[nameof(Pago.Estado)] == DBNull.Value ? "" : reader.GetString(nameof(Pago.Estado)),
                Activo = reader.GetBoolean(nameof(Pago.Activo)),
                Fecha = reader.GetDateTime(nameof(Pago.Fecha)),
                CreadoPorId = reader.GetInt32(nameof(Pago.CreadoPorId)),
                AnuladoPorId = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.AnuladoPorId)))
                    ? null : reader.GetInt32(nameof(Pago.AnuladoPorId)),
                CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPorNombre"))
                    ? null : new Usuario { Nombre = reader.GetString("CreadoPorNombre"), Apellido = reader.GetString("CreadoPorApellido") },
                AnuladoPor = reader.IsDBNull(reader.GetOrdinal("AnuladoPorNombre"))
                    ? null : new Usuario { Nombre = reader.GetString("AnuladoPorNombre"), Apellido = reader.GetString("AnuladoPorApellido") },
                Reserva = new Reserva
                {
                    Id = reader.GetInt32(nameof(Pago.ReservaId)),
                    InmuebleId = reader.GetInt32(nameof(Reserva.InmuebleId)),
                    InquilinoId = reader.GetInt32(nameof(Reserva.InquilinoId)),
                    FechaDesde = reader.GetDateTime(nameof(Reserva.FechaDesde)),
                    FechaHasta = reader.GetDateTime(nameof(Reserva.FechaHasta)),
                    FechaTerminacion = reader[nameof(Reserva.FechaTerminacion)] == DBNull.Value ? null : reader.GetDateTime(nameof(Reserva.FechaTerminacion)),
                    MontoPorDia = reader[nameof(Reserva.MontoPorDia)] == DBNull.Value ? 0 : reader.GetDecimal(nameof(Reserva.MontoPorDia)),
                    Multa = reader[nameof(Reserva.Multa)] == DBNull.Value ? 0 : reader.GetDecimal(nameof(Reserva.Multa))
                }
            };
        }

        public int Anular(int id, int? usuarioId)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE pago SET Activo = 0, AnuladoPorId = @anuladoPorId WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@anuladoPorId", (object?)usuarioId ?? DBNull.Value);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}
