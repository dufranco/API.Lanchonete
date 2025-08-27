using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories.Base;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Data.Repositories
{
    public class UsuarioEFRepository(AppDbContext context) : EFRepositoryBase<Usuario>(context), IUsuarioEFRepository
    {
        public async Task<Usuario> CadastrarUsuario(Usuario usuario)
        {
            try
            {
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("usuarios_email_key"))
                    throw new DbUpdateException($"Já existe um usuário com o e-mail {usuario.Email}.");

                if (ex.InnerException!.Message.Contains("usuarios_id_perfil_fkey"))
                    throw new DbUpdateException($"Perfil com código {usuario.IdPerfil} não encontrado.");

                throw;
            }

            return usuario;
        }

        public async Task AtualizarUsuario(Usuario usuario)
        {
            var entity = await _context.Usuarios.FindAsync(usuario.IdUsuario) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            entity.Nome = usuario.Nome;
            entity.Email = usuario.Email;
            entity.IdPerfil = usuario.IdPerfil;
            entity.DataAtualizacao = DateTime.Now;

            if (!string.IsNullOrEmpty(usuario.SenhaHash))
                entity.SenhaHash = usuario.SenhaHash;

            if (!string.IsNullOrEmpty(usuario.SenhaSalt))
                entity.SenhaSalt = usuario.SenhaSalt;

            _context.Usuarios.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("usuarios_email_key"))
                    throw new DbUpdateException($"Já existe um usuário com o e-mail {usuario.Email}.");

                throw;
            }
        }

        public async Task ExcluirUsuario(int idUsuario)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(f => f.IdUsuario == idUsuario) ?? throw new KeyNotFoundException("Usuário não encontrado.");
            var hasAssociation = await _context.Pedidos.AnyAsync(a => a.IdUsuario == idUsuario);

            if (hasAssociation)
                throw new InvalidOperationException("Não é possível excluir o usuário, pois está associado a um ou mais pedidos.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> ObterUsuarioPorId(int idUsuario)
            => await _context.Usuarios.Where(p => p.IdUsuario == idUsuario).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Usuário não encontrado.");

        public async Task<IEnumerable<Usuario>> ListarUsuarios(UsuarioFiltroDto usuarioFiltro)
        {
            var query = _context.Usuarios
                            .Include(p => p.IdPerfilNavigation)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(usuarioFiltro.Nome))
                query = query.Where(p => p.Nome.ToLower().Contains(usuarioFiltro.Nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(usuarioFiltro.Email))
                query = query.Where(p => p.Email.ToLower().Contains(usuarioFiltro.Email.ToLower()));

            if (!string.IsNullOrWhiteSpace(usuarioFiltro.DescricaoPerfil))
                query = query.Where(p => p.IdPerfilNavigation.Descricao.ToLower().Contains(usuarioFiltro.DescricaoPerfil.ToLower()));

            query = usuarioFiltro.OrdenarPor switch
            {
                1 => usuarioFiltro.OrdemDesc ? query.OrderByDescending(p => p.Nome) : query.OrderBy(p => p.Nome),
                2 => usuarioFiltro.OrdemDesc ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                3 => usuarioFiltro.OrdemDesc ? query.OrderByDescending(p => p.IdPerfilNavigation.Descricao) : query.OrderBy(p => p.IdPerfilNavigation.Descricao),
                _ => usuarioFiltro.OrdemDesc ? query.OrderByDescending(p => p.IdUsuario) : query.OrderBy(p => p.IdUsuario),
            };

            var result = await query.Skip((usuarioFiltro.Pagina - 1) * usuarioFiltro.TamanhoPagina).Take(usuarioFiltro.TamanhoPagina).ToListAsync();

            return result.Count != 0 ? result : throw new KeyNotFoundException("Nenhum usuário encontrado.");
        }

        public async Task<Usuario?> ObterUsuarioPorEmail(string email)
            => await _context.Usuarios.Include(p => p.IdPerfilNavigation).Where(p => p.Email == email).FirstOrDefaultAsync();
    }
}
