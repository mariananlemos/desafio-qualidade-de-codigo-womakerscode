using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class EstoqueService : IEstoqueService
{
    private readonly AppDbContext _context;

    public EstoqueService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<object>> ListarEstoqueAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .Select(p => (object)new
            {
                produto = p.Nome,
                quantidade = p.QuantidadeEstoque
            }).ToListAsync();
    }

    public async Task<bool> ReabastecerEstoqueAsync(int produtoId, int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");

        var produto = await _context.Produtos.FindAsync(produtoId);

        if (produto is null) return false;

        produto.QuantidadeEstoque += quantidade;

        await _context.SaveChangesAsync();

        Utils.Log("Estoque reabastecido, produto " + produtoId);

        return true;
    }
}