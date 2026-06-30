namespace SistemaEstoque.Api.Dtos.Categorias
{
    // DTO utilizado para devolver os dados de uma categoria pela API.
    public class CategoriaResponseDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }
}