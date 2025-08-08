using API.Lanchonete.Data.Mappings.Map;
using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace API.Lanchonete.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ControleAcessoMap());
            modelBuilder.ApplyConfiguration(new ItensPedidoMap());
            modelBuilder.ApplyConfiguration(new PedidoMap());
            modelBuilder.ApplyConfiguration(new PerfilMap());
            modelBuilder.ApplyConfiguration(new ProdutoMap());
            modelBuilder.ApplyConfiguration(new UsuarioMap());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.LogTo(message => Debug.WriteLine(message));

        public virtual DbSet<ControleAcesso> ControleAcesso { get; set; }
        public virtual DbSet<ItensPedido> ItensPedido { get; set; }
        public virtual DbSet<Pedido> Pedidos { get; set; }
        public virtual DbSet<Perfil> Perfis { get; set; }
        public virtual DbSet<Produto> Produtos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
    }
}
