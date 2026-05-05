using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Domain.Events
{
    [BsonIgnoreExtraElements]
    public class EspacoNotification : EventBase
    {
        [BsonElement("Nome")]
        public string Nome { get; set; }
        
        [BsonElement("Endereco")]
        public string Endereco { get; set; }

        [BsonElement("Unidades")]
        public virtual IEnumerable<UnidadeNotification> Unidades { get; set; }
    }

    public class EspacoCreateNotification : EspacoNotification { }
    public class EspacoUpdateNotification : EspacoNotification { }
    public class EspacoDeleteNotification : EspacoNotification { }

    public class EspacoReadModel : EspacoNotification 
    { 

    }


}
