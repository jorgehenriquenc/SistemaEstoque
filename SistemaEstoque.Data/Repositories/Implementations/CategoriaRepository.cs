using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    // Implementa as operações de banco de dados para Categoria.
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todas as categorias cadastradas.
        public async Task<List<Categoria>> ListarTodasAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .OrderBy(categoria => categoria.Nome)
                .ToListAsync();
        }

        // Busca uma categoria pelo identificador.
        public async Task<Categoria?> BuscarPorIdAsync(int id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(categoria => categoria.Id == id);
        }

        // Verifica se já existe categoria com o mesmo nome.
        public async Task<bool> NomeExisteAsync(string nome, int? idIgnorado = null)
        {
            var nomeNormalizado = nome.Trim().ToLower();

            return await _context.Categorias
                .AsNoTracking()
                .AnyAsync(categoria =>
                    categoria.Nome.ToLower() == nomeNormalizado &&
                    (!idIgnorado.HasValue || categoria.Id != idIgnorado.Value)
                );
        }

        // Cadastra uma nova categoria.
        public async Task CadastrarAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();
        }

        // Atualiza uma categoria existente.
        public async Task AtualizarAsync(Categoria categoria)
        {
            await _context.SaveChangesAsync();
        }

        // Remove uma categoria existente.
        public async Task RemoverAsync(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);

            await _context.SaveChangesAsync();
        }

        // Verifica se existem produtos vinculados à categoria.
        public async Task<bool> PossuiProdutosAsync(int categoriaId)
        {
            return await _context.Produtos
                .AsNoTracking()
                .AnyAsync(produto => produto.CategoriaId == categoriaId);
        }
    }
}