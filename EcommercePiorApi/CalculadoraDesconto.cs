namespace PiorEcommerceDoMundo;

/// <summary>
/// Responsável pelas regras de negócio para cálculo de descontos em pedidos.
/// Elimina números mágicos espalhados pelo código e centraliza a política de desconto.
/// </summary>
public static class CalculadoraDesconto
{
    public const decimal ValorMinimoParaDescontoProgressivo = 500.00m;
    public const decimal PercentualDescontoProgressivo = 0.10m; // 10%
    public const decimal PercentualDescontoVip = 0.20m;         // 20%
    public const string TermoClienteIdentificadorVip = "vip";

    /// <summary>
    /// Calcula o valor final com base nas regras:
    /// - Pedidos acima de R$ 500 recebem 10% de desconto.
    /// - Clientes VIP (e-mail contendo "vip") recebem 20% de desconto.
    /// </summary>
    public static decimal CalcularTotalComDesconto(decimal subtotal, string clienteEmail)
    {
        if (subtotal <= 0)
            return 0;

        if (subtotal > ValorMinimoParaDescontoProgressivo)
        {
            return subtotal - (subtotal * PercentualDescontoProgressivo);
        }

        if (!string.IsNullOrWhiteSpace(clienteEmail) &&
            clienteEmail.Contains(TermoClienteIdentificadorVip, StringComparison.OrdinalIgnoreCase))
        {
            return subtotal - (subtotal * PercentualDescontoVip);
        }

        return subtotal;
    }
}
