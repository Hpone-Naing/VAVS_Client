using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;
using VAVS_Client.Classes;
using VAVS_Client.Classes.TaxCalculation;
using VAVS_Client.Factories;
using VAVS_Client.Models;
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
            SessionService sessionService = _serviceFactory.CreateSessionServiceService();
            if (!sessionService.IsActiveSession(HttpContext))
            {
                Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
            if (vehicleStandardValue.VehicleNumber != null)
            {
                bool IsTaxed = _serviceFactory.CreateTaxValidationService().IsTaxedVehicle(vehicleStandardValue.VehicleNumber);
                if(IsTaxed)
                {
                    Utility.AlertMessage(this, "This vehicle has already taxed.", "alert-info");
                    return RedirectToAction("SearchVehicleStandardValue", "VehicleStandardValue");
                }
            }
            LoginUserInfo loginUserInfo = new LoginUserInfo()
            {
                TaxVehicleInfo = new TaxVehicleInfo
                {
                    VehicleNumber = vehicleStandardValue?.VehicleNumber,
                    Manufacturer = vehicleStandardValue?.Manufacturer,
                    BuildType = vehicleStandardValue?.BuildType,
                    FuelType = vehicleStandardValue?.Fuel?.FuelType,
                    ModelYear = vehicleStandardValue?.ModelYear,
                    CountryOfMade = vehicleStandardValue?.CountryOfMade,
                    EnginePower = vehicleStandardValue?.EnginePower,
                    VehicleBrand = vehicleStandardValue?.VehicleBrand,
                },
            };

            _serviceFactory.CreateTaxPayerInfoService().CreateLoginUserInfo(SessionUtil.GetToken(HttpContext), loginUserInfo);
            return View("TaxCalculation", vehicleStandardValue);
        }

        private async void MakeViewBag()
        {
            //ViewBag.StateDivisions = _serviceFactory.CreateStateDivisionService().GetSelectListStateDivisions();
            //ViewBag.Townships = _serviceFactory.CreateTownshipService().GetSelectListTownshipsByStateDivision();//factoryBuilder.CreateTownshipService().GetSelectListTownships();
            TaxpayerInfo taxPayerInfo = _serviceFactory.CreateSessionServiceService().GetLoginUserInfo(HttpContext);
            PersonalDetail personalDetail = await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRCInDBAndAPI(taxPayerInfo.NRC);
            ViewBag.RegisteredStateDivisionName = personalDetail.Township.StateDivision.StateDivisionName;
            ViewBag.RegisteredTownshipName = personalDetail.Township.TownshipName;
        }

        [HttpPost]
        public async Task<IActionResult> ShowCalculatedTaxForm(VehicleStandardValue vehicleStandardValue)
        {
            SessionService sessionService = _serviceFactory.CreateSessionServiceService();
            TaxpayerInfo loginTaxPayerInfo = sessionService.GetLoginUserInfo(HttpContext);
            
            if (!sessionService.IsActiveSession(HttpContext))
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
            /*
             * Save Image Files
             */
            if (vehicleStandardValue.IsImageFilesNotNull())
            {
                FileService fileService = _serviceFactory.CreateFileService();
                string NrcFileName = fileService.GetFileName(vehicleStandardValue.NrcImageFile);
                string CensusFileName = fileService.GetFileName(vehicleStandardValue.CensusImageFile);
                string TransactionContractFileName = fileService.GetFileName(vehicleStandardValue.TransactionContractImageFile);
                string OwnerBookFileName = fileService.GetFileName(vehicleStandardValue.OwnerBookImageFile);
                string WheelTabFileName = fileService.GetFileName(vehicleStandardValue.WheelTagImageFile);
                string VehicleFileName = fileService.GetFileName(vehicleStandardValue.VehicleImageFile);
                List<(string fileName, IFormFile file)> files = new List<(string fileName, IFormFile file)> {
                         (NrcFileName, vehicleStandardValue.NrcImageFile),
                         (CensusFileName, vehicleStandardValue.CensusImageFile),
                         (TransactionContractFileName, vehicleStandardValue.TransactionContractImageFile),
                         (OwnerBookFileName, vehicleStandardValue.OwnerBookImageFile),
                         (WheelTabFileName, vehicleStandardValue.WheelTagImageFile),
                         (VehicleFileName, vehicleStandardValue.VehicleImageFile),
                    };
                string savePath = loginTaxPayerInfo.NRC;
                fileService.SaveFile(Utility.ConcatNRCSemiComa(savePath), files);
            }
            string nrc = loginTaxPayerInfo.NRC;
            PersonalDetail personalInformation = await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRCInDBAndAPI(nrc);//await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRC(nrc);
            string contractPriceString = Request.Form["ContractPrice"];
            long ContractPrice = Utility.MakeDigit(contractPriceString);
            long AssetValue = Utility.MakeDigit(vehicleStandardValue.StandardValue);
            long totalTax = _serviceFactory.CreateTaxCalculationService().CalculateTotalTax(ContractPrice, AssetValue);
            TaxValidation taxValidation = new TaxValidation
            {
                VehicleStandardValue = vehicleStandardValue,
                PersonalDetail = personalInformation,
                TaxAmount = totalTax,
                ContractValue = ContractPrice,
            };
            LoginUserInfo loginUserInfo = _serviceFactory.CreateTaxPayerInfoService().GetLoginUserByHashedToken(SessionUtil.GetToken(HttpContext));
            loginUserInfo.TaxVehicleInfo.StandardValue = vehicleStandardValue?.StandardValue;
            loginUserInfo.TaxVehicleInfo.VehicleNumber = vehicleStandardValue?.VehicleNumber;
            loginUserInfo.TaxVehicleInfo.TaxAmount = totalTax.ToString();
            loginUserInfo.TaxVehicleInfo.ContractValue = ContractPrice.ToString();
            _serviceFactory.CreateTaxPayerInfoService().CreateLoginUserInfo(SessionUtil.GetToken(HttpContext), loginUserInfo);
            ViewBag.BaseValue = AssetValue > ContractPrice ? AssetValue.ToString() : ContractPrice.ToString();
            return View("CalculatedTax", taxValidation);
        }

        public  IActionResult ShowTaxOfficeAddressForm()
        {
            SessionService sessionService = _serviceFactory.CreateSessionServiceService();
            if (!sessionService.IsActiveSession(HttpContext))
            {
                Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
            MakeViewBag();
            return View("TaxOfficeAddressForm", new TaxInfo());
        }

        [HttpPost]
        public async Task<IActionResult> SaveTaxTransaction(TaxInfo taxInfo)
        {
            try
            {
                if (!_serviceFactory.CreateSessionServiceService().IsActiveSession(HttpContext))
                {
                    Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                    return RedirectToAction("Index", "Login");
                }
                
                await _serviceFactory.CreateTaxCalculationService().SaveTaxValidation(HttpContext, taxInfo);
                Utility.AlertMessage(this, "Success. Please wait for admin response", "alert-success");
                return RedirectToAction("PendingList", "TaxValidation");
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
