
using VAVS_Client.Classes;
using VAVS_Client.Classes.TaxCalculation;
using VAVS_Client.Data;
using VAVS_Client.Factories;
using VAVS_Client.Util;

namespace VAVS_Client.Services.Impl
{
    public class TaxCalculationServiceImpl : AbstractServiceImpl<TaxValidation>, TaxCalculationService
    {
        private readonly ILogger<TaxCalculationServiceImpl> _logger;
        private readonly HttpClient _httpClient;
        private readonly PersonalDetailService _personalDetailService;
        private readonly TownshipService _townshipService;
        private readonly StateDivisionService _staetDivisionService;
        private readonly VehicleStandardValueService _vehicleStandardValueService;
        private readonly FuelTypeService _fuelTypeService;
        private readonly TaxPayerInfoService _taxPayerInfoService;
        public TaxCalculationServiceImpl(VAVSClientDBContext context, HttpClient httpClient, ILogger<TaxCalculationServiceImpl> logger, PersonalDetailService personalDetailService, TownshipService townshipService, StateDivisionService staetDivisionService, VehicleStandardValueService vehicleStandardValueService, FuelTypeService fuelTypeService, TaxPayerInfoService taxPayerInfoService) : base(context, logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _personalDetailService = personalDetailService;
            _townshipService = townshipService;
            _staetDivisionService = staetDivisionService;
            _vehicleStandardValueService = vehicleStandardValueService;
            _fuelTypeService = fuelTypeService;
            _taxPayerInfoService = taxPayerInfoService;
        }

        public long CalculateTotalTax(long contractPrice, long assetValue)
        {
            long maximumValue = (contractPrice > assetValue) ? contractPrice : assetValue;
            VehicleTaxCalculation taxCalculation = new VehicleTaxCalculation();
            return taxCalculation.CalculateTax(maximumValue);
        }

        public async Task<bool> SaveTaxValidation(HttpContext httpContext, TaxInfo taxInfo)
        {
            try
            {
                LoginUserInfo loginTaxPayerInfo = _taxPayerInfoService.GetLoginUserByHashedToken(SessionUtil.GetToken(httpContext));
                SessionService sessionService = new SessionServiceImpl();
                TaxpayerInfo loginUserInfo = sessionService.GetLoginUserInfo(httpContext);
                if (loginUserInfo == null)
                    return false;
                /*
                 * Find PersonalDetail from Database and api
                 */
                PersonalDetail personalDetail = await _personalDetailService.GetPersonalInformationByNRCInDBAndAPI(loginUserInfo.NRC);
                Township township = _townshipService.FindTownshipByPkId(personalDetail.TownshipPkid);
                
                if (personalDetail.PersonalPkid == 0)
                {
                     personalDetail.TownshipPkid = personalDetail.TownshipPkid;
                    Console.WriteLine("tainfo...................tateDiviion." + taxInfo.StateDivisionPkid);
                    personalDetail.StateDivisionPkid = personalDetail.StateDivisionPkid;
                    _personalDetailService.CreatePersonalDetail(personalDetail);
                }
                Console.WriteLine("here personalpkid not null.............." + personalDetail.PersonalPkid);
                
                TaxValidation taxValidation = new TaxValidation
                {
                    PersonTINNumber = personalDetail?.PersonTINNumber,
                    PersonNRC = loginUserInfo.NRC,
                    VehicleNumber = loginTaxPayerInfo.TaxVehicleInfo.VehicleNumber,
                    Manufacturer = loginTaxPayerInfo.TaxVehicleInfo.Manufacturer,
                    CountryOfMade = loginTaxPayerInfo.TaxVehicleInfo.CountryOfMade,
                    VehicleBrand = loginTaxPayerInfo.TaxVehicleInfo.VehicleBrand,
                    BuildType = loginTaxPayerInfo.TaxVehicleInfo.BuildType,
                    ModelYear = loginTaxPayerInfo.TaxVehicleInfo.ModelYear,
                    EnginePower = loginTaxPayerInfo.TaxVehicleInfo.EnginePower,
                    FuelType = loginTaxPayerInfo.TaxVehicleInfo.FuelType,
                    StandardValue = decimal.Parse(loginTaxPayerInfo.TaxVehicleInfo.StandardValue),
                    ContractValue = decimal.Parse(loginTaxPayerInfo.TaxVehicleInfo.ContractValue),
                    TaxAmount = decimal.Parse(loginTaxPayerInfo.TaxVehicleInfo.TaxAmount),
                    PersonalDetail = personalDetail,
                    Township = township,
                    EntryDate = DateTime.Now,
                    IsDeleted = false,
                    CreatedBy = personalDetail.PersonalPkid,
                    CreatedDate = DateTime.Now,
                    //VehicleStandardValue = vehicleStandardValue
                };
                return Create(taxValidation);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error occur when saving taxvalidation: " + e);
                return false;
            }
        }
    }
}
