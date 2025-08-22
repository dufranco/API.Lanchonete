using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Request.Filtro
{
    public class ControleAcessoFiltroDto : FiltroBase
    {
        public int? IdControle { get; set; }

        public int? IdPerfil { get; set; }

        public string? NomePerfil { get; set; }

        public string? NomeTela { get; set; }

        public bool? Permitido { get; set; }

        public DateTime? DataCadastroIni { get; set; }

        public DateTime? DataCadastroFim { get; set; }
    }
}
