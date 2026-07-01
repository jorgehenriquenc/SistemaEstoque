namespace SistemaEstoque.Api.Exceptions
{
    // Representa erros causados por dados inválidos
    // enviados pelo usuário.
    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(string message)
            : base(message)
        {
        }
    }
}