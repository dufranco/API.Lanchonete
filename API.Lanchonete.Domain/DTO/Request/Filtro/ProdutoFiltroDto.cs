using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Request.Filtro
{
    public class ProdutoFiltroDto : FiltroBase
    {
        public int? IdProduto { get; set; } = null;
        public string? Nome { get; set; } = null;
        public string? Descricao { get; set; } = null;
        public string? Tipo { get; set; } = null;
        public bool? Ativo { get; set; } = null;
    }
}
