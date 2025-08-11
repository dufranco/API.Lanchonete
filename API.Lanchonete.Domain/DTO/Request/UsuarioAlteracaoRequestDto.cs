using API.Lanchonete.Domain.DTO.Abstract;
using System.Security;
using System.Text.Json.Serialization;

namespace API.Lanchonete.Domain.DTO.Request
{
    public class UsuarioAlteracaoRequestDto : UsuarioBase
    {
        public required int IdUsuario { get; set; }
        public string? Senha { get; set; }
        [JsonIgnore]
        public SecureString? SenhaCriptografada { get; set; }
    }
}