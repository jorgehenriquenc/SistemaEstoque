using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de pedidos.
    public class PedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(
            IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        // Retorna todos os pedidos cadastrados.
        public async Task<object> ListarPedidosAsync()
        {
            var pedidos =
                await _pedidoRepository.ListarTodosAsync();

            var resposta = pedidos
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
                        TotalItem =
                            item.Quantidade * item.PrecoUnitario
                    }),

                    TotalPedido = pedido.Itens.Sum(
                        item =>
                            item.Quantidade * item.PrecoUnitario
                    )
                })
                .ToList();

            return resposta;
        }

        // Busca um pedido pelo identificador.
        public async Task<object?> BuscarPedidoPorIdAsync(
            int id)
        {
            var pedido =
                await _pedidoRepository.BuscarPorIdAsync(id);

            if (pedido is null)
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
                    TotalItem =
                        item.Quantidade * item.PrecoUnitario
                }),

                TotalPedido = pedido.Itens.Sum(
                    item =>
                        item.Quantidade * item.PrecoUnitario
                )
            };
        }

        // Registra um novo pedido.
        public async Task<(Pedido? Pedido, string? Erro)>
            RegistrarPedidoAsync(PedidoCreateDto pedidoDto)
        {
            if (pedidoDto.Itens is null ||
                pedidoDto.Itens.Count == 0)
            {
                return (
                    null,
                    "O pedido precisa ter pelo menos um item."
                );
            }

            var pedido = new Pedido();

            foreach (var itemDto in pedidoDto.Itens)
            {
                if (itemDto is null)
                {
                    return (
                        null,
                        "O pedido contém um item inválido."
                    );
                }

                if (itemDto.Quantidade <= 0)
                {
                    return (
                        null,
                        "A quantidade do item deve ser maior que zero."
                    );
                }

                var produto =
                    await _pedidoRepository
                        .BuscarProdutoPorIdAsync(
                            itemDto.ProdutoId
                        );

                if (produto is null)
                {
                    return (
                        null,
                        $"Produto com ID {itemDto.ProdutoId} não encontrado."
                    );
                }

                if (itemDto.Quantidade >
                    produto.QuantidadeEmEstoque)
                {
                    return (
                        null,
                        $"Estoque insuficiente para o produto {produto.Nome}."
                    );
                }

                var itemPedido = new ItemPedido
                {
                    ProdutoId = produto.Id,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.Preco
                };

                pedido.Itens.Add(itemPedido);

                produto.QuantidadeEmEstoque -=
                    itemDto.Quantidade;
            }

            await _pedidoRepository
                .CadastrarAsync(pedido);

            return (pedido, null);
        }

        // Cancela um pedido e devolve os itens ao estoque.
        public async Task<bool> CancelarPedidoAsync(int id)
        {
            var pedido =
                await _pedidoRepository.BuscarPorIdAsync(id);

            if (pedido is null)
            {
                return false;
            }

            foreach (var item in pedido.Itens)
            {
                item.Produto.QuantidadeEmEstoque +=
                    item.Quantidade;
            }

            await _pedidoRepository
                .RemoverAsync(pedido);

            return true;
        }

        // Gera os dados do dashboard.
        public async Task<object> ExibirDashboardAsync()
        {
            var produtos =
                await _pedidoRepository.ListarProdutosAsync();

            var totalCategorias =
                await _pedidoRepository.ContarCategoriasAsync();

            var totalPedidos =
                await _pedidoRepository.ContarPedidosAsync();

            var totalProdutos = produtos.Count;

            var produtosComEstoqueBaixo = produtos.Count(
                produto =>
                    produto.QuantidadeEmEstoque <=
                    produto.EstoqueMinimo
            );

            var quantidadeTotalEmEstoque = produtos.Sum(
                produto => produto.QuantidadeEmEstoque
            );

            var valorTotalEstoque = produtos.Sum(
                produto =>
                    produto.Preco *
                    produto.QuantidadeEmEstoque
            );

            return new
            {
                TotalProdutos = totalProdutos,
                TotalCategorias = totalCategorias,
                TotalPedidos = totalPedidos,
                ProdutosComEstoqueBaixo =
                    produtosComEstoqueBaixo,
                QuantidadeTotalEmEstoque =
                    quantidadeTotalEmEstoque,
                ValorTotalEstoque =
                    valorTotalEstoque
            };
        }

        // Retorna os produtos mais vendidos.
        public async Task<object>
            ListarProdutosMaisVendidosAsync()
        {
            var itens =
                await _pedidoRepository
                    .ListarItensPedidoAsync();

            var produtosMaisVendidos = itens
                .GroupBy(item => item.Produto.Nome)
                .Select(grupo => new
                {
                    Produto = grupo.Key,

                    QuantidadeVendida = grupo.Sum(
                        item => item.Quantidade
                    )
                })
                .OrderByDescending(
                    produto => produto.QuantidadeVendida
                )
                .ToList();

            return produtosMaisVendidos;
        }
    }
}