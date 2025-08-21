using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories.Base;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Data.Repositories
{
    public class ItensPedidoEFRepository(AppDbContext context) : EFRepositoryBase<ItensPedido>(context), IItensPedidoEFRepository
    {
        public async Task<IEnumerable<ItensPedido>> CadastrarItensPedido(List<ItensPedido> itensPedido)
        {
            try
            {
                await _context.ItensPedido.AddRangeAsync(itensPedido);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("itens_pedido_id_produto_fkey"))
                    throw new DbUpdateException($"Não foi encontrado produto com pelo menos um dos seguintes ids: {string.Join(", ", itensPedido.Select(i => i.IdProduto))}.");

                throw;
            }

            return itensPedido;
        }

        public async Task AtualizarItensPedido(List<ItensPedido> itensPedido)
        {
            foreach (var item in itensPedido)
            {
                var entity = await _context.ItensPedido.FindAsync(item.IdItem) ?? throw new KeyNotFoundException($"Item {item.IdItem} do pedido {item.IdPedido} não encontrado.");

                entity.Quantidade = item.Quantidade;
                entity.Status = item.Status;
                entity.DataAtualizacao = DateTime.Now;

                _context.ItensPedido.Update(entity);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException!.Message.Contains("itens_pedido_id_pedido_fkey"))
                        throw new DbUpdateException($"Não foi encontrado pedido com id {item.IdPedido}.");

                    if (ex.InnerException!.Message.Contains("itens_pedido_id_produto_fkey"))
                        throw new DbUpdateException($"Não foi encontrado produto com id {item.IdProduto}.");

                    throw;
                }
            }
        }

        public async Task IncluirItemPedido(ItensPedido itemPedido)
        {
            _ = await _context.Pedidos.FindAsync(itemPedido.IdPedido) ?? throw new KeyNotFoundException($"O pedido {itemPedido.IdPedido} não foi encontrado.");
            var itemExistente = await _context.ItensPedido.AnyAsync(a => a.IdPedido == itemPedido.IdPedido && a.IdProduto == itemPedido.IdProduto);

            if (itemExistente)
                throw new InvalidOperationException($"O produto {itemPedido.IdProduto} já está incluído no pedido {itemPedido.IdPedido}.");

            await _context.ItensPedido.AddAsync(itemPedido);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("itens_pedido_id_produto_fkey"))
                    throw new DbUpdateException($"Não foi encontrado produto com id {itemPedido.IdProduto}.");

                throw;
            }
        }

        public async Task ExcluirItemPedido(int idItem, int idPedido)
        {
            var itemPedido = await _context.ItensPedido.FirstOrDefaultAsync(f => f.IdItem == idItem) ?? throw new KeyNotFoundException($"Item pedido {idItem} do pedido {idPedido} não encontrado.");
            _context.ItensPedido.Remove(itemPedido);

            await _context.SaveChangesAsync();
        }

        public async Task ExcluirItensPedido(int idPedido)
        {
            var itemPedido = await _context.ItensPedido.Where(w => w.IdPedido == idPedido).ToListAsync() ?? throw new KeyNotFoundException($"Nenhum item encontrado para o pedido {idPedido}.");
            _context.ItensPedido.RemoveRange(itemPedido);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirItensPedido(int idPedido, int idItemPedido)
        {
            var itemPedido = await _context.ItensPedido.Where(w => w.IdPedido == idPedido && w.IdItem == idItemPedido).FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Nenhum item encontrado para o pedido {idPedido}.");
            _context.ItensPedido.Remove(itemPedido);
            await _context.SaveChangesAsync();
        }
    }
}
