using AutoMapper;
using FluentValidation.Results;
using MediatR;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Application.Commnds
{
    public class UnidadeCommandHandler : CommandHandler,
        IRequestHandler<UnidadeCreateCommand, Unidade>,
        IRequestHandler<UnidadeUpdateCommand, Unidade>,
        IRequestHandler<UnidadeDeleteCommand, ValidationResult>
    {

        private readonly IUnidadeRepository _repository;
        private readonly IMapper _mapper;

        public UnidadeCommandHandler(IUnidadeRepository repository,
                                    IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unidade> Handle(UnidadeCreateCommand request, CancellationToken cancellationToken)
        {
            var objeto = _mapper.Map<Unidade>(request);

            if (!request.IsValid()) return null;

            var response = await _repository.AddAsync(objeto);

            response.ValidationResult = await Commit(_repository);

            if (!response.ValidationResult.IsValid) return response;

            response.AddDomainEvent(_mapper.Map<UnidadeCreateNotification>(response));

            return response;
        }

        public async Task<Unidade> Handle(UnidadeUpdateCommand request, CancellationToken cancellationToken)
        {
            var objeto = _mapper.Map<Unidade>(request);

            if (!request.IsValid()) return null;

            var response = await _repository.UpdateAsync(objeto, objeto.Id);

            response.ValidationResult = await Commit(_repository, "");

            if (!response.ValidationResult.IsValid) return response;

            response.AddDomainEvent(_mapper.Map<UnidadeUpdateNotification>(response));

            return response;
        }

        public async Task<ValidationResult> Handle(UnidadeDeleteCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) return null;

            var objeto = await _repository.GetByIdAsync(request.Id);

            if (objeto != null)
            {
                var notification = _mapper.Map<UnidadeDeleteNotification>(objeto);
                objeto.AddDomainEvent(notification);
            }
            else
            {
                var validationResult = new ValidationResult();
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Id", "Registro não encontrado."));
                return validationResult;
            }


            await _repository.DeleteAsync(objeto.Id);
            
            objeto.ValidationResult = await Commit(_repository, "");

            if (!objeto.ValidationResult.IsValid) return objeto.ValidationResult;

            return objeto.ValidationResult;

        }
    }
}
