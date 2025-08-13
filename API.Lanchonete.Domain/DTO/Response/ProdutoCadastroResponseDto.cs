using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Response
{
    public class ProdutoCadastroResponseDto : ProdutoBase
    {
        public int IdProduto { get; set; }
    }
}
