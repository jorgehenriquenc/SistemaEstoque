using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEstoque.Data.Entities
{
    public class Pedido
    {
        // Identificador único do pedido
        public int Id { get; set; }

        // Data e hora em que o pedido foi criado
        public DateTime DataPedido { get; set; } = DateTime.Now;

        // Lista de itens que pertencem a este pedido
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }
}
