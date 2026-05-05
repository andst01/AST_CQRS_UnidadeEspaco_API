using MediatR;
using MongoDB.Driver;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application.Commands
{
    public class EspacoNotificationHandler
        : INotificationHandler<EspacoCreateNotification>,
          INotificationHandler<EspacoUpdateNotification>,
          INotificationHandler<EspacoDeleteNotification>
    {

        private readonly IEspacoMongoDbRepository _repository;

        public EspacoNotificationHandler(IEspacoMongoDbRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(EspacoCreateNotification notification, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(notification, "Espaco");
        }

        public async Task Handle(EspacoUpdateNotification notification, CancellationToken cancellationToken)
        {
           await _repository.UpdateAsync(Builders<EspacoNotification>.Filter.Where(e => e.Id == notification.Id), notification, "Espaco");
        }

        public async Task Handle(EspacoDeleteNotification notification, CancellationToken cancellationToken)
        {
           await _repository.RemoveAsync(Builders<EspacoNotification>.Filter.Where(e => e.Id == notification.Id), "Espaco");
        }
    }
}
