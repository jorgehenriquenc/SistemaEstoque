using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    // Implementação das operações de banco para Categoria
    public class CategoriaRepository : ICategoriaRepository
    {
        // Contexto usado para acessar o banco de dados
        private readonly AppDbContext _context;

        // Construtor que recebe o AppDbContext por injeção de dependência
        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todas as categorias cadastradas
        public List<Categoria> ListarTodas()
        {
            return _context.Categorias.ToList();
        }

        // Busca uma categoria pelo ID
        public Categoria BuscarPorId(int id)
        {
            return _context.Categorias.FirstOrDefault(categoria => categoria.Id == id);
        }

        // Cadastra uma nova categoria
        public void Cadastrar(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            _context.SaveChanges();
        }

        // Atualiza uma categoria existente
        public void Atualizar(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            _context.SaveChanges();
        }

        // Remove uma categoria existente
        public void Remover(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
        }

        // Verifica se uma categoria possui produtos vinculados
        public bool PossuiProdutos(int categoriaId)
        {
            return _context.Produtos.Any(produto => produto.CategoriaId == categoriaId);
        }
    }
}