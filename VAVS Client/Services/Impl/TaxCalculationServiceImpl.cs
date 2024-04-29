
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
                /*
                 * Find PersonalDetail from Database and api
                 */
                PersonalDetail personalDetail = await _personalDetailService.GetPersonalInformationByNRCInDBAndAPI(loginTaxPayerInfo.TaxpayerInfo.NRC);
                Township township = _townshipService.FindTownshipByPkId(taxInfo.TownshipPkid);
                StateDivision stateDivision = _staetDivisionService.FindStateDivisionByPkId(taxInfo.StateDivisionPkid);
                personalDetail.Township = township;
                personalDetail.StateDivision = stateDivision;
                if (personalDetail.PersonalPkid == null)
                {
                    _personalDetailService.CreatePersonalDetail(personalDetail);
                }
                VehicleStandardValue vehicleStandardValue = await _vehicleStandardValueService.GetVehicleValueByVehicleNumberInDBAndAPI(loginTaxPayerInfo.TaxVehicleInfo.VehicleNumber);
                TaxValidation taxValidation = new TaxValidation
                {
                    PersonTINNumber = personalDetail?.PersonTINNumber,
                    PersonNRC = loginTaxPayerInfo.TaxpayerInfo.NRC,
                    VehicleNumber = loginTaxPayerInfo.TaxVehicleInfo.VehicleNumber,
                    Manufacturer = vehicleStandardValue?.Manufacturer,
                    CountryOfMade = vehicleStandardValue?.CountryOfMade,
                    VehicleBrand = vehicleStandardValue?.VehicleBrand,
                    BuildType = vehicleStandardValue?.BuildType,
                    ModelYear = vehicleStandardValue?.ModelYear,
                    EnginePower = vehicleStandardValue?.EnginePower,
                    FuelType = vehicleStandardValue?.Fuel?.FuelType,
                    StandardValue = decimal.Parse(loginTaxPayerInfo.TaxVehicleInfo.StandardValue),
                    ContractValue = decimal.Parse(loginTaxPayerInfo.TaxVehicleInfo.ContractValue),
                    TaxAmount = decimal.Parse(loginTaxPayerInfo.TaxVehicleInfo.TaxAmount),
                    OfficeLetterNo = vehicleStandardValue?.OfficeLetterNo,
                    AttachFileName = vehicleStandardValue?.AttachFileName,
                    EntryDate = vehicleStandardValue?.EntryDate,
                    PersonalDetail = personalDetail,
                    Township = township,
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
