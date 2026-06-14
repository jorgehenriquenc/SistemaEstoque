namespace SistemaEstoque.Api.Dtos.Produtos
{
    // DTO usado para cadastrar um novo produto
    public class ProdutoCreateDto
    {
        public string Nome { get; set; }

        public decimal Preco { get; set; }

        public int QuantidadeEmEstoque { get; set; }

        public int EstoqueMinimo { get; set; }

        public int CategoriaId { get; set; }
    }
}