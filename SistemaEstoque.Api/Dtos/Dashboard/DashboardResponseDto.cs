namespace SistemaEstoque.Api.Dtos.Dashboard
{
    public class DashboardResponseDto
    {
        public int TotalProdutos { get; set; }

        public int TotalCategorias { get; set; }

        public int TotalPedidos { get; set; }

        public int ProdutosComEstoqueBaixo { get; set; }

        public int QuantidadeTotalEmEstoque { get; set; }

        public decimal ValorTotalEstoque { get; set; }
    }
}