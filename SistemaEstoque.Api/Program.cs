using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Repositories.Implementations;
using SistemaEstoque.Data.Repositories.Interfaces;
using System.Text;

namespace SistemaEstoque.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona suporte aos Controllers da API.
            // Sem isso, os controllers como ProdutosController, CategoriasController,
            // PedidosController e AuthController não funcionam.
            builder.Services.AddControllers();

            // Permite que o Swagger encontre os endpoints da API.
            builder.Services.AddEndpointsApiExplorer();

            // Configura o Swagger básico.
            // Neste momento NÃO vamos configurar o botão Authorize aqui,
            // porque o seu projeto está com conflito no pacote Microsoft.OpenApi.
            // O JWT vai funcionar mesmo sem esse botão.
            builder.Services.AddSwaggerGen();

            // Configura o Entity Framework Core com SQL Server LocalDB.
            // Esse banco é o mesmo que você já estava usando no projeto.
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=SistemaEstoqueDb;Trusted_Connection=True;TrustServerCertificate=True;"
                );
            });

            // Registra os Services da aplicação.
            // Service é onde ficam as regras de negócio.
            builder.Services.AddScoped<CategoriaService>();
            builder.Services.AddScoped<ProdutoService>();
            builder.Services.AddScoped<PedidoService>();

            // Service responsável por cadastro, login, senha com hash e geração do token JWT.
            builder.Services.AddScoped<AuthService>();

            // Registra os Repositories da aplicação.
            // Repository é a camada que conversa com o banco de dados.
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

            // Repository de usuários usado no login e cadastro.
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Busca a chave JWT no appsettings.json.
            // Essa chave será usada para assinar e validar os tokens.
            var jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("A chave JWT não foi configurada no appsettings.json.");
            }

            // Configura a autenticação com JWT.
            // Isso permite que a API reconheça tokens enviados no header Authorization.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Em desenvolvimento local, podemos deixar false.
                // Em produção, normalmente ficaria true.
                options.RequireHttpsMetadata = false;

                // Permite salvar o token no contexto da requisição.
                options.SaveToken = true;

                // Define as regras de validação do token.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Valida quem gerou o token.
                    ValidateIssuer = true,

                    // Valida para quem o token foi criado.
                    ValidateAudience = true,

                    // Valida se o token ainda não expirou.
                    ValidateLifetime = true,

                    // Valida se a chave usada para assinar o token é correta.
                    ValidateIssuerSigningKey = true,

                    // Valores esperados, vindos do appsettings.json.
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    // Chave secreta usada para validar a assinatura do token.
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    )
                };
            });

            // Adiciona autorização.
            // Isso permite usar [Authorize] nos controllers.
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Habilita Swagger em ambiente de desenvolvimento.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Primeiro a API tenta autenticar o usuário pelo token.
            app.UseAuthentication();

            // Depois ela verifica se o usuário tem permissão para acessar a rota.
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}