using Microsoft.IdentityModel.Tokens;
using SistemaEstoque.Api.Dtos.Auth;
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

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public void Registrar(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new Exception("O nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new Exception("O email é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Senha))
                throw new Exception("A senha é obrigatória.");

            if (dto.Senha.Length < 6)
                throw new Exception("A senha deve ter pelo menos 6 caracteres.");

            if (_usuarioRepository.EmailExiste(dto.Email))
                throw new Exception("Já existe um usuário cadastrado com esse email.");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = GerarHashSenha(dto.Senha)
            };

            _usuarioRepository.Cadastrar(usuario);
        }

        public AuthResponseDto Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new Exception("O email é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Senha))
                throw new Exception("A senha é obrigatória.");

            var usuario = _usuarioRepository.BuscarPorEmail(dto.Email);

            if (usuario == null)
                throw new Exception("Email ou senha inválidos.");

            var senhaValida = VerificarSenha(dto.Senha, usuario.SenhaHash);

            if (!senhaValida)
                throw new Exception("Email ou senha inválidos.");

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
            var jwtKey = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new Exception("Chave JWT não configurada.");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GerarHashSenha(string senha)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                senha,
                salt,
                100000,
                HashAlgorithmName.SHA256
            );

            byte[] hash = pbkdf2.GetBytes(32);

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        private bool VerificarSenha(string senha, string senhaHashSalva)
        {
            var partes = senhaHashSalva.Split('.');

            if (partes.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(partes[0]);
            byte[] hashSalvo = Convert.FromBase64String(partes[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                senha,
                salt,
                100000,
                HashAlgorithmName.SHA256
            );

            byte[] hashDigitado = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(hashSalvo, hashDigitado);
        }
    }
}