using API.Lanchonete.Domain.DTO.Abstract;
using System.Text.Json.Serialization;

namespace API.Lanchonete.Domain.DTO.Request
{
    public class ItemPedidoRequestDto : ItemPedidoBase
    {
        [JsonIgnore]
        public int? IdPedido { get; set; }
    }
}
