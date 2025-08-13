using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Request.Filtro
{
    public class PerfilFiltroDto : FiltroBase
    {
        public string? Nome { get; set; } = null;
        public string? Descricao { get; set; } = null;
    }
}
