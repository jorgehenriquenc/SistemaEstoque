using Microsoft.IdentityModel.Tokens;
using SistemaEstoque.Api.Dtos.Auth;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SistemaEstoque.Api.Services
{
    public class AuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public void Registrar(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                throw new BusinessValidationException(
                    "O nome é obrigatório."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new BusinessValidationException(
                    "O email é obrigatório."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.Senha))
            {
                throw new BusinessValidationException(
                    "A senha é obrigatória."
                );
            }

            if (dto.Senha.Length < 6)
            {
                throw new BusinessValidationException(
                    "A senha deve ter pelo menos 6 caracteres."
                );
            }

            var nome = dto.Nome.Trim();
            var email = dto.Email.Trim().ToLowerInvariant();

            if (_usuarioRepository.EmailExiste(email))
            {
                throw new ConflictException(
                    "Já existe um usuário cadastrado com esse email."
                );
            }

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                SenhaHash = GerarHashSenha(dto.Senha)
            };

            _usuarioRepository.Cadastrar(usuario);
        }

        public AuthResponseDto Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new BusinessValidationException(
                    "O email é obrigatório."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.Senha))
            {
                throw new BusinessValidationException(
                    "A senha é obrigatória."
                );
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var usuario =
                _usuarioRepository.BuscarPorEmail(email);

            if (usuario is null)
            {
                throw new InvalidCredentialsException(
                    "Email ou senha inválidos."
                );
            }

            var senhaValida = VerificarSenha(
                dto.Senha,
                usuario.SenhaHash
            );

            if (!senhaValida)
            {
                throw new InvalidCredentialsException(
                    "Email ou senha inválidos."
                );
            }

            var token = GerarToken(usuario);

            return new AuthResponseDto
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Token = token
            };
        }

        private string GerarToken(Usuario usuario)
        {
            var jwtKey =
                _configuration["Jwt:Key"];

            var issuer =
                _configuration["Jwt:Issuer"];

            var audience =
                _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "A chave JWT não foi configurada."
                );
            }

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new InvalidOperationException(
                    "O emissor JWT não foi configurado."
                );
            }

            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "O público JWT não foi configurado."
                );
            }

            var claims = new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    usuario.Id.ToString()
                ),

                new(
                    ClaimTypes.Name,
                    usuario.Nome
                ),

                new(
                    ClaimTypes.Email,
                    usuario.Email
                )
            };

            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credenciais = new SigningCredentials(
                chave,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private static string GerarHashSenha(string senha)
        {
            var salt =
                RandomNumberGenerator.GetBytes(16);

            using var pbkdf2 =
                new Rfc2898DeriveBytes(
                    senha,
                    salt,
                    100000,
                    HashAlgorithmName.SHA256
                );

            var hash = pbkdf2.GetBytes(32);

            return
                $"{Convert.ToBase64String(salt)}." +
                $"{Convert.ToBase64String(hash)}";
        }

        private static bool VerificarSenha(
            string senha,
            string senhaHashSalva)
        {
            var partes =
                senhaHashSalva.Split('.');

            if (partes.Length != 2)
            {
                return false;
            }

            try
            {
                var salt =
                    Convert.FromBase64String(partes[0]);

                var hashSalvo =
                    Convert.FromBase64String(partes[1]);

                using var pbkdf2 =
                    new Rfc2898DeriveBytes(
                        senha,
                        salt,
                        100000,
                        HashAlgorithmName.SHA256
                    );

                var hashDigitado =
                    pbkdf2.GetBytes(32);

                return CryptographicOperations
                    .FixedTimeEquals(
                        hashSalvo,
                        hashDigitado
                    );
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}