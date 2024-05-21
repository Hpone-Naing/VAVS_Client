namespace VAVS_Client.Services
{
    public interface TaxValidationService
    {
        bool IsTaxedVehicle(string vehicleNumber);
        TaxValidation FindTaxValidationByNrc(string nrc);

    }
}
