using VAVS_Client.Paging;

namespace VAVS_Client.Services
{
    public interface VehicleStandardValueService
    {
        List<VehicleStandardValue> GetAllVehicles();
        PagingList<VehicleStandardValue> GetAllVehiclesWithPagin(string searchString, int? pageNo, int PageSize);
        VehicleStandardValue FindVehicleStandardValueById(int id);
        VehicleStandardValue FindVehicleStandardValueByIdEgerLoad(int id);
        VehicleStandardValue FindVehicleStandardValueByIdYBSTableEgerLoad(int id);
        VehicleStandardValue FindVehicleStandardValueByIdContainSoftDeleteEgerLoad(int id);
        VehicleStandardValue FindVehicleByVehicleNumber(string vehicleNumer);
        Task<VehicleStandardValue> GetVehicleValue(string manufacturer, string buildType, string fuelType, string vehicleBrand, string modelYear, string enginePower);
        Task<VehicleStandardValue> GetVehicleValueByVehicleNumber(string carNumber);

    }
}
