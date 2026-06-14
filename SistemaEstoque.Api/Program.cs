using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Repositories.Interfaces;
using SistemaEstoque.Data.Repositories.Implementations;
using SistemaEstoque.Data.Repositories.Interfaces;
using SistemaEstoque.Data.Repositories.Implementations;

namespace SistemaEstoque.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona suporte a Controllers na API
            builder.Services.AddControllers();

            // Registra o AppDbContext para a API conseguir acessar o banco de dados
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=SistemaEstoqueDb;Trusted_Connection=True;"
                );
            });

            // Registra os Services para serem usados pelos Controllers
            builder.Services.AddScoped<CategoriaService>();
            builder.Services.AddScoped<ProdutoService>();
            builder.Services.AddScoped<PedidoService>();

            // Registra os Repositories da aplicação
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

            // Adiciona suporte ao Swagger para documentar e testar a API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Habilita o Swagger apenas em ambiente de desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Redireciona chamadas HTTP para HTTPS
            app.UseHttpsRedirection();

            // Habilita autorização na pipeline da aplicação
            app.UseAuthorization();

            // Mapeia os controllers da API
            app.MapControllers();

            // Inicia a aplicação
            app.Run();
        }
    }
}