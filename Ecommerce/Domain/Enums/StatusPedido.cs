namespace Ecommerce.Domain.Enums;

/// <summary>
/// Representa os estados possíveis de um pedido no ciclo de vida da compra.
/// Substitui o uso de strings soltas ("Pendente", "Pago", "Falhou", "Enviado").
/// </summary>
public enum StatusPedido
{
    Pendente = 1,
    Pago = 2,
    Falhou = 3,
    Enviado = 4
}
