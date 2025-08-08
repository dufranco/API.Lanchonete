using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Lanchonete.Data.Mappings.Map
{
    public class PerfilMap : IEntityTypeConfiguration<Perfil>
    {
        public void Configure(EntityTypeBuilder<Perfil> builder)
        {
            builder.HasKey(e => e.IdPerfil).HasName("perfis_pkey");

            builder.ToTable("perfis");

            builder.HasIndex(e => e.Nome, "perfis_nome_key").IsUnique();

            builder.Property(e => e.IdPerfil).HasColumnName("id_perfil");
            builder.Property(e => e.DataCadastro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_cadastro");
            builder.Property(e => e.Descricao).HasColumnName("descricao");
            builder.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("nome");
        }
    }
}
