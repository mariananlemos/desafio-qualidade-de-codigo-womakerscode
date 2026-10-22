namespace PiorEcommerceDoMundo;

/// <summary>
/// Responsável pela geração de códigos de rastreamento para encomendas.
/// Remove os números mágicos e utiliza constantes configuradas.
/// </summary>
public static class GeradorCodigoRastreio
{
    private const string PrefixoBrasil = "BR";
    private const int FaixaMinima = 100000;
    private const int FaixaMaxima = 999999;

    public static string Gerar()
    {
        var numeroRastreio = Random.Shared.Next(FaixaMinima, FaixaMaxima + 1);
        return $"{PrefixoBrasil}{numeroRastreio}";
    }
}
