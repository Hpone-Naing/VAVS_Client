using Microsoft.AspNetCore.Mvc;
using VAVS_Client.Classes;
using VAVS_Client.Factories;
using VAVS_Client.Util;

namespace VAVS_Client.Controllers.TaxCalculation
{
    public class TaxCalculationController : Controller
    {
        public ServiceFactory _serviceFactory;

        public TaxCalculationController(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        [HttpPost]
        public IActionResult ShowCalculateTaxForm(VehicleStandardValue vehicleStandardValue)
        {
            return View("TaxCalculation", vehicleStandardValue);
        }

        [HttpPost]
        public async Task<IActionResult> ShowCalculatedTaxForm(VehicleStandardValue vehicleStandardValue)
        {
            string nrc = SessionUtil.GetLoginUserInfo(HttpContext).NRC;
            PersonalInformation personalInformation = await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRC(nrc);
            string contractPriceString = Request.Form["ContractPrice"];
            long ContractPrice = Utility.MakeDigit(contractPriceString);
            long AssetValue = Utility.MakeDigit(vehicleStandardValue.StandardValue);
            long totalTax = _serviceFactory.CreateTaxCalculationService().CalculateTotalTax(ContractPrice, AssetValue);

            TaxpayerInfo taxpayerInfo = new TaxpayerInfo
            {
                BuildType = vehicleStandardValue.BuildType,
                VehicleBrand = vehicleStandardValue.VehicleBrand,
                ModelYear = vehicleStandardValue.ModelYear,
                EnginePower = vehicleStandardValue.EnginePower,
                FuelType = vehicleStandardValue.Fuel.FuelType,
                Name = personalInformation.Name,
                PhoneNumber = personalInformation.PhoneNumber,
                NRC = nrc,
                HousingNumber = personalInformation.HousingNumber,
                Quarter = personalInformation.Quarter,
                Street = personalInformation.Street,
                TotalTax = totalTax
            };
            return View("CalculatedTax", taxpayerInfo);
        }
    }
}
