using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEstoque.Data.Entities
{
    public  class ItemPedido
    {
        // Identificador único do item
        public int Id { get; set; }

        // Chave estrangeira para o pedido
        public int PedidoId { get; set; }

        // Navegação para o pedido
        public Pedido Pedido { get; set; }

        // Chave estrangeira para o produto
        public int ProdutoId { get; set; }

        // Navegação para o produto
        public Produto Produto { get; set; }

        // Quantidade do produto neste item
        public int Quantidade { get; set; }

        // Preço do produto no momento da venda
        public decimal PrecoUnitario { get; set; }
    }
}