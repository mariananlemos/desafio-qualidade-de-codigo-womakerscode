namespace Ecommerce.Application.Services;

/// <summary>
/// Responsável pelo registro de logs no console e em arquivo de texto.
/// </summary>
public static class ServicoLog
{
    private const string CaminhoArquivoLog = "log.txt";

    public static void Log(string mensagem)
    {
        var linhaFormatada = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensagem}";
        Console.WriteLine(linhaFormatada);

        try
        {
            File.AppendAllText(CaminhoArquivoLog, linhaFormatada + Environment.NewLine);
        }
        catch (Exception ex)
        {
            // Trata o erro e informa no stream de erro do console sem derrubar a aplicação
            Console.Error.WriteLine($"[ServicoLog] Erro ao persistir log em arquivo: {ex.Message}");
        }
    }
}
