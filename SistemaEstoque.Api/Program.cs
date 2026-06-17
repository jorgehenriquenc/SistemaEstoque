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

            // Configura o Entity Framework Core com SQL Server LocalDB.
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=SistemaEstoqueDb;Trusted_Connection=True;TrustServerCertificate=True;"
                );
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

            // Busca a chave JWT configurada no appsettings.json.
            var jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("A chave JWT não foi configurada no appsettings.json.");
            }

            // Configura autenticação JWT.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],

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
                // No Swashbuckle 10, usa OpenApiSecuritySchemeReference.
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });

            var app = builder.Build();

            // Habilita Swagger em ambiente de desenvolvimento.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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