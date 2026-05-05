using AutoMapper;
using FluentValidation.Results;
using MediatR;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Application.Commnds
{
    public class EspacoCommandHandler : CommandHandler,
        IRequestHandler<EspacoCreateCommand, Espaco>,
        IRequestHandler<EspacoUpdateCommand, Espaco>,
        IRequestHandler<EspacoDeleteCommand, ValidationResult>
    {

        private readonly IEspacoRepository _repository;
        private readonly IMapper _mapper;

        public EspacoCommandHandler(IEspacoRepository repository, 
                                    IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Espaco> Handle(EspacoCreateCommand request, CancellationToken cancellationToken)
        {
            var objeto = _mapper.Map<Espaco>(request);
           
            if(!request.IsValid()) return null;

            var response = await _repository.AddAsync(objeto);

            response.ValidationResult = await Commit(_repository);

            if (!response.ValidationResult.IsValid) return response;

            response.AddDomainEvent(_mapper.Map<EspacoCreateNotification>(response));

            return response;
        }

        public async Task<Espaco> Handle(EspacoUpdateCommand request, CancellationToken cancellationToken)
        {
            var objeto = _mapper.Map<Espaco>(request);

            if (!request.IsValid()) return null;

            var response = await _repository.UpdateAsync(objeto, objeto.Id);

            response.ValidationResult = await Commit(_repository, "");

            if (!response.ValidationResult.IsValid) return response;

            response.AddDomainEvent(_mapper.Map<EspacoUpdateNotification>(response));

            return response;
        }

        public async Task<ValidationResult> Handle(EspacoDeleteCommand request, CancellationToken cancellationToken)
        {
           
            if (!request.IsValid()) return null;

            var objeto = await _repository.GetByIdAsync(request.Id);

            await _repository.DeleteAsync(objeto.Id);

            objeto.ValidationResult = await Commit(_repository, "");

            if (!objeto.ValidationResult.IsValid) return objeto.ValidationResult;

            objeto.AddDomainEvent(_mapper.Map<EspacoDeleteNotification>(objeto));

            return objeto.ValidationResult;
        }
    }
}
