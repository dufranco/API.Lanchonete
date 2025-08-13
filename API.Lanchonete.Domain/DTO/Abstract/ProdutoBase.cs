namespace API.Lanchonete.Domain.DTO.Abstract
{
    public abstract class ProdutoBase
    {
        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public required decimal Preco { get; set; }
        public required string Tipo { get; set; }
        public required bool Ativo { get; set; }
    }
}
