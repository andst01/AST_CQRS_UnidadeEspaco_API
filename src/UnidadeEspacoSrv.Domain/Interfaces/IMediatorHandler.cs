using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Domain.Interfaces
{
    public interface IMediatorHandler
    {
        Task<bool?> PublishEvent<T>(T @event) where T : EventBase;

        Task<TResponse> SendCommand<TRequest, TResponse>(TRequest command) where TRequest : IRequest<TResponse>;

        Task<ValidationResult> SendCommand<T>(T command) where T : IRequest<ValidationResult>;

        Task PublishEvent();
    }
}
