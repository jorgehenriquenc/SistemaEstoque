using System;
using System.Collections.Generic;

namespace SistemaEstoque.Data.Entities
{
    public class Pedido
    {
        // Identificador único do pedido.
        public int Id { get; set; }

        // Data e hora em UTC em que o pedido foi criado.
        // PostgreSQL timestamp with time zone exige DateTime com Kind UTC.
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;

        // Lista de itens que pertencem a este pedido.
        public List<ItemPedido> Itens { get; set; } = new();
    }
}