using VAVS_Client.Data;

namespace VAVS_Client.Services.Impl
{
    public class TaxValidationServiceImpl : AbstractServiceImpl<TaxValidation>, TaxValidationService
    {
        private readonly ILogger<TaxValidationServiceImpl> _logger;
        public TaxValidationServiceImpl(VAVSClientDBContext context, ILogger<TaxValidationServiceImpl> logger) : base(context, logger)
        {
            _logger = logger;
        }
        public bool IsTaxedVehicle(string vehicleNumber)
        {
            _logger.LogInformation(">>>>>>>>>> [TaxValidationServiceImpl][IsTaxedVehicle] Find Taxed Vehicle by VehicleNumber. <<<<<<<<<<");

            try
            {
                TaxValidation taxValidation = FindByString("VehicleNumber", vehicleNumber);
                if (taxValidation != null)
                {
                    return true;
                }
                _logger.LogInformation(">>>>>>>>>> Success. Find Taxed Vehicle by VehicleNumber. <<<<<<<<<<");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding Taxed Vehicle by VehicleNumber. <<<<<<<<<<" + e);
                throw;
            }

        }

        public TaxValidation FindTaxValidationByNrc(string nrc)
        {
            _logger.LogInformation(">>>>>>>>>> [TaxValidationServiceImpl][FindTaxValidationByNrc] Find Taxed Vehicle by nrc. <<<<<<<<<<");

            try
            {
                TaxValidation taxValidation = FindByString("PersonNRC", nrc);
                _logger.LogInformation(">>>>>>>>>> Success. Find Taxed Vehicle by nrc. <<<<<<<<<<");
                return taxValidation;

            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding Taxed Vehicle by nrc. <<<<<<<<<<" + e);
                throw;
            }

        }
    }
}
