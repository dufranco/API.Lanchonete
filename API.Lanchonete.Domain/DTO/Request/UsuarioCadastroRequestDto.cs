using API.Lanchonete.Domain.DTO.Abstract;
using System.Security;
using System.Text.Json.Serialization;

namespace API.Lanchonete.Domain.DTO.Request
{
    public class UsuarioCadastroRequestDto : UsuarioBase
    {
        public string? Senha { get; set; }

        [JsonIgnore]
        public SecureString? SenhaCriptografada { get; set; }
    }
}