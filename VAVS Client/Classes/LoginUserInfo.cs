namespace VAVS_Client.Classes
{
    public class LoginUserInfo
    {
        public TaxpayerInfo TaxpayerInfo { get; set; }
        public TaxVehicleInfo TaxVehicleInfo { get; set; }
        public DateTime LoggedInTime { get; set; }
        public bool RememberMe { get; set; }

        public bool IsTaxpayerInfoAndTaxVehicleInfoNull()
        {
            return TaxpayerInfo == null || TaxVehicleInfo == null;
        }
        public bool IsTaxpayerInfoNull()
        {
            return this == null || TaxpayerInfo == null;
        }
        public bool IsTaxVehicleInfoNull()
        {
            return this == null || TaxVehicleInfo == null;
        }
    }
}
