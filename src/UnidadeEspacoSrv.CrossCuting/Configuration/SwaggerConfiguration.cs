using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.CrossCuting.Configuration
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));


            services.AddSwaggerGen
           (
               s =>
               {
                   s.SwaggerDoc
                   (
                       "v1"
                       , new OpenApiInfo
                       {
                           Version = "v1",
                           Title = "Espaço/Unidade API",
                           Description = "Espaço/Unidade API Swagger",
                           Contact = new OpenApiContact
                           {
                               // Name = "TransPetro",
                               Email = string.Empty
                           }
                       }

                   );


                   s.CustomSchemaIds(x => x.FullName);
                   s.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
               }
           );
        }
    }
}
