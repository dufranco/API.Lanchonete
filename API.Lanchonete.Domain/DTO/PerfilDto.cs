namespace API.Lanchonete.Domain.DTO
{
    public class PerfilDto
    {
        public int? IdPerfil { get; set; }
        public required string Nome { get; set; }
        public required string Descricao { get; set; }

    }
}
