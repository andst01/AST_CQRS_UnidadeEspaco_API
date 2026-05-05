using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Application.Interfaces
{
    public interface IEspacoApp : IAppBase<Espaco,
                    EspacoRequest,
                    EspacoViewModel,
                    EspacoCreateCommand,
                    EspacoUpdateCommand,
                    EspacoDeleteCommand,
                    EspacoNotification>
    {
        Task<List<EspacoViewModel>> ObterEspacosComUnidadesAsync();
    }
}
