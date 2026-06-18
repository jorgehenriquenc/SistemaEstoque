using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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
            builder.Services.AddControllers();

            // Permite que o Swagger encontre os endpoints da API.
            builder.Services.AddEndpointsApiExplorer();

            // Busca a connection string chamada "DefaultConnection".
            // Localmente, ela vem do appsettings.json ou user-secrets.
            // Na Azure, ela virá das configurações do App Service.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("A connection string 'DefaultConnection' não foi configurada.");
            }

            // Configura o Entity Framework Core com PostgreSQL/Neon.
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // Registra os Services da aplicação.
            builder.Services.AddScoped<CategoriaService>();
            builder.Services.AddScoped<ProdutoService>();
            builder.Services.AddScoped<PedidoService>();
            builder.Services.AddScoped<AuthService>();

            // Registra os Repositories da aplicação.
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Busca as configurações JWT.
            // Localmente, elas podem vir do appsettings.json ou user-secrets.
            // Na Azure, elas podem vir das Environment Variables do App Service.
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("A configuração JWT 'Jwt:Key' não foi configurada.");
            }

            if (string.IsNullOrWhiteSpace(jwtIssuer))
            {
                throw new Exception("A configuração JWT 'Jwt:Issuer' não foi configurada.");
            }

            if (string.IsNullOrWhiteSpace(jwtAudience))
            {
                throw new Exception("A configuração JWT 'Jwt:Audience' não foi configurada.");
            }

            // Configura autenticação JWT.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    )
                };
            });

            // Permite usar [Authorize] nos Controllers.
            builder.Services.AddAuthorization();

            // Configura Swagger com botão Authorize compatível com Swashbuckle 10.
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SistemaEstoque API",
                    Version = "v1",
                    Description = "API REST para gerenciamento de estoque com autenticação JWT."
                });

                // Define autenticação Bearer JWT.
                // Com Type = Http e Scheme = bearer, o Swagger coloca o prefixo Bearer automaticamente.
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Cole apenas o token JWT. Não precisa escrever Bearer antes."
                });

                // Aplica o esquema de segurança nas rotas protegidas.
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });

            var app = builder.Build();

            // Habilita Swagger também no Azure.
            // Para projeto de portfólio, isso é útil porque permite testar a API publicada.
            app.UseSwagger();
            app.UseSwaggerUI();

            // Quando abrir a URL principal da API, redireciona direto para o Swagger.
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.UseHttpsRedirection();

            // Primeiro autentica o token.
            app.UseAuthentication();

            // Depois verifica o [Authorize].
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}