using System.Net.Mail;

namespace Ecommerce.Application.Services;

/// <summary>
/// Responsável exclusivamente pela validação de formato de endereços de e-mail.
/// </summary>
public static class ValidadorEmail
{
    public static bool EhValido(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var endereco = new MailAddress(email);
            return endereco.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
