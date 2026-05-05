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
    /// UnidadeMap é a classe de configuração para a entidade Unidade, definindo como ela será mapeada para o banco de dados usando Entity Framework Core. Ela especifica o nome da tabela, as chaves primárias e as propriedades da entidade, garantindo que os dados sejam armazenados corretamente no banco de dados.
    /// </summary>
    public class UnidadeMap : IEntityTypeConfiguration<Unidade>
    {
        public void Configure(EntityTypeBuilder<Unidade> builder)
        {
            builder.ToTable("Unidade");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Rede).IsRequired().HasMaxLength(100);
            builder.Property(u => u.IdEspaco).HasMaxLength(500);


        }
    }
}
