using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class InmuebleFiltroRepository : BaseRepository, IInmuebleFiltroRepository
    {
        public InmuebleFiltroRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public IList<Inmueble> ObtenerPorFiltro(InmuebleFiltro filtro)
        {
            var lista = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.*,
                                   p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido,
                                   t.Tipo AS TipoNombre,
                                   (CASE
                                        WHEN @fechaDesde IS NOT NULL AND @fechaHasta IS NOT NULL AND EXISTS (
                                            SELECT 1 FROM reserva r2
                                            WHERE r2.InmuebleId = i.Id
                                              AND r2.Estado = 'Vigente'
                                              AND r2.FechaDesde < @fechaHasta
                                              AND r2.FechaHasta > @fechaDesde
                                        ) THEN 'Reservado'
                                        ELSE 'Disponible'
                                    END) AS EstadoDinamico
                            FROM inmueble i
                            INNER JOIN propietario p ON i.PropietarioId = p.Id
                            INNER JOIN tipoinmueble t ON i.TipoInmuebleId = t.Id
                            LEFT JOIN reserva r ON i.Id = r.InmuebleId AND r.Estado <> 'Cancelado'
                            WHERE 1=1";

                var parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@fechaDesde", (object?)filtro.FechaDesde ?? DBNull.Value),
                    new MySqlParameter("@fechaHasta", (object?)filtro.FechaHasta ?? DBNull.Value),
                };

                if (filtro.SinReservasDias.HasValue && filtro.SinReservasDias.Value > 0)
                {
                    DateTime fechaLimite = DateTime.Now.AddDays(-filtro.SinReservasDias.Value);
                    sql += @" AND i.Id NOT IN (
                                SELECT DISTINCT InmuebleId
                                FROM reserva
                                WHERE Estado <> 'Cancelado'
                                  AND FechaHasta >= @fechaLimite)";
                    parameters.Add(new MySqlParameter("@fechaLimite", fechaLimite));
                }

                if (!string.IsNullOrWhiteSpace(filtro.PropietarioTexto))
                {
                    sql += @" AND (p.Nombre LIKE @propietarioTexto
                                OR p.Apellido LIKE @propietarioTexto
                                OR CONCAT(p.Nombre, ' ', p.Apellido) LIKE @propietarioTexto
                                OR CONCAT(p.Apellido, ' ', p.Nombre) LIKE @propietarioTexto)";
                    parameters.Add(new MySqlParameter("@propietarioTexto", $"%{filtro.PropietarioTexto.Trim()}%"));
                }

                if (filtro.TipoInmuebleId.HasValue && filtro.TipoInmuebleId.Value > 0)
                {
                    sql += " AND i.TipoInmuebleId = @tipoInmuebleId";
                    parameters.Add(new MySqlParameter("@tipoInmuebleId", filtro.TipoInmuebleId.Value));
                }

                sql += " GROUP BY i.Id, p.Id, t.Id";

                string expresionEstado = @"(CASE
                                            WHEN @fechaDesde IS NOT NULL AND @fechaHasta IS NOT NULL AND EXISTS (
                                                SELECT 1 FROM reserva r2
                                                WHERE r2.InmuebleId = i.Id
                                                  AND r2.Estado = 'Vigente'
                                                  AND r2.FechaDesde < @fechaHasta
                                                  AND r2.FechaHasta > @fechaDesde
                                            ) THEN 'Reservado'
                                            ELSE 'Disponible'
                                        END)";

                if (filtro.MasReservadosUltimoAno)
                {
                    DateTime unAnoAtras = DateTime.Now.AddYears(-1);
                    sql += @" ORDER BY EstadoDinamico ASC, COUNT(CASE WHEN r.FechaDesde >= @unAnoAtras THEN 1 END) DESC, i.Id DESC";
                    parameters.Add(new MySqlParameter("@unAnoAtras", unAnoAtras));
                }
                else
                {
                    sql += " ORDER BY i.Id DESC";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                                ImgPortadaURL = reader.IsDBNull(reader.GetOrdinal("ImgPortadaURL")) ? string.Empty : reader.GetString("ImgPortadaURL"),
                                Cupo = reader.GetInt32("Cupo"),
                                Direccion = reader.GetString("Direccion"),
                                PrecioXDia = reader.GetDecimal("PrecioXDia"),
                                Estado = reader.GetString("EstadoDinamico"),
                                PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Activo = reader.GetBoolean("Activo"),
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Nombre = reader.GetString("PropietarioNombre"),
                                    Apellido = reader.GetString("PropietarioApellido")
                                },
                                TipoInmueble = new TipoInmueble
                                {
                                    Id = reader.GetInt32("TipoInmuebleId"),
                                    Tipo = reader.GetString("TipoNombre")
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
