using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories.Base;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;

namespace API.Lanchonete.Data.Repositories
{
    public class PerfilRepository : EFRepositoryBase<Perfil>, IPerfilEFRepository
    {
        public PerfilRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Perfil> CadastrarPerfil(PerfilDto perfil)
        {
            var entity = new Perfil
            {
                Nome = perfil.Nome,
                Descricao = perfil.Descricao
            };

            await _context.Perfis.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
