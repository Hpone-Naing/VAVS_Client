using VAVS_Client.Classes;

namespace VAVS_Client.Services
{
    public interface PersonalDetailService
    {
        PersonalDetail FindPersonalDetailByNrc(string nrc);
        bool CreatePersonalDetail(PersonalDetail personalDetail);
        PersonalDetail FindPersonalDetailByPhoneNumber(string phoneNumber);
        Task<PersonalInformation> GetPersonalInformationByNRC(string nrc);

    }
}
