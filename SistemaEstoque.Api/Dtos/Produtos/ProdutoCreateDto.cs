using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Api.Dtos.Produtos
{
    // DTO usado para cadastrar um novo produto.
    public class ProdutoCreateDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "O nome do produto deve ter entre 2 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        [Range(
            0,
            double.MaxValue,
            MinimumIsExclusive = true,
            ErrorMessage = "O preço deve ser maior que zero."
        )]
        public decimal Preco { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "A quantidade em estoque não pode ser negativa."
        )]
        public int QuantidadeEmEstoque { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "O estoque mínimo não pode ser negativo."
        )]
        public int EstoqueMinimo { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "A categoria é obrigatória."
        )]
        public int CategoriaId { get; set; }
    }
}