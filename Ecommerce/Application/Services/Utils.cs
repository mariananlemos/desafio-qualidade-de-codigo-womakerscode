namespace Ecommerce.Application.Services;

/// <summary>
/// Classe legada mantida temporariamente para compatibilidade com outras partes do código.
/// Suas responsabilidades foram divididas em:
/// - <see cref="ValidadorEmail"/>
/// - <see cref="ServicoLog"/>
/// - <see cref="GeradorCodigoRastreio"/>
/// </summary>
[Obsolete("A classe Utils violava o Princípio da Responsabilidade Única (SRP). Utilize ValidadorEmail, ServicoLog ou GeradorCodigoRastreio diretamente.")]
public static class Utils
{
    public static bool ValidarEmail(string email) => ValidadorEmail.EhValido(email);

    public static void Log(string mensagem) => ServicoLog.Log(mensagem);

    public static string GerarCodigoRastreio() => GeradorCodigoRastreio.Gerar();
}
