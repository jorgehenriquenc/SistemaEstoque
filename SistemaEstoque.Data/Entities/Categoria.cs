namespace SistemaEstoque.Data.Entities
{
    // Representa uma categoria de produtos.
    public class Categoria
    {
        // Identificador único da categoria.
        public int Id { get; set; }

        // Nome da categoria.
        public string Nome { get; set; } = string.Empty;

        // Produtos vinculados a esta categoria.
        public List<Produto> Produtos { get; set; } = new();
    }
}