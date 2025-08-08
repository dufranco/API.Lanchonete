using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Lanchonete.Data.Mappings.Map
{
    public class PedidoMap : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(e => e.IdPedido).HasName("pedidos_pkey");

            builder.ToTable("pedidos");

            builder.Property(e => e.IdPedido).HasColumnName("id_pedido");
            builder.Property(e => e.DataAtualizacao)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_atualizacao");
            builder.Property(e => e.DataCadastro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_cadastro");
            builder.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            builder.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendente'::character varying")
                .HasColumnName("status");

            builder.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedidos_id_usuario_fkey");
        }
    }
}
