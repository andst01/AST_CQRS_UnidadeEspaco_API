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
    public class HistoricoEventoMap : IEntityTypeConfiguration<HistoricoEvento>
    {
        public void Configure(EntityTypeBuilder<HistoricoEvento> builder)
        {
            builder.ToTable("HistoricoEventos");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Codigo).IsRequired();
            builder.Property(e => e.NomeTabela).IsRequired().HasMaxLength(100);
            builder.Property(e => e.CodigoUsuario).IsRequired(false);
            builder.Property(e => e.DataCadastro).IsRequired();
            builder.Property(e => e.ValoresChaves).IsRequired();
            builder.Property(e => e.ValoresAntigos).IsRequired(false);
            builder.Property(e => e.ValoresNovos);
            builder.Property(e => e.TipoOperacao).IsRequired().HasMaxLength(50);
        }
    }
}