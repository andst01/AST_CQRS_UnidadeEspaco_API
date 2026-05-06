using FluentValidation.Results;
using MediatR;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

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

        

        public async Task PublishEvent()
        {
            // 1. Busca todas as entidades que herdaram de EntidadeBase e têm eventos acumulados
            //var domainEntities = _context.ChangeTracker
            //    .Entries<EntityBase>()
            //    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            _context.ChangeTracker.DetectChanges();
            var count = _context.ChangeTracker.Entries().Count();

            var domainEntities = _context.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            //var domainEvents = domainEntities
            //    .SelectMany(x => x.Entity.DomainEvents)
            //    .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // 2. Limpa os eventos das entidades para não disparar em duplicidade
            //domainEntities.ToList()
            //    .ForEach(entity => entity.Entity.ClearDomainEvents());

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }

            // 3. Dispara cada evento via MediatR
            // Esses eventos serão capturados pelos Handlers que atualizarão o MongoDB
            //var tasks = domainEvents
            //    .Select(async (domainEvent) => {
            //        await _mediator.Publish(domainEvent);
            //    });

           // await Task.WhenAll(tasks);
        }

        public async Task<ValidationResult> PublishEventNovo()
        {
            var validationResult = new ValidationResult();

            // 1. Sincroniza o estado atual do rastreador
            _context.ChangeTracker.DetectChanges();

            // 2. Captura as entidades que possuem eventos ANTES do SaveChanges
            // É crucial fazer isso agora, pois após o commit, entidades deletadas podem sumir do tracker.
            var domainEntities = _context.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            // 3. Extrai os eventos para uma lista local
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // 4. Limpa os eventos das entidades para evitar que sejam disparados novamente 
            // caso o mesmo contexto seja usado em outra operação no mesmo escopo.
            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            // 5. EFETIVA O COMMIT NO BANCO DE DADOS (MongoDB)
            // Se o CommandHandler usou _context.Remove(), a exclusão física ocorre aqui.
            var success = await _context.Commit();

            // 6. VALIDAÇÃO: Se havia algo para salvar (eventos detectados) mas o banco não confirmou
            if (!success && domainEvents.Any())
            {
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                    string.Empty, "Não foi possível persistir as alterações no banco de dados."));
                return validationResult;
            }

            // 7. DISPARO DAS NOTIFICAÇÕES (Somente após o sucesso da persistência)
            // Aqui os NotificationHandlers (como o que atualiza o Read Model) serão executados.
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
