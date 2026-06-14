using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEstoque.Data.Entities
{
    public class Produto
    {

        // Código único do produto dentro do sistema
        public int Id { get; set; }

        // Nome do produto
        public string Nome { get; set; }

        // Valor unitário do produto
        public decimal Preco { get; set; }

        // Quantidade disponível no estoque
        public int QuantidadeEmEstoque { get; set; }

        // Quantidade mínima antes de alertar reposição
        public int EstoqueMinimo { get; set; }

        // Indica se o produto está ativo no sistema
        public bool Ativo { get; set; } = true;

        // Chave estrangeira para Categoria
        public int CategoriaId { get; set; }

        // Navegação para Categoria
        public Categoria Categoria { get; set; }
    }
}
