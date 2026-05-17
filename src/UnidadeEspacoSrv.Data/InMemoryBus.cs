using FluentValidation.Results;
using MediatR;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;


namespace UnidadeEspacoSrv.Data
{
    public class InMemoryBus : IMediatorHandler
    {

        private readonly IMediator _mediator;
        private readonly SQLDbContext _context;


        public InMemoryBus(IMediator mediator,
                           SQLDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<bool?> PublishEvent<T>(T @event) where T : EventBase
        {
            bool? publishEvent = null;

            await _mediator.Publish(@event);

            return publishEvent;
        }


        #region PublishEvent Antigo

        //public async Task PublishEventAntigo()
        //{
        //    // 1. Busca todas as entidades que herdaram de EntidadeBase e têm eventos acumulados
        //    //var domainEntities = _context.ChangeTracker
        //    //    .Entries<EntityBase>()
        //    //    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        //    _context.ChangeTracker.DetectChanges();
        //    var count = _context.ChangeTracker.Entries().Count();

        //    var domainEntities = _context.ChangeTracker
        //        .Entries<EntityBase>()
        //        .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
        //        .ToList();

        //    //var domainEvents = domainEntities
        //    //    .SelectMany(x => x.Entity.DomainEvents)
        //    //    .ToList();

        //    var domainEvents = domainEntities
        //        .SelectMany(x => x.Entity.DomainEvents)
        //        .ToList();

        //    // 2. Limpa os eventos das entidades para não disparar em duplicidade
        //    //domainEntities.ToList()
        //    //    .ForEach(entity => entity.Entity.ClearDomainEvents());

        //    domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        //    foreach (var domainEvent in domainEvents)
        //    {
        //        await _mediator.Publish(domainEvent);
        //    }

        //    // 3. Dispara cada evento via MediatR
        //    // Esses eventos serão capturados pelos Handlers que atualizarão o MongoDB
        //    //var tasks = domainEvents
        //    //    .Select(async (domainEvent) => {
        //    //        await _mediator.Publish(domainEvent);
        //    //    });

        //    // await Task.WhenAll(tasks);
        //}

        #endregion

        public async Task<ValidationResult> CommitAsync()
        {
            var validationResult = new ValidationResult();

            var success = await _context.Commit();

            if (!success)
            {
                validationResult.Errors.Add(new ValidationFailure(
                    string.Empty, "Não foi possível persistir as alterações no banco de dados."));
                return validationResult;
            }

            return validationResult;

        }

        public async Task<ValidationResult> PublishEvent(bool hasCommit = true)
        {
            var validationResult = new ValidationResult();

            _context.ChangeTracker.DetectChanges();

            var domainEntities = _context.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            if (!hasCommit)
            {
                var valiationResult = await CommitAsync();
                if (!valiationResult.IsValid)
                    return valiationResult;
            }

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }

            return validationResult;
        }

        public async Task<ValidationResult> SendCommand<T>(T command) where T : IRequest<FluentValidation.Results.ValidationResult>
        {
            return await _mediator.Send(command);
        }

        public async Task<TResponse> SendCommand<TRequest, TResponse>(TRequest command) where TRequest : IRequest<TResponse>
        {
            return await _mediator.Send(command);
        }
    }
}
