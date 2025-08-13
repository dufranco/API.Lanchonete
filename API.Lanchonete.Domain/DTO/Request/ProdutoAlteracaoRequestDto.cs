using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Request
{
    public class ProdutoAlteracaoRequestDto : ProdutoBase
    {
        public required int IdProduto { get; set; }
    }
}