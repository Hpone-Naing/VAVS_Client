using System.Numerics;
using VAVS_Client.Classes;
using VAVS_Client.Classes.TaxCalculation;
using VAVS_Client.Data;

namespace VAVS_Client.Services.Impl
{
    public class TaxCalculationServiceImpl : AbstractServiceImpl<TaxpayerInfo>, TaxCalculationService
    {
        private readonly ILogger<TaxCalculationServiceImpl> _logger;
        private readonly HttpClient _httpClient;
        public TaxCalculationServiceImpl(VAVSClientDBContext context, HttpClient httpClient, ILogger<TaxCalculationServiceImpl> logger) : base(context, logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public long CalculateTotalTax(long contractPrice, long assetValue)
        {
            long maximumValue = (contractPrice > assetValue) ? contractPrice : assetValue;
            VehicleTaxCalculation taxCalculation = new VehicleTaxCalculation();
            return taxCalculation.CalculateTax(maximumValue);
        }
    }
}
