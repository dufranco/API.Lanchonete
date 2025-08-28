using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Lanchonete.Business.Business
{
    public class PedidoBusiness(ILogger<PedidoBusiness> logger, IMapper mapper, IPedidoEFRepository pedidoEFRepository, IItensPedidoEFRepository itensPedidoEFRepository) : IPedidoBusiness
    {
        private readonly ILogger<PedidoBusiness> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IPedidoEFRepository _pedidoEFRepository = pedidoEFRepository ?? throw new ArgumentNullException(nameof(pedidoEFRepository));
        private readonly IItensPedidoEFRepository _itensPedidoEFRepository = itensPedidoEFRepository ?? throw new ArgumentNullException(nameof(itensPedidoEFRepository));

        public async Task<PedidoCadastroResponseDto> CadastrarPedido(PedidoCadastroRequestDto pedido)
        {
            var transaction = await _pedidoEFRepository.BeginTransactionAsync();

            try
            {
                var pedidoResult = await _pedidoEFRepository.CadastrarPedido(_mapper.Map<Pedido>(pedido));
                var itensResult = await _itensPedidoEFRepository.CadastrarItensPedido(PedidoCadastroRequestDtoToItensPedidoList(pedido.ItensPedido, pedidoResult.IdPedido));

                await transaction.CommitAsync();

                return new()
                {
                    IdPedido = pedidoResult.IdPedido,
                    IdUsuario = pedidoResult.IdUsuario,
                    Status = pedidoResult.Status,
                    ItensPedido = itensResult.Select(i => new ItemPedidoCadastroResponseDto
                    {
                        IdItem = i.IdItem,
                        IdProduto = i.IdProduto,
                        Quantidade = i.Quantidade,
                        Status = i.Status,
                    }).ToList()
                };
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao cadastrar pedido.", ex);
            }
        }

        public async Task AtualizarPedido(PedidoAlteracaoRequestDto pedido)
        {
            var transaction = await _pedidoEFRepository.BeginTransactionAsync();

            try
            {
                await _pedidoEFRepository.AtualizarPedido(_mapper.Map<Pedido>(pedido));
                await _itensPedidoEFRepository.AtualizarItensPedido(PedidoAlteracaoRequestDtoToItensPedidoList(pedido));
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao atualizar pedido.", ex);
            }
        }

        public async Task<ItemPedidoCadastroResponseDto> IncluirItemPedido(ItemPedidoRequestDto itemPedido)
        {
            var transaction = await _pedidoEFRepository.BeginTransactionAsync();
            var result = (ItemPedidoCadastroResponseDto)null;

            try
            {
                result = _mapper.Map<ItemPedidoCadastroResponseDto>(await _itensPedidoEFRepository.IncluirItemPedido(_mapper.Map<ItensPedido>(itemPedido)));
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao incluir o produto {itemPedido.IdProduto} no pedido {itemPedido.IdPedido}.", ex);
            }

            return result;
        }

        public async Task ExcluirItemPedido(int idItem, int idPedido)
        {
            var transaction = await _pedidoEFRepository.BeginTransactionAsync();

            try
            {
                await _itensPedidoEFRepository.ExcluirItemPedido(idItem, idPedido);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao excluir item {idItem} do pedido {idPedido}.", ex);
            }
        }

        public async Task ExcluirPedido(int idPedido)
        {
            var transaction = await _pedidoEFRepository.BeginTransactionAsync();

            try
            {
                await _itensPedidoEFRepository.ExcluirItensPedido(idPedido);
                await _pedidoEFRepository.ExcluirPedido(idPedido);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao excluir pedido.", ex);
            }
        }

        public async Task<IEnumerable<PedidoResponseDto>> ListarPedidos(PedidoFiltroDto pedidoFiltro)
            => _mapper.Map<IEnumerable<PedidoResponseDto>>(await _pedidoEFRepository.ListarPedidos(pedidoFiltro));

        public async Task<PedidoResponseDto> ObterPedidoPorId(int idPedido)
        {
            if (idPedido <= 0)
                throw new ArgumentException("O ID do usuário deve ser informado para obter.");

            return _mapper.Map<PedidoResponseDto>(await _pedidoEFRepository.ObterPedidoPorId(idPedido));
        }

        private static List<ItensPedido> PedidoCadastroRequestDtoToItensPedidoList(List<ItemPedidoCadastroRequestDto> pedidoAlteracao, int idPedido)
            => pedidoAlteracao.Select(ip => new ItensPedido()
               {
                    IdPedido = idPedido,
                    IdProduto = ip.IdProduto,
                    Quantidade = ip.Quantidade,
               }).ToList();

        private static List<ItensPedido> PedidoAlteracaoRequestDtoToItensPedidoList(PedidoAlteracaoRequestDto pedidoAlteracao)
            => pedidoAlteracao.ItensPedido.Select(ip => new ItensPedido()
               {
                    IdItem = ip.IdItem,
                    IdPedido = pedidoAlteracao.IdPedido,
                    IdProduto = ip.IdProduto,
                    Quantidade = ip.Quantidade,
                    Status = ip.Status,
               }).ToList();
    }
}
