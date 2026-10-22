namespace PiorEcommerceDoMundo;

/// <summary>
/// Mock de processamento de pagamentos.
/// Não utiliza Thread.Sleep (não bloqueia a thread) nem Random para decidir regras críticas de negócio.
/// Permite simulação previsível de sucesso e falha para desenvolvimento e testes automatizados.
/// </summary>
public static class PagamentoMock
{
    private const int LatenciaGatewayMs = 500;

    /// <summary>
    /// Valor sentinela utilizado exclusivamente em testes para forçar a simulação de pagamento recusado.
    /// </summary>
    public const decimal ValorTesteSimularFalha = 9999.99m;

    /// <summary>
    /// Processa o pagamento de forma assíncrona, simulando latência com Task.Delay.
    /// </summary>
    public static async Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento)
    {
        ServicoLog.Log($"Iniciando processamento de pagamento. Valor: {valor:C2}, Forma: {formaPagamento}");

        // Simula tempo de resposta do gateway sem travar a thread da aplicação
        await Task.Delay(LatenciaGatewayMs);

        // Validações determinísticas
        if (valor <= 0)
        {
            ServicoLog.Log("Pagamento recusado: valor inválido.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(formaPagamento))
        {
            ServicoLog.Log("Pagamento recusado: forma de pagamento não fornecida.");
            return false;
        }

        // Cenário previsível para testar fluxo de falha sem depender de sorteio
        if (valor == ValorTesteSimularFalha)
        {
            ServicoLog.Log("Pagamento recusado: simulação de falha acionada pelo valor de teste.");
            return false;
        }

        ServicoLog.Log("Pagamento aprovado com sucesso.");
        return true;
    }

    /// <summary>
    /// Sobrecarga síncrona mantida para compatibilidade retroativa.
    /// </summary>
    public static bool ProcessarPagamento(decimal valor, string formaPagamento)
    {
        return ProcessarPagamentoAsync(valor, formaPagamento).GetAwaiter().GetResult();
    }
}
