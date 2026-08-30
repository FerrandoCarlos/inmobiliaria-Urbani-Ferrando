using System.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaApp.Repositories.Implementations
{
    public class ImagenesInmuebleRepository : BaseRepository, IImagenesInmuebleRepository
    {
        public ImagenesInmuebleRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public int Alta(ImagenesInmueble p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO imagenesinmueble
                (InmuebleId, ImgURL)
                VALUES (@inmuebleId, @imgURL)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inmuebleId", p.InmuebleId);
                    command.Parameters.AddWithValue("@imgURL", p.ImgURL);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
                return res;
            }
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql= @$"DELETE FROM imagenesinmueble WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id",id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(ImagenesInmueble p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE imagenesinmueble
                    ImgURL=@imgURL
                    WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@imgURL", p.ImgURL);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public ImagenesInmueble? ObtenerPorId(int id)
        {
            ImagenesInmueble? res = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT
                    i.Id, i.InmuebleId, i.ImgURL
                    FROM imagenesinmueble i
                    WHERE i.Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new ImagenesInmueble();
                        res.Id = reader.GetInt32(nameof(ImagenesInmueble.Id));
                        res.InmuebleId = reader.GetInt32(nameof(ImagenesInmueble.InmuebleId));
                        res.ImgURL = reader.GetString(nameof(ImagenesInmueble.ImgURL));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<ImagenesInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            List<ImagenesInmueble> res = new List<ImagenesInmueble>();
            int offset = (paginaNro-1) * tamPagina;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT
                    i.Id, i.InmuebleId, i.ImgURL
                    FROM imagenesinmueble i
                    ORDER BY Id
                    LIMIT @tamPagina OFFSET @offset";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new ImagenesInmueble
                        {
                            Id = reader.GetInt32(nameof(ImagenesInmueble.Id)),
                            InmuebleId = reader.GetInt32(nameof(ImagenesInmueble.InmuebleId)),
                            ImgURL = reader.GetString(nameof(ImagenesInmueble.ImgURL)),
                        });
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
                string sql = @"SELECT COUNT(Id) FROM imagenesinmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public IList<ImagenesInmueble> BuscarPorInmueble(int inmuebleId)
        {
            List<ImagenesInmueble> res = new List<ImagenesInmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT
                    i.Id, i.InmuebleId, i.ImgURL
                    FROM imagenesinmueble i
                    WHERE i.InmuebleId=@inmuebleId";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read()){
                        res.Add(new ImagenesInmueble
                        {
                            Id = reader.GetInt32(nameof(ImagenesInmueble.Id)),
                            InmuebleId = reader.GetInt32(nameof(ImagenesInmueble.InmuebleId)),
                            ImgURL = reader.GetString(nameof(ImagenesInmueble.ImgURL)),
                        });
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }

}