using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using VAVS_Client.Classes;
using VAVS_Client.Data;
using VAVS_Client.Util;

namespace VAVS_Client.APIService.Impl
{
    public class PersonalDetailAPIServiceImpl : PersonalDetailAPIService
    {
        private readonly ILogger<PersonalDetailAPIServiceImpl> _logger;
        private readonly HttpClient _httpClient;
        public PersonalDetailAPIServiceImpl(HttpClient httpClient, ILogger<PersonalDetailAPIServiceImpl> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<PersonalInformation> GetPersonalInformationByNRC(string nrc)
        {
            _logger.LogInformation(">>>>>>>>>> [PersonalDetailAPIServiceImpl][GetPersonalInformationByNRC] Get personal information by nrc. <<<<<<<<<<");
            try
            {

                string apiKey = Utility.SEARCH_VEHICLE_STANDARD_VALUE_API_KEY;
                string baseUrl = "http://203.81.89.218:99/VehicleStandardAPI/api/PersonalInformation/GetPersonalInformationByNRC";
                string url = $"{baseUrl}?NRC={nrc}&apiKey={apiKey}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                Console.WriteLine("success state code: " + response.StatusCode);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    PersonalInformation personalInfo = JsonConvert.DeserializeObject<PersonalInformation>(json);
                    return personalInfo;
                }

                Console.WriteLine("fail state code: " + response.StatusCode);
                Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
                throw new HttpRequestException($"Failed to send message. Status code: {response.StatusCode}");

            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by phone number. <<<<<<<<<<" + e);
                throw;
            }
        }
    }
}
