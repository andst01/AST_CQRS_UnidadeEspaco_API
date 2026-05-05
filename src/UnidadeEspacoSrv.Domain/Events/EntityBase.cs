using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Domain.Events
{
    public abstract class EntityBase
    {
        private List<EventBase> _domainEvents = new(); // Inicialização moderna
        public IReadOnlyCollection<EventBase> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(EventBase domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        [NotMapped] // Importante se usar EF Core para não tentar criar coluna no SQL
        public ValidationResult ValidationResult { get; set; } = new();
    }
}
