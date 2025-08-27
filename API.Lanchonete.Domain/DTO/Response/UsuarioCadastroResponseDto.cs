using API.Lanchonete.Domain.DTO.Abstract;
using API.Lanchonete.Domain.Entities;

namespace API.Lanchonete.Domain.DTO.Response
{
    public class UsuarioCadastroResponseDto : UsuarioBase
    {
        public int IdUsuario { get; set; }
        public string NomePerfil { get; set; }
        public List<ControleAcesso> ControleAcessos { get; set; }
    }
}
