namespace SistemaEstoque.Api.Dtos.Produtos
{
    // Representa os dados de produto devolvidos pela API.
    public class ProdutoResponseDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public int QuantidadeEmEstoque { get; set; }

        public int EstoqueMinimo { get; set; }

        public bool Ativo { get; set; }

        public int CategoriaId { get; set; }

        public string Categoria { get; set; } = string.Empty;
    }
}