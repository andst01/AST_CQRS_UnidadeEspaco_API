using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Application.Interfaces
{
    public interface INotificationMapper<TNotification>
    {
       void MapFromNotification(TNotification notification);
    }
}
