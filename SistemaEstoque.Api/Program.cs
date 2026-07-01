using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SistemaEstoque.Api.Exceptions;
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

            // Registra o formato padronizado ProblemDetails.
            builder.Services.AddProblemDetails();

            // Registra o manipulador global de exceções.
            builder.Services
                .AddExceptionHandler<GlobalExceptionHandler>();

            // Permite que o Swagger encontre os endpoints da API.
            builder.Services.AddEndpointsApiExplorer();

            // Busca a connection string chamada "DefaultConnection".
            // Localmente, ela vem do appsettings.json ou user-secrets.
            // No Azure, ela vem das configurações do App Service.
            var connectionString =
                builder.Configuration.GetConnectionString(
                    "DefaultConnection"
                );

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "A connection string 'DefaultConnection' não foi configurada."
                );
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
            builder.Services.AddScoped<
                ICategoriaRepository,
                CategoriaRepository
            >();

            builder.Services.AddScoped<
                IProdutoRepository,
                ProdutoRepository
            >();

            builder.Services.AddScoped<
                IPedidoRepository,
                PedidoRepository
            >();

            builder.Services.AddScoped<
                IUsuarioRepository,
                UsuarioRepository
            >();

            // Busca as configurações JWT.
            // Localmente, elas podem vir do appsettings.json
            // ou dos User Secrets.
            // No Azure, vêm das variáveis de ambiente.
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception(
                    "A configuração JWT 'Jwt:Key' não foi configurada."
                );
            }

            if (string.IsNullOrWhiteSpace(jwtIssuer))
            {
                throw new Exception(
                    "A configuração JWT 'Jwt:Issuer' não foi configurada."
                );
            }

            if (string.IsNullOrWhiteSpace(jwtAudience))
            {
                throw new Exception(
                    "A configuração JWT 'Jwt:Audience' não foi configurada."
                );
            }

            // Configura autenticação JWT.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey)
                            )
                    };
            });

            // Permite usar [Authorize] nos Controllers.
            builder.Services.AddAuthorization();

            // Configura Swagger com autenticação JWT.
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SistemaEstoque API",
                    Version = "v1",
                    Description =
                        "API REST para gerenciamento de estoque com autenticação JWT."
                });

                options.AddSecurityDefinition(
                    "bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description =
                            "Cole apenas o token JWT. Não precisa escrever Bearer antes."
                    }
                );

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecuritySchemeReference(
                                "bearer",
                                document
                            )
                        ] = []
                    }
                );
            });

            var app = builder.Build();

            // Deve ficar no início do pipeline para capturar
            // exceções lançadas pelos middlewares e endpoints seguintes.
            app.UseExceptionHandler();

            // Habilita Swagger localmente e no Azure.
            app.UseSwagger();
            app.UseSwaggerUI();

            // Redireciona a URL principal para o Swagger.
            app.MapGet(
                "/",
                () => Results.Redirect("/swagger")
            );

            app.UseHttpsRedirection();

            // Primeiro autentica o token.
            app.UseAuthentication();

            // Depois verifica o atributo [Authorize].
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}