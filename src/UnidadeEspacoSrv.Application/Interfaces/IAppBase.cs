using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Application.Interfaces
{
    public interface IAppBase<TEntity, 
                              TRequest, 
                              TViewModel,
                              TCommandCreate, 
                              TCommandUpdate,
                              TCommandDelete,
                              TNotification>
        where TEntity : EntityBase
        where TRequest : class
        where TViewModel : class
        where TCommandCreate : BaseCommand<TEntity>
        where TCommandUpdate : BaseCommand<TEntity>
        where TCommandDelete : BaseCommand<ValidationResult>
        where TNotification : EventBase

    {
        Task<TViewModel> AdicionarAsync(TRequest request);

        Task<TViewModel> AtualizarAsync(TRequest request);

        Task<ValidationResult> ExcluirAsync(int id);

        Task<TViewModel> ObterPorIdAsync(int id);

        Task<List<TViewModel>> ObterTodosAsync();
    }
    

}
