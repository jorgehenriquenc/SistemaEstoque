namespace SistemaEstoque.Api.Dtos.Produtos
{
    // DTO usado para atualizar um produto existente
    public class ProdutoUpdateDto
    {
        public string Nome { get; set; }

        public decimal Preco { get; set; }

        public int QuantidadeEmEstoque { get; set; }

        public int EstoqueMinimo { get; set; }

        public bool Ativo { get; set; }

        public int CategoriaId { get; set; }
    }
}