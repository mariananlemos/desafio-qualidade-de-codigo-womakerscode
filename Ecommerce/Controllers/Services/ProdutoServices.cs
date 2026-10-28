using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Produto>> ListarProdutoAsync()
    {
        return await _context.Produtos.AsNoTracking().ToListAsync();
    }

    public async Task<Produto?> BuscarProdutoPorIdAsync(int id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<Produto> CriarProdutoAsync(Produto produto)
    {
        ValidarProduto(produto);

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        Utils.Log("Produto criado: " + produto.Nome);

        return produto;
    }

    public async Task<Produto?> AtualizarProdutoAsync(int id, Produto produto)
    {
        var existente = await _context.Produtos.FindAsync(id);

        if (existente is null) return null;

        ValidarProduto(produto);

        existente.Nome = produto.Nome;
        existente.Descricao = produto.Descricao;
        existente.Preco = produto.Preco;
        existente.QuantidadeEstoque = produto.QuantidadeEstoque;
        existente.Ativo = produto.Ativo;

        await _context.SaveChangesAsync();

        return existente;
    }

    private static void ValidarProduto(Produto produto)
    {
        if (string.IsNullOrWhiteSpace(produto.Nome))
            throw new ArgumentException("Nome inválido");

        if (produto.Preco <= 0)
            throw new ArgumentException("Preço inválido");
    }

    public async Task<bool> DeletarProdutoAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto is null)
            return false;

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        return true;
    }
}