using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Api.Dtos.Pedidos
{
    // DTO usado para informar um produto dentro de um pedido.
    public class ItemPedidoCreateDto
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "O produto é obrigatório."
        )]
        public int ProdutoId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "A quantidade do item deve ser maior que zero."
        )]
        public int Quantidade { get; set; }
    }
}