using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Request
{
    public class ControleAcessoAlteracaoRequestDto : ControleAcessoBase
    {
        public int IdControle { get; set; }
    }
}
