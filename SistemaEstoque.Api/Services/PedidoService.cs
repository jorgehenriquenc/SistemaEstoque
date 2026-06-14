using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de pedidos
    public class PedidoService
    {
        // Repositório usado para acessar os dados de pedidos
        private readonly IPedidoRepository _pedidoRepository;

        // Construtor que recebe o repositório por injeção de dependência
        public PedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        // Método responsável por listar todos os pedidos
        public object ListarPedidos()
        {
            var pedidos = _pedidoRepository.ListarTodos()
                .Select(pedido => new
                {
                    pedido.Id,
                    pedido.DataPedido,
                    Itens = pedido.Itens.Select(item => new
                    {
                        item.Id,
                        item.ProdutoId,
                        Produto = item.Produto.Nome,
                        Categoria = item.Produto.Categoria.Nome,
                        item.Quantidade,
                        item.PrecoUnitario,
                        TotalItem = item.Quantidade * item.PrecoUnitario
                    }),
                    TotalPedido = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario)
                })
                .ToList();

            return pedidos;
        }

        // Método responsável por buscar um pedido pelo ID
        public object BuscarPedidoPorId(int id)
        {
            var pedido = _pedidoRepository.BuscarPorId(id);

            if (pedido == null)
            {
                return null;
            }

            return new
            {
                pedido.Id,
                pedido.DataPedido,
                Itens = pedido.Itens.Select(item => new
                {
                    item.Id,
                    item.ProdutoId,
                    Produto = item.Produto.Nome,
                    item.Quantidade,
                    item.PrecoUnitario,
                    TotalItem = item.Quantidade * item.PrecoUnitario
                }),
                TotalPedido = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario)
            };
        }

        // Método responsável por registrar um novo pedido
        public Pedido RegistrarPedido(PedidoCreateDto pedidoDto, out string erro)
        {
            erro = string.Empty;

            if (pedidoDto.Itens == null || pedidoDto.Itens.Count == 0)
            {
                erro = "O pedido precisa ter pelo menos um item.";
                return null;
            }

            Pedido pedido = new Pedido();

            foreach (var itemDto in pedidoDto.Itens)
            {
                if (itemDto.Quantidade <= 0)
                {
                    erro = "A quantidade do item deve ser maior que zero.";
                    return null;
                }

                Produto produto = _pedidoRepository.BuscarProdutoPorId(itemDto.ProdutoId);

                if (produto == null)
                {
                    erro = $"Produto com ID {itemDto.ProdutoId} não encontrado.";
                    return null;
                }

                if (itemDto.Quantidade > produto.QuantidadeEmEstoque)
                {
                    erro = $"Estoque insuficiente para o produto {produto.Nome}.";
                    return null;
                }

                ItemPedido itemPedido = new ItemPedido
                {
                    ProdutoId = produto.Id,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.Preco
                };

                pedido.Itens.Add(itemPedido);

                produto.QuantidadeEmEstoque -= itemDto.Quantidade;
            }

            _pedidoRepository.Cadastrar(pedido);

            return pedido;
        }

        // Método responsável por cancelar um pedido e devolver o estoque
        public bool CancelarPedido(int id)
        {
            Pedido pedido = _pedidoRepository.BuscarPorId(id);

            if (pedido == null)
            {
                return false;
            }

            foreach (var item in pedido.Itens)
            {
                item.Produto.QuantidadeEmEstoque += item.Quantidade;
            }

            _pedidoRepository.Remover(pedido);

            return true;
        }

        // Método responsável por gerar os dados do dashboard
        public object ExibirDashboard()
        {
            var produtos = _pedidoRepository.ListarProdutos();

            int totalProdutos = produtos.Count;

            int totalCategorias = _pedidoRepository.ContarCategorias();

            int totalPedidos = _pedidoRepository.ContarPedidos();

            int produtosComEstoqueBaixo = produtos
                .Count(produto => produto.QuantidadeEmEstoque <= produto.EstoqueMinimo);

            int quantidadeTotalEmEstoque = produtos
                .Sum(produto => produto.QuantidadeEmEstoque);

            decimal valorTotalEstoque = produtos
                .Sum(produto => produto.Preco * produto.QuantidadeEmEstoque);

            return new
            {
                TotalProdutos = totalProdutos,
                TotalCategorias = totalCategorias,
                TotalPedidos = totalPedidos,
                ProdutosComEstoqueBaixo = produtosComEstoqueBaixo,
                QuantidadeTotalEmEstoque = quantidadeTotalEmEstoque,
                ValorTotalEstoque = valorTotalEstoque
            };
        }

        // Método responsável por listar produtos mais vendidos
        public object ListarProdutosMaisVendidos()
        {
            var produtosMaisVendidos = _pedidoRepository.ListarItensPedido()
                .GroupBy(item => item.Produto.Nome)
                .Select(grupo => new
                {
                    Produto = grupo.Key,
                    QuantidadeVendida = grupo.Sum(item => item.Quantidade)
                })
                .OrderByDescending(produto => produto.QuantidadeVendida)
                .ToList();

            return produtosMaisVendidos;
        }
    }
}