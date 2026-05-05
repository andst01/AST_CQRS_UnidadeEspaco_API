using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnidadeEspacoSrv.Data.Contexto;

namespace UnidadeEspacoSrv.CrossCuting.Configuration
{
    public static class DataBaseConfiguration
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<SQLDbContext>(x =>
               x.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")));

            
        }
    }
}
