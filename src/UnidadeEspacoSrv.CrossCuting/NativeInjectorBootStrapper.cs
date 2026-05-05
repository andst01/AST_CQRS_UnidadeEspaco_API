using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UnidadeEspacoSrv.Application;
using UnidadeEspacoSrv.Application.Commands;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Data;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Data.Repository.MongoDB;
using UnidadeEspacoSrv.Data.Repository.SQL;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.CrossCuting
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisteredServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(NativeInjectorBootStrapper).Assembly);

            services.AddScoped<IMediatorHandler, InMemoryBus>();
            
            services.AddScoped((typeof(ISQLBaseRepository<>)), (typeof(SQLBaseRepository<>)));
            services.AddScoped<IUnidadeRepository, UnidadeRepository>();
            services.AddScoped<IEspacoRepository, EspacoRepository>();

            services.AddScoped<IMongoContext, MongoDbContext>();
            services.AddScoped((typeof(IMongoDbBaseRepository<>)), (typeof(MongoDbBaseRepository<>)));
            services.AddScoped<IUnidadeMongoDbRepository, UnidadeMongoDbRepository>();
            services.AddScoped<IEspacoMongoDbRepository, EspacoMongoDbRepository>();

           
            services.AddScoped(typeof(IAppBase<,,,,,,>), typeof(AppBase<,,,,,,>));
            services.AddScoped<IEspacoApp, EspacoApp>();
            services.AddScoped<IUnidadeApp, UnidadeApp>();

            services.AddScoped<INotificationHandler<UnidadeCreateNotification>, UnidadeNotificationHandler>();
            services.AddScoped<INotificationHandler<UnidadeUpdateNotification>, UnidadeNotificationHandler>();
            services.AddScoped<INotificationHandler<UnidadeDeleteNotification>, UnidadeNotificationHandler>();

            services.AddScoped<INotificationHandler<EspacoCreateNotification>, EspacoNotificationHandler>();
            services.AddScoped<INotificationHandler<EspacoUpdateNotification>, EspacoNotificationHandler>();
            services.AddScoped<INotificationHandler<EspacoDeleteNotification>, EspacoNotificationHandler>();

            services.AddScoped<IRequestHandler<UnidadeCreateCommand, Unidade>, UnidadeCommandHandler>();
            services.AddScoped<IRequestHandler<UnidadeUpdateCommand, Unidade>, UnidadeCommandHandler>();
            services.AddScoped<IRequestHandler<UnidadeDeleteCommand, ValidationResult>, UnidadeCommandHandler>();

            services.AddScoped<IRequestHandler<EspacoCreateCommand, Espaco>, EspacoCommandHandler>();
            services.AddScoped<IRequestHandler<EspacoUpdateCommand, Espaco>, EspacoCommandHandler>();
            services.AddScoped<IRequestHandler<EspacoDeleteCommand, ValidationResult>, EspacoCommandHandler>();

            services.AddScoped<SQLDbContext>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }
    }
}
