using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Api.Dtos.Categorias
{
    // DTO usado para cadastrar uma nova categoria.
    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "O nome da categoria deve ter entre 2 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;
    }
}