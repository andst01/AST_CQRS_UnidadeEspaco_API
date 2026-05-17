using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Entities;

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

        protected static T MapFrom<T>(Espaco espaco) where T : EspacoNotification, new()
        {
            return new T
            {
                Id = espaco.Id,
                Nome = espaco.Nome,
                Endereco = espaco.Endereco
            };
        }
    }

    public class EspacoCreateNotification : EspacoNotification 
    { 
        public static implicit operator EspacoCreateNotification(Espaco espaco)
        {
            return MapFrom<EspacoCreateNotification>(espaco);
        }
    }
    public class EspacoUpdateNotification : EspacoNotification 
    {
        public static implicit operator EspacoUpdateNotification(Espaco espaco)
        {
            return MapFrom<EspacoUpdateNotification>(espaco);
        }
    }
    
    public class EspacoDeleteNotification : EspacoNotification 
    {
        public static implicit operator EspacoDeleteNotification(Espaco espaco)
        {
            return MapFrom<EspacoDeleteNotification>(espaco);
        }
    }

    public class EspacoReadModel : EspacoNotification 
    { 

    }


}
