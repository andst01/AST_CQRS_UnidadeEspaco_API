using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Application.Interfaces
{
    public interface IRequestToCommand<TRequest, TCommand>
    {
        TCommand MapFromRequest(TRequest request);
    }

   
}
