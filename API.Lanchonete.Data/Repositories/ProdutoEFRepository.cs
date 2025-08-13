using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories.Base;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Data.Repositories
{
    public class ProdutoEFRepository(AppDbContext context) : EFRepositoryBase<Produto>(context), IProdutoEFRepository
    {
        private const string _erroCheckTipoProduto = "Só são aceitos produtos com os tipos 'prato' ou 'bebida'.";
        private const string _erroProdutoNaoEncontrado = "Produto não encontrado.";

        public async Task<Produto> CadastrarProduto(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("produtos_tipo_check"))
                    throw new DbUpdateException(_erroCheckTipoProduto);

                throw;
            }

            return produto;
        }

        public async Task AtualizarProduto(Produto produto)
        {
            var entity = await _context.Produtos.FindAsync(produto.IdProduto) ?? throw new KeyNotFoundException(_erroProdutoNaoEncontrado);

            entity.Nome = produto.Nome;
            entity.Descricao = produto.Descricao;
            entity.Ativo = produto.Ativo;
            entity.Preco = produto.Preco;
            entity.Tipo = produto.Tipo;
            entity.DataAtualizacao = DateTime.Now;

            _context.Produtos.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!.Message.Contains("produtos_tipo_check"))
                    throw new DbUpdateException(_erroCheckTipoProduto);

                throw;
            }
        }

        public async Task ExcluirProduto(int idProduto)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(f => f.IdProduto == idProduto) ?? throw new KeyNotFoundException(_erroProdutoNaoEncontrado);
            var hasAssociation = await _context.ItensPedido.AnyAsync(a => a.IdProduto == idProduto);

            if (hasAssociation)
                throw new InvalidOperationException("Não é possível excluir o produto, pois está associado a um ou mais pedidos.");

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<Produto> ObterProdutoPorId(int idProduto)
            => await _context.Produtos.Where(p => p.IdProduto == idProduto).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(_erroProdutoNaoEncontrado);

        public async Task<IEnumerable<Produto>> ListarProdutos(ProdutoFiltroDto produtoFiltro)
        {
            var query = _context.Produtos.AsQueryable();

            if (produtoFiltro.IdProduto.HasValue && produtoFiltro.IdProduto > 0)
                query = query.Where(p => p.IdProduto.ToString().Contains(produtoFiltro.IdProduto.Value.ToString()));

            if (!string.IsNullOrWhiteSpace(produtoFiltro.Nome))
                query = query.Where(p => p.Nome.ToLower().Contains(produtoFiltro.Nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(produtoFiltro.Descricao))
                query = query.Where(p => p.Descricao.ToLower().Contains(produtoFiltro.Descricao.ToLower()));

            if (!string.IsNullOrWhiteSpace(produtoFiltro.Tipo))
                query = query.Where(p => p.Tipo.ToLower().Contains(produtoFiltro.Tipo.ToLower()));

            if (produtoFiltro.Ativo.HasValue)
                query = query.Where(p => p.Ativo == produtoFiltro.Ativo);

            query = produtoFiltro.OrdenarPor switch
            {
                1 => produtoFiltro.OrdemDesc ? query.OrderByDescending(p => p.Nome) : query.OrderBy(p => p.Nome),
                2 => produtoFiltro.OrdemDesc ? query.OrderByDescending(p => p.Descricao) : query.OrderBy(p => p.Descricao),
                3 => produtoFiltro.OrdemDesc ? query.OrderByDescending(p => p.Tipo) : query.OrderBy(p => p.Tipo),
                _ => produtoFiltro.OrdemDesc ? query.OrderByDescending(p => p.IdProduto) : query.OrderBy(p => p.IdProduto),
            };

            var result = await query.Skip((produtoFiltro.Pagina - 1) * produtoFiltro.TamanhoPagina).Take(produtoFiltro.TamanhoPagina).ToListAsync();

            return result.Count != 0 ? result : throw new KeyNotFoundException("Nenhum produto encontrado.");
        }
    }
}
