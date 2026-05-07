using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Data.Contexto
{
    /// <summary>
    /// SqlDbContext: Contexto do Entity Framework para a aplicação, responsável por gerenciar as entidades "Unidade" e "Espaco" e suas configurações de mapeamento.
    /// </summary>
    public class SQLDbContext : DbContext
    {
        public SQLDbContext(DbContextOptions<SQLDbContext> options) : base(options)
        {
        }

        public DbSet<Unidade> Unidades { get; set; }
        public DbSet<Espaco> Espacos { get; set; }
        public DbSet<HistoricoEvento> HistoricoEventos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<EventBase>();

            modelBuilder.ApplyConfiguration(new Mapeamento.UnidadeMap());
            modelBuilder.ApplyConfiguration(new Mapeamento.EspacoMap());
            modelBuilder.ApplyConfiguration(new Mapeamento.HistoricoEventoMap());

            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> Commit()
        {

            var listaHistoricoEntry = await OnBeforePublishEvent();
            var saveChanges = await this.SaveChangesAsync();

            if (saveChanges == 0) return false;

            await OnAfterSaveChangesAsync(listaHistoricoEntry);


            return saveChanges > 1;
        }


        private Task OnAfterSaveChangesAsync(Tuple<List<HistoricoEvento>, List<HistoricoEventoEntry>> tuplaHistoricos)
        {
            if (tuplaHistoricos.Item2 == null || tuplaHistoricos.Item2.Count == 0)
                return Task.CompletedTask;



            // For each temporary property in each audit entry - update the value in the audit entry to the actual (generated) value
            foreach (var entry in tuplaHistoricos.Item2)
            {
                var entidade = tuplaHistoricos.Item1
                        .Where(x => x.NomeTabela.ToUpper() == entry.NomeTabela.ToUpper() && x.TipoOperacao == "I")
                        .FirstOrDefault();



                if (entidade != null)
                {
                    var updateHistorico = new HistoricoEvento();

                    // ultimo teste
                    var objeto = this.Set<HistoricoEvento>().Find(entidade.Id);

                    // ultimo teste
                    this.Set<HistoricoEvento>().Attach(objeto);

                    int Id = entidade.Id;

                    foreach (var prop in entry.TemporaryProperties)
                    {

                        string propertyName = prop.Metadata.GetColumnName();

                        if (prop.Metadata.IsKey())
                        {
                            // deu certo
                            entry.ValoresChaves[propertyName] = prop.CurrentValue;
                        }

                        // deu certo
                        //entidade.ValoresChaves = Newtonsoft.Json.JsonConvert.SerializeObject(entry.ValoresChaves);

                        EntityEntry entityEntry = this.Entry(objeto);
                        entityEntry.Properties.Where(x => x.Metadata.IsKey());
                        // objeto = entry.ToHistoricoEventoUpdate(Id);

                        updateHistorico = entry.ToHistoricoEventoUpdate(Id);


                    }

                    var entryUpdate = this.Entry(objeto);
                    foreach (var property in entryUpdate.CurrentValues.Properties)
                    {
                        // Só copia o valor se não for a chave primária
                        if (!property.IsPrimaryKey())
                        {
                            entryUpdate.CurrentValues[property] = entryUpdate.Property(property.Name).CurrentValue;
                            // Ou use o valor vindo do seu objeto de update
                        }
                    }
                    //this.Entry(objeto).CurrentValues.SetValues(updateHistorico);
                    //this.Entry(objeto).State = EntityState.Modified;
                   


                    // deu certo
                    //this.Set<HistoricoEvento>().Update(entidade);


                    this.SaveChanges();
                }

            }

            return Task.CompletedTask;
        }

        private async Task<Tuple<List<HistoricoEvento>, List<HistoricoEventoEntry>>> OnBeforePublishEvent()
        {

            var historicoEventosEntry = new List<HistoricoEventoEntry>();
            var historicoEventos = new List<HistoricoEvento>();

            this.ChangeTracker.DetectChanges();

            foreach (var entry in this.ChangeTracker.Entries())
            {

                //bool teste = true;
                if (entry.Entity is HistoricoEvento || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new HistoricoEventoEntry(entry);

                //entry.Members
                auditEntry.NomeTabela = entry.Metadata.GetTableName(); //.Relational().TableName; // EF Core 3.1: entry.Metadata.GetTableName();
                historicoEventosEntry.Add(auditEntry);

                foreach (var property in entry.Properties)
                {

                    auditEntry.TemporaryProperties = entry.Properties.Where(p => p.IsTemporary).ToList();

                    string propertyName = property.Metadata.GetColumnName();

                    if (property.Metadata.IsKey())
                    {
                        //auditEntry.ValoresAntigos[]
                        if (entry.State == EntityState.Added)
                            auditEntry.ValoresChaves[propertyName] = 0;
                        else
                            auditEntry.ValoresChaves[propertyName] = property.CurrentValue;
                        continue;
                    }


                    //auditEntry.CodigoUsuario = int.Parse(.Claims.FirstOrDefault(x => x.Type == "codigo_usuario")?.Value);
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.ValoresNovos[propertyName] = property.CurrentValue;
                            auditEntry.ValoresAntigos[propertyName] = " ";
                            auditEntry.TipoOperacao = "I";
                            break;

                        case EntityState.Deleted:
                            auditEntry.ValoresAntigos[propertyName] = property.OriginalValue;
                            auditEntry.ValoresNovos[propertyName] = " ";
                            auditEntry.TipoOperacao = "E";
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ValoresAntigos[propertyName] = property.OriginalValue;
                                auditEntry.ValoresNovos[propertyName] = property.CurrentValue;
                                auditEntry.TipoOperacao = "A";
                            }
                            break;
                    }
                }


            }

            foreach (var auditEntry in historicoEventosEntry)
            {
                var historicoEvento = auditEntry.ToHistoricoEvento();

                historicoEventos.Add(historicoEvento);
                HistoricoEventos.Add(historicoEvento);
            }


            var response = new Tuple<List<HistoricoEvento>, List<HistoricoEventoEntry>>(historicoEventos, historicoEventosEntry);

            return response;
        }

    }
}
