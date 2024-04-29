
using VAVS_Client.Classes.TaxCalculation;
using VAVS_Client.Data;
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
        public TaxCalculationServiceImpl(VAVSClientDBContext context, HttpClient httpClient, ILogger<TaxCalculationServiceImpl> logger, PersonalDetailService personalDetailService, TownshipService townshipService, StateDivisionService staetDivisionService, VehicleStandardValueService vehicleStandardValueService, VehicleStandardValueService vehicleStandardValueService1, FuelTypeService fuelTypeService) : base(context, logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _personalDetailService = personalDetailService;
            _townshipService = townshipService;
            _staetDivisionService = staetDivisionService;
            _vehicleStandardValueService = vehicleStandardValueService;
            _fuelTypeService = fuelTypeService;
        }

        public long CalculateTotalTax(long contractPrice, long assetValue)
        {
            long maximumValue = (contractPrice > assetValue) ? contractPrice : assetValue;
            VehicleTaxCalculation taxCalculation = new VehicleTaxCalculation();
            return taxCalculation.CalculateTax(maximumValue);
        }

        public async Task<bool> SaveTaxValidation(string nrc, string vehicleNumber, TaxInfo taxInfo)
        {

            try
            {
                VehicleStandardValue vehicleStandardValue;
                /*
                 * Find PersonalDetail from Database
                 */
                PersonalDetail personalDetail = await _personalDetailService.GetPersonalInformationByNRCInDBAndAPI(nrc);
                Township township = _townshipService.FindTownshipByPkId(taxInfo.TownshipPkid);
                StateDivision stateDivision = _staetDivisionService.FindStateDivisionByPkId(taxInfo.StateDivisionPkid);
                
                personalDetail.Township = township;
                personalDetail.StateDivision = stateDivision;
                if (personalDetail.PersonalPkid == null)
                {
                    _personalDetailService.CreatePersonalDetail(personalDetail);
                }

                vehicleStandardValue = _vehicleStandardValueService.FindVehicleByVehicleNumberEgerLoad(vehicleNumber);
                if (vehicleStandardValue == null)
                {
                    vehicleStandardValue = await _vehicleStandardValueService.GetVehicleValueByVehicleNumber(vehicleNumber);
                    Fuel fuel = _fuelTypeService.FindFuelByFuelType(vehicleStandardValue.Fuel.FuelType);
                    vehicleStandardValue.StateDivision = personalDetail.StateDivision;
                    vehicleStandardValue.Fuel = fuel;
                    //_vehicleStandardValueService.CreateVehicleStandardValue(vehicleStandardValue);
                }

                TaxValidation taxValidation = new TaxValidation
                {
                    PersonTINNumber = personalDetail?.PersonTINNumber,
                    PersonNRC = nrc,
                    VehicleNumber = vehicleNumber,
                    Manufacturer = vehicleStandardValue?.Manufacturer,
                    CountryOfMade = vehicleStandardValue?.CountryOfMade,
                    VehicleBrand = vehicleStandardValue?.VehicleBrand,
                    BuildType = vehicleStandardValue?.BuildType,
                    ModelYear = vehicleStandardValue?.ModelYear,
                    EnginePower = vehicleStandardValue?.EnginePower,
                    FuelType = vehicleStandardValue?.Fuel?.FuelType,
                    StandardValue = decimal.Parse(taxInfo.StandardValue),
                    ContractValue = decimal.Parse(taxInfo.ContractValue),
                    TaxAmount = decimal.Parse(taxInfo.TaxAmount),
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
