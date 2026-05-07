using AutoMapper;
using FluentValidation.Results;
using MediatR;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Application.Commnds
{
    public class UnidadeCommandHandler : //: CommandHandler,
        IRequestHandler<UnidadeCreateCommand, Unidade>,
        IRequestHandler<UnidadeUpdateCommand, Unidade>,
        IRequestHandler<UnidadeDeleteCommand, ValidationResult>
    {

        private readonly IUnidadeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _mediator;

        public UnidadeCommandHandler(IUnidadeRepository repository,
                                    IMapper mapper,
                                    IMediatorHandler mediator)
        {
            _repository = repository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Unidade> Handle(UnidadeCreateCommand request, CancellationToken cancellationToken)
        {
            var objeto = _mapper.Map<Unidade>(request);

            if (!request.IsValid()) return null;

            var response = await _repository.AddAsync(objeto);

            response.ValidationResult = await _mediator.CommitAsync();

            if (!response.ValidationResult.IsValid) return response;

            response.AddDomainEvent(_mapper.Map<UnidadeCreateNotification>(response));

            response.ValidationResult = await _mediator.PublishEvent(true);

            return response;
        }

        public async Task<Unidade> Handle(UnidadeUpdateCommand request, CancellationToken cancellationToken)
        {
            var objeto = _mapper.Map<Unidade>(request);

            if (!request.IsValid()) return null;

            var response = await _repository.UpdateAsync(objeto, objeto.Id);

            response.ValidationResult = await _mediator.CommitAsync();

            if (!response.ValidationResult.IsValid) return response;

            response.AddDomainEvent(_mapper.Map<UnidadeUpdateNotification>(response));

            response.ValidationResult = await _mediator.PublishEvent(true);

            return response;
        }

        public async Task<ValidationResult> Handle(UnidadeDeleteCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) return null;

            var objeto = await _repository.GetByIdAsync(request.Id);

            await _repository.DeleteAsync(objeto.Id);

            objeto.AddDomainEvent(_mapper.Map<UnidadeDeleteNotification>(objeto));

            objeto.ValidationResult = await _mediator.PublishEvent(false);

            return objeto.ValidationResult;

        }
    }
}
