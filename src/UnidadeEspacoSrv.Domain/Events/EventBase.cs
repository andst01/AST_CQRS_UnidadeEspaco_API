using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Domain.Events
{
    public class EventBase : INotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        // [BsonElement("id")]

        public ObjectId _Id { get; set; }
        public virtual int Id { get; set; }
    }
}
