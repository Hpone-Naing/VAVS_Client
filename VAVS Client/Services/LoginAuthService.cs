using VAVS_Client.Classes;

namespace VAVS_Client.Services
{
    public interface LoginAuthService
    {
        bool CreateLoginAuth(LoginAuth loginAuth);
        LoginAuth GetLoginAuthByPhoneNumber(string phoneNumber);
        void UpdateOtp(string phoneNumber, string hashedOtp = null);
        void UpdateResendCodeTime(string phoneNumber, string hashedOtp = null);
    }
}
