namespace SistemaEstoque.Api.Dtos.Pedidos
{
    public class ItemPedidoResponseDto
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }

        public string Produto { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }

        public decimal TotalItem { get; set; }
    }
}