namespace API.Lanchonete.Domain.DTO
{
    public class PerfilFiltroDto
    {
        public string? FiltroNome { get; set; } = null;
        public string? FiltroDescricao { get; set; } = null;
        public int? OrdenarPor { get; set; } = 0;
        public bool OrdemDesc { get; set; } = false;
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
