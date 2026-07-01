namespace SistemaEstoque.Api.Exceptions
{
    // Representa um recurso que não foi encontrado.
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}