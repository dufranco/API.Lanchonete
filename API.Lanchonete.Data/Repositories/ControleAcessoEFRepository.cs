using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories.Base;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Data.Repositories
{
    public class ControleAcessoEFRepository(AppDbContext context) : EFRepositoryBase<ControleAcesso>(context), IControleAcessoEFRepository
    {
        public async Task<ControleAcesso> CadastrarControleAcesso(ControleAcesso controleAcesso)
        {
            await _context.ControleAcesso.AddAsync(controleAcesso);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("controle_acesso_id_perfil_fkey"))
                    throw new DbUpdateException($"Não foi encontrado perfil com id {controleAcesso.IdPerfil}.");

                throw;
            }

            return controleAcesso;
        }

        public async Task AtualizarControleAcesso(ControleAcesso controleAcesso)
        {
            var entity = await _context.ControleAcesso.FindAsync(controleAcesso.IdControle) ?? throw new KeyNotFoundException($"Controle de acesso {controleAcesso.IdControle} não encontrado.");

            entity.IdPerfil = controleAcesso.IdPerfil;
            entity.NomeTela = controleAcesso.NomeTela;
            entity.Permitido = controleAcesso.Permitido;

            _context.ControleAcesso.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("controle_acesso_pkey"))
                    throw new DbUpdateException($"Não foi encontrado controle de acesso com id {controleAcesso.IdControle}.");

                if (ex.InnerException!.Message.Contains("controle_acesso_id_perfil_fkey"))
                    throw new DbUpdateException($"Não foi encontrado perfil com id {controleAcesso.IdPerfil}.");

                throw;
            }
        }

        public async Task ExcluirControleAcesso(int idControleAcesso)
        {
            var controleAcesso = await _context.ControleAcesso.FirstOrDefaultAsync(f => f.IdControle == idControleAcesso) ?? throw new KeyNotFoundException($"O controle de acesso {idControleAcesso} não foi encontrado.");
            _context.ControleAcesso.Remove(controleAcesso);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ControleAcesso>> ListarControleAcessos(ControleAcessoFiltroDto controleAcessoFiltro)
        {
            var query = _context.ControleAcesso
                                .Include(p => p.IdPerfilNavigation)
                                .AsQueryable();

            if (controleAcessoFiltro.IdControle.HasValue)
                query = query.Where(p => p.IdControle == controleAcessoFiltro.IdControle.Value);

            if (controleAcessoFiltro.DataCadastroIni.HasValue)
                query = query.Where(p => p.DataCadastro >= controleAcessoFiltro.DataCadastroIni.Value);

            if (controleAcessoFiltro.DataCadastroFim.HasValue)
                query = query.Where(p => p.DataCadastro <= controleAcessoFiltro.DataCadastroFim.Value);

            if (controleAcessoFiltro.IdPerfil.HasValue)
                query = query.Where(p => p.IdPerfil == controleAcessoFiltro.IdPerfil.Value);

            if (!string.IsNullOrEmpty(controleAcessoFiltro.NomePerfil?.Trim()))
                query = query.Where(p => p.IdPerfilNavigation.Nome.ToLower().Contains(controleAcessoFiltro.NomePerfil.ToLower()));

            query = controleAcessoFiltro.OrdenarPor!.Value switch
            {
                1 => controleAcessoFiltro.OrdemDesc ? query.OrderByDescending(p => p.IdControle) : query.OrderBy(p => p.IdControle),
                2 => controleAcessoFiltro.OrdemDesc ? query.OrderByDescending(p => p.DataCadastro) : query.OrderBy(p => p.DataCadastro),
                _ => query.OrderBy(p => p.IdControle),
            };
            
            query = query.Skip((controleAcessoFiltro.Pagina - 1) * controleAcessoFiltro.TamanhoPagina).Take(controleAcessoFiltro.TamanhoPagina);

            return await query.ToListAsync();
        }

        public async Task<ControleAcesso> ObterControleAcessoPorId(int idControleAcesso)
            => await _context.ControleAcesso
                             .Include(p => p.IdPerfilNavigation)
                             .FirstOrDefaultAsync(p => p.IdControle == idControleAcesso) ?? throw new KeyNotFoundException($"ControleAcesso {idControleAcesso} não encontrado.");
    }
}
