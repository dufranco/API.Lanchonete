using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Lanchonete.Data.Mappings.Map
{
    public class ControleAcessoMap : IEntityTypeConfiguration<ControleAcesso>
    {
        public void Configure(EntityTypeBuilder<ControleAcesso> builder)
        {
            builder.HasKey(e => e.IdControle).HasName("controle_acesso_pkey");

            builder.ToTable("controle_acesso");

            builder.Property(e => e.IdControle).HasColumnName("id_controle");
            builder.Property(e => e.DataCadastro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_cadastro");
            builder.Property(e => e.IdPerfil).HasColumnName("id_perfil");
            builder.Property(e => e.NomeTela)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nome_tela");
            builder.Property(e => e.Permitido)
                .HasDefaultValue(true)
                .HasColumnName("permitido");

            builder.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.ControleAcessos)
                .HasForeignKey(d => d.IdPerfil)
                .HasConstraintName("controle_acesso_id_perfil_fkey");
        }
    }
}
