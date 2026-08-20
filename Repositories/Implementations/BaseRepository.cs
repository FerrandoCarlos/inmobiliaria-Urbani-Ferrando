using Microsoft.Extensions.Configuration;

namespace InmobiliariaApp.Repositories.Implementations
{
    //Conexión
    public abstract class BaseRepository
    {
        protected readonly string connectionString;
        // comentario de prueba en rama joseBranch

        protected BaseRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró  la cadena de conexión 'DefaultConnection' en appsettings.json."
                );
        }
    }
}
