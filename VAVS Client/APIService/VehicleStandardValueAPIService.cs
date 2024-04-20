namespace VAVS_Client.APIService
{
    public interface VehicleStandardValueAPIService
    {
        Task<VehicleStandardValue> GetVehicleValueByVehicleNumber(string carNumber);
        Task<VehicleStandardValue> GetVehicleValue(string manufacturer, string buildType, string fuelType, string vehicleBrand, string modelYear, string enginePower);


    }
}
