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
    public class UnidadeNotification : EventBase
    {
        [BsonElement("IdEspaco")]
        public int IdEspaco { get; set; }

        [BsonElement("Rede")]
        public string Rede { get; set; }

        [BsonElement("Espaco")]
        public virtual EspacoNotification Espaco { get; set; }

        protected static T MapFrom<T>(Unidade unidade) where T : UnidadeNotification, new()
        {
            return new T
            {
                Id = unidade.Id,
                IdEspaco = unidade.IdEspaco,
                Rede = unidade.Rede
            };
        }
    }

    public class UnidadeCreateNotification : UnidadeNotification 
    {
        public static implicit operator UnidadeCreateNotification(Unidade unidade)
        {
            return MapFrom<UnidadeCreateNotification>(unidade);
        }
    }
    public class UnidadeUpdateNotification : UnidadeNotification 
    {
        public static implicit operator UnidadeUpdateNotification(Unidade unidade)
        {
            return MapFrom<UnidadeUpdateNotification>(unidade);
        }

    }
    
    public class UnidadeDeleteNotification : UnidadeNotification 
    {
        public static implicit operator UnidadeDeleteNotification(Unidade unidade)
        {
            return MapFrom<UnidadeDeleteNotification>(unidade);
        }
    }

    public class UnidadeReadModel : UnidadeNotification
    {
        
    }
}
