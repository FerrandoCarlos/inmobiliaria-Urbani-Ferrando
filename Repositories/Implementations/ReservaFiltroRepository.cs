using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class ReservaFiltroRepository : BaseRepository, IReservaFiltroRepository
    {
        public ReservaFiltroRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public IList<Reserva> ObtenerPorFiltro(ReservaFiltro filtro)
        {
            var lista = new List<Reserva>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT r.*,
                                   i.Direccion AS InmuebleDireccion,
                                   inq.Nombre AS InquilinoNombre, inq.Apellido AS InquilinoApellido,
                                   uc.Nombre AS NombreUsuarioC, uc.Apellido AS ApellidoUsuarioC,
                                   ut.Nombre AS NombreUsuarioT, ut.Apellido AS ApellidoUsuarioT
                            FROM reserva r
                            INNER JOIN inmueble i ON r.InmuebleId = i.Id
                            INNER JOIN inquilino inq ON r.InquilinoId = inq.Id
                            LEFT JOIN usuario uc ON r.CreadoPorId = uc.Id
                            LEFT JOIN usuario ut ON r.TerminadoPorId = ut.Id
                            WHERE 1=1";
                var parameters = new List<MySqlParameter>();

                if (!filtro.FechaDesde.HasValue && !filtro.FechaHasta.HasValue && !filtro.FinalizanEnDias.HasValue)
                {
                    sql += @" AND r.Estado = 'Vigente'";
                }
                if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue)
                {
                    sql += @" AND r.FechaDesde < @fechaHasta AND r.FechaHasta > @fechaDesde";
                    parameters.Add(new MySqlParameter("@fechaDesde", filtro.FechaDesde.Value));
                    parameters.Add(new MySqlParameter("@fechaHasta", filtro.FechaHasta.Value));
                }
                if (filtro.FinalizanEnDias.HasValue && filtro.FinalizanEnDias.Value > 0)
                {
                    DateTime hoy = DateTime.Today;
                    DateTime fechaLimite = hoy.AddDays(filtro.FinalizanEnDias.Value);
                    sql += @" AND r.FechaHasta BETWEEN @hoy AND @fechaLimite";
                    parameters.Add(new MySqlParameter("@hoy", hoy));
                    parameters.Add(new MySqlParameter("@fechaLimite", fechaLimite));
                }
                sql += " ORDER BY r.FechaHasta ASC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Reserva
                            {
                                Id = reader.GetInt32("Id"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                FechaDesde = reader.GetDateTime("FechaDesde"),
                                FechaHasta = reader.GetDateTime("FechaHasta"),
                                FechaTerminacion = reader.IsDBNull(reader.GetOrdinal("FechaTerminacion")) ? null : reader.GetDateTime("FechaTerminacion"),
                                MontoPorDia = reader.GetDecimal("MontoPorDia"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? 0 : reader.GetDecimal("Multa"),
                                Estado = reader.GetString("Estado"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion"),

                                CreadoPorId = reader.IsDBNull(reader.GetOrdinal("CreadoPorId")) ? null : reader.GetInt32("CreadoPorId"),
                                TerminadoPorId = reader.IsDBNull(reader.GetOrdinal("TerminadoPorId")) ? null : reader.GetInt32("TerminadoPorId"),

                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("InmuebleId"),
                                    Direccion = reader.IsDBNull(reader.GetOrdinal("InmuebleDireccion")) ? "" : reader.GetString("InmuebleDireccion")
                                },
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("InquilinoNombre")) ? "" : reader.GetString("InquilinoNombre"),
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("InquilinoApellido")) ? "" : reader.GetString("InquilinoApellido")
                                },

                                CreadoPor = reader.IsDBNull(reader.GetOrdinal("CreadoPorId")) ? null : new Usuario
                                {
                                    Id = reader.GetInt32("CreadoPorId"),
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("NombreUsuarioC")) ? "" : reader.GetString("NombreUsuarioC"),
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("ApellidoUsuarioC")) ? "" : reader.GetString("ApellidoUsuarioC")
                                },
                                TerminadoPor = reader.IsDBNull(reader.GetOrdinal("TerminadoPorId")) ? null : new Usuario
                                {
                                    Id = reader.GetInt32("TerminadoPorId"),
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("NombreUsuarioT")) ? "" : reader.GetString("NombreUsuarioT"),
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("ApellidoUsuarioT")) ? "" : reader.GetString("ApellidoUsuarioT")
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}