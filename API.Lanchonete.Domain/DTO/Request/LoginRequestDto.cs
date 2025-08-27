using System.Security;
using System.Text.Json.Serialization;

namespace API.Lanchonete.Domain.DTO.Request
{
    public class LoginRequestDto
    {
        public string? Email { get; set; }
        public string? Senha { get; set; }

        [JsonIgnore]
        public SecureString? SenhaCriptografada { get; set; }
    }
}
