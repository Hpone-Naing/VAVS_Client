using VAVS_Client.Classes;

namespace VAVS_Client.Services
{
    public interface TaxPayerInfoService
    {
        bool CreateLoginUserInfo(string token, LoginUserInfo loginUserInfo);
        LoginUserInfo GetLoginUserByHashedToken(string token);
        void UpdateTaxedPayerInfo(string token, TaxpayerInfo taxPayerInfo);
        void UpdateTaxVehicleInfo(string token, TaxVehicleInfo taxVehicleInfo);

    }
}
