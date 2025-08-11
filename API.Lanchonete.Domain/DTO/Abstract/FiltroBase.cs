namespace API.Lanchonete.Domain.DTO.Abstract
{
    public abstract class FiltroBase
    {
        public int? OrdenarPor { get; set; } = 0;
        public bool OrdemDesc { get; set; } = false;
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
