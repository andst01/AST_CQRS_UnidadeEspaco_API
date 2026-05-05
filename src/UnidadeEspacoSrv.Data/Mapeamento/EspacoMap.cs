using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Data.Mapeamento
{

    /// <summary>
    /// EspacoMap é a classe de configuração para a entidade Espaco, implementando a interface IEntityTypeConfiguration<Espaco>.
    /// </summary>
    public class EspacoMap : IEntityTypeConfiguration<Espaco>
    {
        public void Configure(EntityTypeBuilder<Espaco> builder)
        {
            builder.ToTable("Espaco");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Endereco).HasMaxLength(500);

            builder.HasMany(e => e.Unidades)
                   .WithOne(u => u.Espaco)
                   .HasForeignKey(u => u.IdEspaco)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
