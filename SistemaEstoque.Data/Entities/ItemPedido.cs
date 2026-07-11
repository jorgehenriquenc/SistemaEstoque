namespace SistemaEstoque.Data.Entities
{
    // Representa um item dentro de um pedido.
    public class ItemPedido
    {
        // Identificador único do item.
        public int Id { get; set; }

        // Chave estrangeira para o pedido.
        public int PedidoId { get; set; }

        // Navegação para o pedido.
        // O Entity Framework preenche essa propriedade quando o pedido é carregado.
        public Pedido Pedido { get; set; } = null!;

        // Chave estrangeira para o produto.
        public int ProdutoId { get; set; }

        // Navegação para o produto.
        // O Entity Framework preenche essa propriedade quando o produto é carregado.
        public Produto Produto { get; set; } = null!;

        // Quantidade do produto neste item.
        public int Quantidade { get; set; }

        // Preço do produto no momento da venda.
        public decimal PrecoUnitario { get; set; }
    }
}