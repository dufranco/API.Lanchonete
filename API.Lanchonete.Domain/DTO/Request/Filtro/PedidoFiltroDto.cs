using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Request.Filtro
{
    public class PedidoFiltroDto : FiltroBase
    {
        public int? IdPedido { get; set; }
        public DateTime? DataCadastroIni { get; set; }
        public DateTime? DataCadastroFim { get; set; }
        public string? Status { get; set; }
        public int? IdProduto { get; set; }
    }
}
