namespace SistemaEstoque.Api.Dtos.Pedidos
{
    public class PedidoResponseDto
    {
        public int Id { get; set; }

        public DateTime DataPedido { get; set; }

        public List<ItemPedidoResponseDto> Itens { get; set; } = new();

        public decimal TotalPedido { get; set; }
    }
}