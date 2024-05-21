 using System.Net.Http;
using System.Net;
using System.Text.Json;
using VAVS_Client.Data;
using VAVS_Client.Util;
using Newtonsoft.Json.Linq;

namespace VAVS_Client.APIService.Impl
{
    public class VehicleStandardValueAPIServiceImpl : VehicleStandardValueAPIService
    {
        private readonly ILogger<VehicleStandardValueAPIServiceImpl> _logger;
        private readonly HttpClient _httpClient;

        public VehicleStandardValueAPIServiceImpl(HttpClient httpClient, ILogger<VehicleStandardValueAPIServiceImpl> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<VehicleStandardValue> GetVehicleValueByVehicleNumber(string carNumber)
        {
            string apiKey = Utility.SEARCH_VEHICLE_STANDARD_VALUE_API_KEY;
            string baseUrl = "http://203.81.89.218:99/VehicleStandardAPI/api/VehicleStandard/GetVehicleByCarNumber";
            string url = $"{baseUrl}?carNumber={carNumber}&apiKey={apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            Console.WriteLine("success state code: " + response.StatusCode);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;
                return new VehicleStandardValue()
                {
                    Manufacturer = root.GetProperty("manufacturer").GetString(),
                    BuildType = root.GetProperty("buildType").GetString(),
                    VehicleBrand = root.GetProperty("vehicleBrand").GetString(),
                    ModelYear = root.GetProperty("modelYear").GetString(),
                    EnginePower = root.GetProperty("enginePower").GetString(),
                    VehicleNumber = root.GetProperty("carNumber").GetString(),
                    StandardValue =  root.GetProperty("standardValue").GetString(),
                    Fuel = new Fuel()
                    {
                        FuelType = root.GetProperty("fuelTypeID").GetString()
                    }
                };
            }

            Console.WriteLine("fail state code: " + response.StatusCode);
            Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
            throw new HttpRequestException($"Failed to send message. Status code: {response.StatusCode}");

        }

        public async Task<VehicleStandardValue> GetVehicleValue(string manufacturer, string buildType, string fuelType, string vehicleBrand, string modelYear, string enginePower)
        {
            string apiKey = Utility.SEARCH_VEHICLE_STANDARD_VALUE_API_KEY;
            string baseUrl = "http://203.81.89.218:99/VehicleStandardAPI/api/VehicleStandard/GetVehicleStandardValue";
            string url = $"{baseUrl}?manufacturer={manufacturer}&buildType={buildType}&fuelType={fuelType}&vehicleBrand={vehicleBrand}&modelYear={modelYear}&enginePower={enginePower}&apiKey={apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            Console.WriteLine("success state code: " + response.StatusCode);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(responseJson);
                string standardValue = (string)jsonObject["standardValue"];
                return new VehicleStandardValue()
                {
                    Manufacturer = manufacturer,
                    BuildType = buildType,
                    VehicleBrand = vehicleBrand,
                    ModelYear = modelYear,
                    EnginePower = enginePower,
                    StandardValue = standardValue,
                    Fuel = new Fuel()
                    {
                        FuelType = fuelType
                    }
                };
            }

            Console.WriteLine("fail state code: " + response.StatusCode);
            Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
            throw new HttpRequestException($"Failed to send message. Status code: {response.StatusCode}");

        }
    }
}
