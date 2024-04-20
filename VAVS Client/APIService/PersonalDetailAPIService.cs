using VAVS_Client.Classes;

namespace VAVS_Client.APIService
{
    public interface PersonalDetailAPIService
    {
        Task<PersonalInformation> GetPersonalInformationByNRC(string nrc);
    }
}
