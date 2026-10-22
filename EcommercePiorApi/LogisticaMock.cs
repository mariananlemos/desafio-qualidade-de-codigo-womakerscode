namespace PiorEcommerceDoMundo;

/// <summary>
/// Mock de integração com serviço externo de logística parceiro.
/// </summary>
public static class LogisticaMock
{
    private const int LatenciaIntegracaoMs = 300;

    public static async Task<string> EnviarAsync(Pedido pedido)
    {
        if (pedido == null)
            throw new ArgumentNullException(nameof(pedido));

        ServicoLog.Log($"Enviando pedido {pedido.Id} para a transportadora parceira...");

        // Simula latência de chamada de rede sem travar thread
        await Task.Delay(LatenciaIntegracaoMs);

        var codigoRastreio = GeradorCodigoRastreio.Gerar();
        ServicoLog.Log($"Código de rastreio gerado: {codigoRastreio} para o pedido {pedido.Id}");

        return codigoRastreio;
    }
}
