using System.Numerics;
using VAVS_Client.Classes.TaxCalculation;

namespace VAVS_Client.Services
{
    public interface TaxCalculationService
    {
        public long CalculateTotalTax(long contractPrice, long assetValue);
        public Task<bool> SaveTaxValidation(string nrc, string vehicleNumber, TaxInfo taxInfo);


    }
}
