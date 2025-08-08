using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Lanchonete.Data.Mappings.Map
{
    public class ItensPedidoMap : IEntityTypeConfiguration<ItensPedido>
    {
        public void Configure(EntityTypeBuilder<ItensPedido> builder)
        {
            builder.HasKey(e => e.IdItem).HasName("itens_pedido_pkey");

            builder.ToTable("itens_pedido");

            builder.Property(e => e.IdItem).HasColumnName("id_item");
            builder.Property(e => e.DataAtualizacao)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_atualizacao");
            builder.Property(e => e.DataCadastro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_cadastro");
            builder.Property(e => e.IdPedido).HasColumnName("id_pedido");
            builder.Property(e => e.IdProduto).HasColumnName("id_produto");
            builder.Property(e => e.Quantidade)
                .HasDefaultValue(1)
                .HasColumnName("quantidade");
            builder.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendente'::character varying")
                .HasColumnName("status");

            builder.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.ItensPedidos)
                .HasForeignKey(d => d.IdPedido)
                .HasConstraintName("itens_pedido_id_pedido_fkey");

            builder.HasOne(d => d.IdProdutoNavigation).WithMany(p => p.ItensPedidos)
                .HasForeignKey(d => d.IdProduto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("itens_pedido_id_produto_fkey");
        }
    }
}
