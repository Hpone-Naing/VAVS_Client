using VAVS_Client.Classes;

namespace VAVS_Client.APIService
{
    public interface PersonalDetailAPIService
    {
        Task<PersonalDetail> GetPersonalInformationByNRC(string nrc);
    }
}
