using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Lanchonete.Data.Mappings.Map
{
    public class ProdutoMap : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(e => e.IdProduto).HasName("produtos_pkey");

            builder.ToTable("produtos");

            builder.Property(e => e.IdProduto).HasColumnName("id_produto");
            builder.Property(e => e.Ativo)
                .HasDefaultValue(true)
                .HasColumnName("ativo");
            builder.Property(e => e.DataAtualizacao)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_atualizacao");
            builder.Property(e => e.DataCadastro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_cadastro");
            builder.Property(e => e.Descricao).HasColumnName("descricao");
            builder.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nome");
            builder.Property(e => e.Preco)
                .HasPrecision(10, 2)
                .HasColumnName("preco");
            builder.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        }
    }
}
