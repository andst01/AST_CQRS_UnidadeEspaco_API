using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Domain.Events
{
    [BsonIgnoreExtraElements]
    public class UnidadeNotification : EventBase
    {
        [BsonElement("IdEspaco")]
        public int IdEspaco { get; set; }

        [BsonElement("Rede")]
        public string Rede { get; set; }

        [BsonElement("Espaco")]
        public virtual EspacoNotification Espaco { get; set; }
    }

    public class UnidadeCreateNotification : UnidadeNotification { }
    public class UnidadeUpdateNotification : UnidadeNotification { }
    public class UnidadeDeleteNotification : UnidadeNotification { }

    public class UnidadeReadModel : UnidadeNotification
    {
        
    }
}
