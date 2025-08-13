using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Request.Filtro
{
    public class UsuarioFiltroDto : FiltroBase
    {
        public string? Nome { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? DescricaoPerfil { get; set; } = null;
    }
}
