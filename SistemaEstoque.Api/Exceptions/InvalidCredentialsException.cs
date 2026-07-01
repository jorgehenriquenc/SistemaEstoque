namespace SistemaEstoque.Api.Exceptions
{
    // Representa falha de autenticação por credenciais inválidas.
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message)
            : base(message)
        {
        }
    }
}