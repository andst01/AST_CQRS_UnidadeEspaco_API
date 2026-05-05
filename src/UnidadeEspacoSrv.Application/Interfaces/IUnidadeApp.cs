using FluentValidation.Results;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Application.Interfaces
{
    public interface IUnidadeApp : IAppBase<Unidade,
                               UnidadeRequest,
                               UnidadeViewModel,
                               UnidadeCreateCommand,
                               UnidadeUpdateCommand,
                               UnidadeDeleteCommand,
                               UnidadeNotification>
    {

        Task<List<UnidadeViewModel>> ObterTodasUnidadeComEspaco();

        Task<UnidadeViewModel> ObterUnidadeComEspacoPorIdAsync(int id);

    }
}
