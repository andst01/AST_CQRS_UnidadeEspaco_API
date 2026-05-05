using MediatR;
using MongoDB.Driver;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application.Commnds
{
    public class UnidadeNotificationHandler
        : INotificationHandler<UnidadeCreateNotification>,
          INotificationHandler<UnidadeUpdateNotification>,
          INotificationHandler<UnidadeDeleteNotification>
    {

        private readonly IUnidadeMongoDbRepository _repository;

        public UnidadeNotificationHandler(IUnidadeMongoDbRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UnidadeCreateNotification notification, CancellationToken cancellationToken)
        {
           await _repository.AddAsync(notification, "Unidade");
        }

        public async Task Handle(UnidadeUpdateNotification notification, CancellationToken cancellationToken)
        {
            //await _repository.GetByFilter(Builders<UnidadeNotification>.Filter.Where(e => e.Id == notification.Id), "Unidade");
            notification.Espaco = null;
            await _repository.UpdateAsync(Builders<UnidadeNotification>.Filter.Where(e => e.Id == notification.Id), notification, "Unidade");
        }

        public async Task Handle(UnidadeDeleteNotification notification, CancellationToken cancellationToken)
        {
           await _repository.RemoveAsync(Builders<UnidadeNotification>.Filter.Where(e => e.Id == notification.Id), "Unidade");
        }
    }
}
