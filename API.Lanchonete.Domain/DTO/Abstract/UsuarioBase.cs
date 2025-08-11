namespace API.Lanchonete.Domain.DTO.Abstract
{
    public abstract class UsuarioBase
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required int IdPerfil { get; set; }
    }
}
