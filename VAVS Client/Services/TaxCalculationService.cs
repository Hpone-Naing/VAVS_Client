using System.Numerics;

namespace VAVS_Client.Services
{
    public interface TaxCalculationService
    {
        public long CalculateTotalTax(long contractPrice, long assetValue);

    }
}
