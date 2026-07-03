using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Api.Dtos.Pedidos
{
    // DTO usado para registrar um novo pedido.
    public class PedidoCreateDto : IValidatableObject
    {
        [Required(
            ErrorMessage = "A lista de itens do pedido é obrigatória."
        )]
        [MinLength(
            1,
            ErrorMessage = "O pedido precisa ter pelo menos um item."
        )]
        public List<ItemPedidoCreateDto> Itens { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (Itens is null)
            {
                yield break;
            }

            for (int indice = 0; indice < Itens.Count; indice++)
            {
                if (Itens[indice] is null)
                {
                    yield return new ValidationResult(
                        $"O item da posição {indice + 1} não pode ser nulo.",
                        new[] { nameof(Itens) }
                    );
                }
            }
        }
    }
}