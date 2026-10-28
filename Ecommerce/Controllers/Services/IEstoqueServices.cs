using Ecommerce.Domain.Entities;
namespace Ecommerce.Application.Interfaces;

public interface IEstoqueService
{
    //Task<List<Produto>> ListarEstoqueAsync();
    Task<List<object>> ListarEstoqueAsync();
    Task<bool> ReabastecerEstoqueAsync(int produtoId, int quantidade);
}