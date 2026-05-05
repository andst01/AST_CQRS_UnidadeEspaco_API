using Microsoft.Extensions.DependencyInjection;
using UnidadeEspacoSrv.CrossCuting.AutoMapper;

namespace UnidadeEspacoSrv.CrossCuting.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection service)
        {
           if(service == null) throw new ArgumentNullException(nameof(service));
           
           service.AddAutoMapper(typeof(CommandToEntityMapping),
                                 typeof(EntityToNotificationMapping),
                                 typeof(EntityToViewModelMapping),
                                 typeof(NotificationToViewModelMapping),
                                 typeof(ViewlModelToCommandMapping),
                                 typeof(RequestToCommandMapping));
        }
    }
}
