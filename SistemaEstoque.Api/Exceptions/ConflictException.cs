namespace SistemaEstoque.Api.Exceptions
{
    // Representa um conflito com um recurso já existente.
    public class ConflictException : Exception
    {
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}