using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Lanchonete.Data.Mappings.Map
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(e => e.IdUsuario).HasName("usuarios_pkey");

            builder.ToTable("usuarios");

            builder.HasIndex(e => e.Email, "usuarios_email_key").IsUnique();

            builder.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            builder.Property(e => e.DataAtualizacao)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_atualizacao");
            builder.Property(e => e.DataCadastro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_cadastro");
            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("email");
            builder.Property(e => e.IdPerfil).HasColumnName("id_perfil");
            builder.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nome");
            builder.Property(e => e.SenhaHash)
                .IsRequired()
                .HasColumnName("senha_hash");
            builder.Property(e => e.SenhaSalt)
                .IsRequired()
                .HasColumnName("senha_salt");

            builder.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuarios_id_perfil_fkey");
        }
    }
}
