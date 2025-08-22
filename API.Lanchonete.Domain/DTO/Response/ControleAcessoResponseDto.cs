using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Response
{
    public class ControleAcessoResponseDto : ControleAcessoBase
    {
        public int IdControle { get; set; }
        public DateTime DataCadastro { get; set; }
        public string NomePerfil { get; set; }
    }
}
