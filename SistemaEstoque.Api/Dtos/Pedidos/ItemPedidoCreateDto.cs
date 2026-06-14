namespace SistemaEstoque.Api.Dtos.Pedidos
{
    // DTO usado para informar um produto dentro de um pedido
    public class ItemPedidoCreateDto
    {
        public int ProdutoId { get; set; }

        public int Quantidade { get; set; }
    }
}