using SistemaEstoque.Api.Dtos.Dashboard;
using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Api.Dtos.Relatorios;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    public class PedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<List<PedidoResponseDto>> ListarPedidosAsync()
        {
            var pedidos = await _pedidoRepository.ListarTodosAsync();

            return pedidos
                .Select(MapearParaResponseDto)
                .ToList();
        }

        public async Task<PedidoResponseDto> BuscarPedidoPorIdAsync(int id)
        {
            var pedido = await _pedidoRepository.BuscarPorIdAsync(id);

            if (pedido is null)
            {
                throw new NotFoundException("Pedido não encontrado.");
            }

            return MapearParaResponseDto(pedido);
        }

        public async Task<PedidoResponseDto> RegistrarPedidoAsync(PedidoCreateDto pedidoDto)
        {
            if (pedidoDto.Itens is null || pedidoDto.Itens.Count == 0)
            {
                throw new BusinessValidationException("O pedido precisa ter pelo menos um item.");
            }

            if (pedidoDto.Itens.Any(item => item is null))
            {
                throw new BusinessValidationException("O pedido contém um item inválido.");
            }

            foreach (var itemDto in pedidoDto.Itens)
            {
                if (itemDto.ProdutoId <= 0)
                {
                    throw new BusinessValidationException("O produto é obrigatório.");
                }

                if (itemDto.Quantidade <= 0)
                {
                    throw new BusinessValidationException("A quantidade do item deve ser maior que zero.");
                }
            }

            var itensAgrupados = pedidoDto.Itens
                .GroupBy(item => item.ProdutoId)
                .Select(grupo => new
                {
                    ProdutoId = grupo.Key,
                    Quantidade = grupo.Sum(item => item.Quantidade)
                })
                .ToList();

            var pedido = new Pedido();

            foreach (var itemAgrupado in itensAgrupados)
            {
                var produto = await _pedidoRepository.BuscarProdutoPorIdAsync(itemAgrupado.ProdutoId);

                if (produto is null)
                {
                    throw new NotFoundException($"Produto com ID {itemAgrupado.ProdutoId} não encontrado.");
                }

                if (!produto.Ativo)
                {
                    throw new BusinessValidationException($"O produto '{produto.Nome}' está inativo e não pode ser vendido.");
                }

                if (itemAgrupado.Quantidade > produto.QuantidadeEmEstoque)
                {
                    throw new BusinessValidationException($"Estoque insuficiente para o produto '{produto.Nome}'.");
                }

                var itemPedido = new ItemPedido
                {
                    ProdutoId = produto.Id,
                    Quantidade = itemAgrupado.Quantidade,
                    PrecoUnitario = produto.Preco
                };

                pedido.Itens.Add(itemPedido);

                produto.QuantidadeEmEstoque -= itemAgrupado.Quantidade;
            }

            await _pedidoRepository.CadastrarAsync(pedido);

            var pedidoCompleto = await _pedidoRepository.BuscarPorIdAsync(pedido.Id);

            if (pedidoCompleto is null)
            {
                throw new NotFoundException("Pedido cadastrado, mas não foi possível carregá-lo.");
            }

            return MapearParaResponseDto(pedidoCompleto);
        }

        public async Task CancelarPedidoAsync(int id)
        {
            var pedido = await _pedidoRepository.BuscarPorIdAsync(id);

            if (pedido is null)
            {
                throw new NotFoundException("Pedido não encontrado.");
            }

            foreach (var item in pedido.Itens)
            {
                item.Produto.QuantidadeEmEstoque += item.Quantidade;
            }

            await _pedidoRepository.RemoverAsync(pedido);
        }

        public async Task<DashboardResponseDto> ExibirDashboardAsync()
        {
            var produtos = await _pedidoRepository.ListarProdutosAsync();
            var totalCategorias = await _pedidoRepository.ContarCategoriasAsync();
            var totalPedidos = await _pedidoRepository.ContarPedidosAsync();

            return new DashboardResponseDto
            {
                TotalProdutos = produtos.Count,
                TotalCategorias = totalCategorias,
                TotalPedidos = totalPedidos,
                ProdutosComEstoqueBaixo = produtos.Count(produto =>
                    produto.QuantidadeEmEstoque <= produto.EstoqueMinimo
                ),
                QuantidadeTotalEmEstoque = produtos.Sum(produto =>
                    produto.QuantidadeEmEstoque
                ),
                ValorTotalEstoque = produtos.Sum(produto =>
                    produto.Preco * produto.QuantidadeEmEstoque
                )
            };
        }

        public async Task<List<ProdutoMaisVendidoResponseDto>> ListarProdutosMaisVendidosAsync()
        {
            var itens = await _pedidoRepository.ListarItensPedidoAsync();

            return itens
                .GroupBy(item => new
                {
                    item.ProdutoId,
                    item.Produto.Nome
                })
                .Select(grupo => new ProdutoMaisVendidoResponseDto
                {
                    ProdutoId = grupo.Key.ProdutoId,
                    Produto = grupo.Key.Nome,
                    QuantidadeVendida = grupo.Sum(item => item.Quantidade),
                    ValorTotalVendido = grupo.Sum(item => item.Quantidade * item.PrecoUnitario)
                })
                .OrderByDescending(produto => produto.QuantidadeVendida)
                .ToList();
        }

        private static PedidoResponseDto MapearParaResponseDto(Pedido pedido)
        {
            var itens = pedido.Itens
                .Select(item => new ItemPedidoResponseDto
                {
                    Id = item.Id,
                    ProdutoId = item.ProdutoId,
                    Produto = item.Produto?.Nome ?? string.Empty,
                    Categoria = item.Produto?.Categoria?.Nome ?? string.Empty,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario,
                    TotalItem = item.Quantidade * item.PrecoUnitario
                })
                .ToList();

            return new PedidoResponseDto
            {
                Id = pedido.Id,
                DataPedido = pedido.DataPedido,
                Itens = itens,
                TotalPedido = itens.Sum(item => item.TotalItem)
            };
        }
    }
}