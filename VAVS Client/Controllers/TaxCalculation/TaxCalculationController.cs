using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;
using VAVS_Client.Classes;
using VAVS_Client.Classes.TaxCalculation;
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
            if(vehicleStandardValue.VehicleNumber != null)
            {
                bool IsTaxed = _serviceFactory.CreateTaxValidationService().IsTaxedVehicle(vehicleStandardValue.VehicleNumber);
                if(IsTaxed)
                {
                    Utility.AlertMessage(this, "This vehicle has already taxed.", "alert-info");
                    return RedirectToAction("SearchVehicleStandardValue", "VehicleStandardValue");
                }
            }
            return View("TaxCalculation", vehicleStandardValue);
        }

        private void MakeViewBag()
        {
            ViewBag.StateDivisions = _serviceFactory.CreateStateDivisionService().GetSelectListStateDivisions();
            ViewBag.Townships = _serviceFactory.CreateTownshipService().GetSelectListTownshipsByStateDivision();//factoryBuilder.CreateTownshipService().GetSelectListTownships();
        }

        [HttpPost]
        public async Task<IActionResult> ShowCalculatedTaxForm(VehicleStandardValue vehicleStandardValue)
        {
            LoginUserInfo loginTaxPayerInfo = _serviceFactory.CreateTaxPayerInfoService().GetLoginUserByHashedToken(SessionUtil.GetToken(HttpContext));
            if (loginTaxPayerInfo.IsTaxpayerInfoNull())
            {
                Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
            if (vehicleStandardValue.VehicleNumber != null)
            {
                bool IsTaxed = _serviceFactory.CreateTaxValidationService().IsTaxedVehicle(vehicleStandardValue.VehicleNumber);
                if (IsTaxed)
                {
                    Utility.AlertMessage(this, "This vehicle has already taxed.", "alert-info");
                    return RedirectToAction("SearchVehicleStandardValue", "VehicleStandardValue");
                }
            }
            string nrc = loginTaxPayerInfo.TaxpayerInfo.NRC;
            PersonalDetail personalInformation = await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRCInDBAndAPI(nrc);//await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRC(nrc);
            string contractPriceString = Request.Form["ContractPrice"];
            long ContractPrice = Utility.MakeDigit(contractPriceString);
            long AssetValue = Utility.MakeDigit(vehicleStandardValue.StandardValue, "true");
            long totalTax = _serviceFactory.CreateTaxCalculationService().CalculateTotalTax(ContractPrice, AssetValue);
            TaxValidation taxValidation = new TaxValidation
            {
                VehicleStandardValue = vehicleStandardValue,
                PersonalDetail = personalInformation,
                TaxAmount = totalTax,
                ContractValue = ContractPrice,
            };
            loginTaxPayerInfo.TaxVehicleInfo = new TaxVehicleInfo {
                VehicleNumber = vehicleStandardValue.VehicleNumber,
                Manufacturer = vehicleStandardValue.Manufacturer,
                BuildType = vehicleStandardValue.BuildType,
                FuelType = vehicleStandardValue.Fuel.FuelType,
                ModelYear = vehicleStandardValue.ModelYear,
                CountryOfMade = vehicleStandardValue.CountryOfMade,
                EnginePower = vehicleStandardValue.EnginePower,
                VehicleBrand = vehicleStandardValue.VehicleBrand,
                StandardValue = vehicleStandardValue.StandardValue,
                TaxAmount = totalTax.ToString(),
                ContractValue = ContractPrice.ToString(),
            };
            _serviceFactory.CreateTaxPayerInfoService().CreateLoginUserInfo(SessionUtil.GetToken(HttpContext),loginTaxPayerInfo);
            return View("CalculatedTax", taxValidation);
        }

        public  IActionResult ShowTaxOfficeAddressForm()
        {
            MakeViewBag();
            return View("TaxOfficeAddressForm", new TaxInfo());
        }

        [HttpPost]
        public async Task<IActionResult> SaveTaxTransaction(TaxInfo taxInfo)
        {
            try
            {
                LoginUserInfo loginTaxPayerInfo = _serviceFactory.CreateTaxPayerInfoService().GetLoginUserByHashedToken(SessionUtil.GetToken(HttpContext));
                if (loginTaxPayerInfo.IsTaxpayerInfoNull())
                {
                    Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                    return RedirectToAction("Index", "Login");
                }
                if (loginTaxPayerInfo.IsTaxVehicleInfoNull())
                {
                    Utility.AlertMessage(this, "You haven't search your vehicle.", "alert-danger");
                    return RedirectToAction("SearchVehicleStandardValue", "VehicleStandardValue");
                }
                await _serviceFactory.CreateTaxCalculationService().SaveTaxValidation(HttpContext, taxInfo);
                Utility.AlertMessage(this, "Success. Please wait for admin response", "alert-success");
                return RedirectToAction("SearchVehicleStandardValue", "VehicleStandardValue");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occur when saving taxValidation" + e);
                Utility.AlertMessage(this, "Internal server error.", "alert-danger");
                return RedirectToAction("SearchVehicleStandardValue", "VehicleStandardValue");
                
            }
        }
    }
}
