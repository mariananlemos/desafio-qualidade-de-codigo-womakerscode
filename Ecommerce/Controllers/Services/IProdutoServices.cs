using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IProdutoService
{
    Task<List<Produto>> ListarProdutoAsync();
    Task<Produto?> BuscarProdutoPorIdAsync(int id);
    Task<Produto> CriarProdutoAsync(Produto produto);
    Task<Produto?> AtualizarProdutoAsync(int id, Produto produto);
    Task<bool> DeletarProdutoAsync(int id);
}