namespace SistemaEstoque.Api.Dtos.Pedidos
{
    // DTO usado para registrar um novo pedido
    public class PedidoCreateDto
    {
        public List<ItemPedidoCreateDto> Itens { get; set; } = new List<ItemPedidoCreateDto>();
    }
}