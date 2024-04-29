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
                MakeViewBag();
                Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
            string nrc = loginTaxPayerInfo.TaxpayerInfo.NRC;//SessionUtil.GetLoginUserInfo(HttpContext).TaxpayerInfo.NRC;
            //PersonalInformation personalInformation = await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRC(nrc);
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
                string nrc = loginTaxPayerInfo.TaxpayerInfo.NRC;//SessionUtil.GetLoginUserInfo(HttpContext).TaxpayerInfo.NRC;
                string StandardValue = loginTaxPayerInfo.TaxVehicleInfo.StandardValue;// SessionUtil.GetLoginUserInfo(HttpContext).TaxVehicleInfo.StandardValue;
                string ContractValue = loginTaxPayerInfo.TaxVehicleInfo.ContractValue;//SessionUtil.GetLoginUserInfo(HttpContext).TaxVehicleInfo.ContractValue;
                string TotalTax = loginTaxPayerInfo.TaxVehicleInfo.TaxAmount;//SessionUtil.GetLoginUserInfo(HttpContext).TaxVehicleInfo.TaxAmount;
                string vehicleNumber = loginTaxPayerInfo.TaxVehicleInfo.VehicleNumber;//SessionUtil.GetLoginUserInfo(HttpContext).TaxVehicleInfo.VehicleNumber;
                taxInfo.StandardValue = StandardValue;
                taxInfo.ContractValue = ContractValue;
                taxInfo.TaxAmount = TotalTax;
                await _serviceFactory.CreateTaxCalculationService().SaveTaxValidation(nrc, vehicleNumber, taxInfo);
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
