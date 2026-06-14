using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    // Implementação das operações de banco para Pedido
    public class PedidoRepository : IPedidoRepository
    {
        // Contexto usado para acessar o banco de dados
        private readonly AppDbContext _context;

        // Construtor que recebe o AppDbContext por injeção de dependência
        public PedidoRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todos os pedidos com itens, produtos e categorias
        public List<Pedido> ListarTodos()
        {
            return _context.Pedidos
                .Include(pedido => pedido.Itens)
                .ThenInclude(item => item.Produto)
                .ThenInclude(produto => produto.Categoria)
                .ToList();
        }

        // Busca um pedido pelo ID com itens e produtos
        public Pedido BuscarPorId(int id)
        {
            return _context.Pedidos
                .Include(pedido => pedido.Itens)
                .ThenInclude(item => item.Produto)
                .FirstOrDefault(pedido => pedido.Id == id);
        }

        // Busca um produto pelo ID
        public Produto BuscarProdutoPorId(int produtoId)
        {
            return _context.Produtos.FirstOrDefault(produto => produto.Id == produtoId);
        }

        // Cadastra um novo pedido
        public void Cadastrar(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();
        }

        // Remove um pedido existente
        public void Remover(Pedido pedido)
        {
            _context.Pedidos.Remove(pedido);
            _context.SaveChanges();
        }

        // Salva alterações feitas no banco
        public void SalvarAlteracoes()
        {
            _context.SaveChanges();
        }

        // Retorna todos os itens de pedido
        public List<ItemPedido> ListarItensPedido()
        {
            return _context.ItensPedido
                .Include(item => item.Produto)
                .ToList();
        }

        // Retorna todos os produtos
        public List<Produto> ListarProdutos()
        {
            return _context.Produtos.ToList();
        }

        // Retorna o total de categorias
        public int ContarCategorias()
        {
            return _context.Categorias.Count();
        }

        // Retorna o total de pedidos
        public int ContarPedidos()
        {
            return _context.Pedidos.Count();
        }
    }
}