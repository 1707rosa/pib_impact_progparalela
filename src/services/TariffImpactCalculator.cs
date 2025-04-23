using System;
public class TariffImpactCalculator 
{
    
    private const double SensibilidadArancel = 1.5;
    public static double CalcularPibAjustado(double pibOriginal, double tasaArancel)
    {
        return pibOriginal * Math.Exp(-SensibilidadArancel * tasaArancel);
    }
}