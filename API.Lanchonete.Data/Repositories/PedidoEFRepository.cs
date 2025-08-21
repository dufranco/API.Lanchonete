using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories.Base;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Data.Repositories
{
    public class PedidoEFRepository(AppDbContext context) : EFRepositoryBase<Pedido>(context), IPedidoEFRepository
    {
        public async Task<Pedido> CadastrarPedido(Pedido pedido)
        {
            await _context.Pedidos.AddRangeAsync(pedido);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("pedidos_id_usuario_fkey"))
                    throw new DbUpdateException($"Não foi encontrado usuário com id {pedido.IdUsuario}.");

                throw;
            }

            return pedido;
        }

        public async Task AtualizarPedido(Pedido pedido)
        {
            var entity = await _context.Pedidos.FindAsync(pedido.IdPedido) ?? throw new KeyNotFoundException($"Pedido {pedido.IdPedido} não encontrado.");

            entity.Status = pedido.Status;
            entity.DataAtualizacao = DateTime.Now;

            _context.Pedidos.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("pedidos_id_usuario_fkey"))
                    throw new DbUpdateException($"Não foi encontrado usuário com id {pedido.IdUsuario}.");

                if (ex.InnerException!.Message.Contains("pedidos_status_check"))
                    throw new DbUpdateException($"O status informado '{pedido.Status}' é inválido.");

                throw;
            }
        }

        public async Task ExcluirPedido(int idPedido)
        {
            var pedido = await _context.Pedidos.FirstOrDefaultAsync(f => f.IdPedido == idPedido) ?? throw new KeyNotFoundException($"O pedido {idPedido} não foi encontrado.");
            _context.Pedidos.Remove(pedido);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Pedido>> ListarPedidos(PedidoFiltroDto pedidoFiltro)
        {
            var query = _context.Pedidos
                                .Include(p => p.IdUsuarioNavigation)
                                .Include(p => p.ItensPedidos)
                                .ThenInclude(ip => ip.IdProdutoNavigation)
                                .AsQueryable();

            if (pedidoFiltro.IdPedido.HasValue)
                query = query.Where(p => p.IdPedido == pedidoFiltro.IdPedido.Value);

            if (pedidoFiltro.DataCadastroIni.HasValue)
                query = query.Where(p => p.DataCadastro >= pedidoFiltro.DataCadastroIni.Value);

            if (pedidoFiltro.DataCadastroFim.HasValue)
                query = query.Where(p => p.DataCadastro <= pedidoFiltro.DataCadastroFim.Value);

            if (!string.IsNullOrEmpty(pedidoFiltro.Status))
                query = query.Where(p => p.Status == pedidoFiltro.Status);

            if (pedidoFiltro.IdProduto.HasValue)
                query = query.Where(p => p.ItensPedidos.Any(i => i.IdProduto == pedidoFiltro.IdProduto.Value));

            query = pedidoFiltro.OrdenarPor!.Value switch
            {
                1 => pedidoFiltro.OrdemDesc ? query.OrderByDescending(p => p.IdPedido) : query.OrderBy(p => p.IdPedido),
                2 => pedidoFiltro.OrdemDesc ? query.OrderByDescending(p => p.DataCadastro) : query.OrderBy(p => p.DataCadastro),
                _ => query.OrderBy(p => p.IdPedido),
            };
            
            query = query.Skip((pedidoFiltro.Pagina - 1) * pedidoFiltro.TamanhoPagina).Take(pedidoFiltro.TamanhoPagina);

            return await query.ToListAsync();
        }

        public async Task<Pedido> ObterPedidoPorId(int idPedido)
            => await _context.Pedidos
                             .Include(p => p.IdUsuarioNavigation)
                             .Include(p => p.ItensPedidos)
                             .ThenInclude(ip => ip.IdProdutoNavigation)
                             .FirstOrDefaultAsync(p => p.IdPedido == idPedido) ?? throw new KeyNotFoundException($"Pedido {idPedido} não encontrado.");
    }
}
