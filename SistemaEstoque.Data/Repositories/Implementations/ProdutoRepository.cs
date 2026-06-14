using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    // Implementação das operações de banco para Produto
    public class ProdutoRepository : IProdutoRepository
    {
        // Contexto usado para acessar o banco de dados
        private readonly AppDbContext _context;

        // Construtor que recebe o AppDbContext por injeção de dependência
        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todos os produtos cadastrados com suas categorias
        public List<Produto> ListarTodos()
        {
            return _context.Produtos
                .Include(produto => produto.Categoria)
                .ToList();
        }

        // Busca um produto pelo ID com sua categoria
        public Produto BuscarPorId(int id)
        {
            return _context.Produtos
                .Include(produto => produto.Categoria)
                .FirstOrDefault(produto => produto.Id == id);
        }

        // Verifica se uma categoria existe
        public bool CategoriaExiste(int categoriaId)
        {
            return _context.Categorias.Any(categoria => categoria.Id == categoriaId);
        }

        // Cadastra um novo produto
        public void Cadastrar(Produto produto)
        {
            _context.Produtos.Add(produto);
            _context.SaveChanges();
        }

        // Atualiza um produto existente
        public void Atualizar(Produto produto)
        {
            _context.Produtos.Update(produto);
            _context.SaveChanges();
        }

        // Remove um produto existente
        public void Remover(Produto produto)
        {
            _context.Produtos.Remove(produto);
            _context.SaveChanges();
        }
    }
}