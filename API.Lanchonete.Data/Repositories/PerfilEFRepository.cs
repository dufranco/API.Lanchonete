using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories.Base;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Data.Repositories
{
    public class PerfilEFRepository(AppDbContext context) : EFRepositoryBase<Perfil>(context), IPerfilEFRepository
    {
        public async Task<Perfil> CadastrarPerfil(PerfilDto perfil)
        {
            var entity = new Perfil
            {
                Nome = perfil.Nome,
                Descricao = perfil.Descricao
            };

            try
            {
                await _context.Perfis.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new DbUpdateException($"Já existe um perfil com o nome {perfil.Nome}.");
            }

            return entity;
        }

        public async Task AtualizarPerfil(PerfilDto perfil)
        {
            var entity = await _context.Perfis.FindAsync(perfil.IdPerfil!.Value) ?? throw new KeyNotFoundException("Perfil não encontrado.");

            entity.Nome = perfil.Nome;
            entity.Descricao = perfil.Descricao;

            _context.Perfis.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("perfis_nome_key"))
                    throw new DbUpdateException($"Já existe um perfil com o nome {perfil.Nome}.");

                throw;
            }
        }

        public async Task ExcluirPerfil(int idPerfil)
        {
            var perfil = await _context.Perfis.FirstOrDefaultAsync(f => f.IdPerfil == idPerfil) ?? throw new KeyNotFoundException("Perfil não encontrado.");
            var hasAssociations = await _context.Usuarios.AnyAsync(a => a.IdPerfil == idPerfil) || await _context.ControleAcesso.AnyAsync(a => a.IdPerfil == idPerfil);

            if (hasAssociations)
                throw new InvalidOperationException("Não é possível excluir o perfil, pois ele está associado a usuários ou controle de acesso.");

            _context.Perfis.Remove(perfil);
            await _context.SaveChangesAsync();
        }

        public async Task<Perfil?> ObterPerfilPorId(int idPerfil)
            => await _context.Perfis.FirstOrDefaultAsync(f => f.IdPerfil == idPerfil) ?? throw new KeyNotFoundException("Perfil não encontrado.");

        public async Task<IEnumerable<Perfil>> ListarPerfis(PerfilFiltroDto perfilFiltro)
        {
            var query = _context.Perfis.AsQueryable();

            if (!string.IsNullOrWhiteSpace(perfilFiltro.Nome))
                query = query.Where(p => p.Nome.ToLower().Contains($"{perfilFiltro.Nome}"));

            if (!string.IsNullOrWhiteSpace(perfilFiltro.Descricao))
                query = query.Where(p => p.Descricao.ToLower().Contains($"{perfilFiltro.Descricao}"));

            query = perfilFiltro.OrdenarPor switch
            {
                1 => perfilFiltro.OrdemDesc ? query.OrderByDescending(p => p.Nome) : query.OrderBy(p => p.Nome),
                2 => perfilFiltro.OrdemDesc ? query.OrderByDescending(p => p.Descricao) : query.OrderBy(p => p.Descricao),
                _ => perfilFiltro.OrdemDesc ? query.OrderByDescending(p => p.IdPerfil) : query.OrderBy(p => p.IdPerfil),
            };

            return await query.Skip((perfilFiltro.Pagina - 1) * perfilFiltro.TamanhoPagina).Take(perfilFiltro.TamanhoPagina).ToListAsync();
        }
    }
}
