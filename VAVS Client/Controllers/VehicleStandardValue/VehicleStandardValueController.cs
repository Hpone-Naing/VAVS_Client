using Microsoft.AspNetCore.Mvc;
using VAVS_Client.Classes;
using VAVS_Client.Factories;
using VAVS_Client.Util;
using VAVS_Client.ViewModels;

namespace VAVS_Client.Controllers.VehicleStandardValueController
{
    public class VehicleStandardValueController : Controller
    {
        private readonly ServiceFactory _serviceFactory;
        public VehicleStandardValueController(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        public async Task<IActionResult> SearchVehicleStandardValue()
        {
            try
            {
                LoginUserInfo loginTaxPayerInfo = _serviceFactory.CreateTaxPayerInfoService().GetLoginUserByHashedToken(SessionUtil.GetToken(HttpContext));
                if (loginTaxPayerInfo.IsTaxpayerInfoNull())
                {
                    Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                    return RedirectToAction("Index", "Login");
                }
                string searchString = Request.Query["SearchString"];
                ViewBag.SearchString = searchString;

                if (string.IsNullOrEmpty(searchString))
                    return View();

                VehicleStandardValue vehicleStandardValue = await _serviceFactory.CreateVehicleStandardValueService().GetVehicleValueByVehicleNumberInDBAndAPI(searchString);//await _serviceFactory.CreateVehicleStandardValueService().GetVehicleValueByVehicleNumber(searchString);
                if (vehicleStandardValue == null)
                {
                    ViewBag.SearchString = "Not Found";
                    return View("SearchVehicleStandardValue");
                }
                return View("Details", vehicleStandardValue);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception occur: " + e);
                Utility.AlertMessage(this, "Server Error encounter. Fail to view detail page.", "alert-danger");
                return RedirectToAction(nameof(SearchVehicleStandardValue));
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetVehicleValue()
        {
            try
            {
                LoginUserInfo loginTaxPayerInfo = _serviceFactory.CreateTaxPayerInfoService().GetLoginUserByHashedToken(SessionUtil.GetToken(HttpContext));
                if (loginTaxPayerInfo.IsTaxpayerInfoNull())
                {
                    Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                    return RedirectToAction("Index", "Login");
                }

                string manufacturer = Request.Form["manufacturer"];
                string buildType = Request.Form["buildType"];
                string fuelType = Request.Form["fuelType"];
                string vehicleBrand = Request.Form["vehicleBrand"];
                string modelYear = Request.Form["modelYear"];
                string enginePower = Request.Form["enginePower"];

                VehicleStandardValue vehicleStandardValue = await _serviceFactory.CreateVehicleStandardValueService().GetVehicleValue(manufacturer, buildType, fuelType, vehicleBrand, modelYear, enginePower);
                if(vehicleStandardValue == null)
                {
                    ViewBag.SearchString = "Not Found";
                    return View("SearchVehicleStandardValue");
                }
                return View("Details", vehicleStandardValue);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception...." + e);
                Utility.AlertMessage(this, "Server Error encounter. Fail to view detail page.", "alert-danger");
                return RedirectToAction(nameof(SearchVehicleStandardValue));
            }
        }
        public IActionResult Details(int Id)
        {
            
            try
            {
                VehicleStandardValue vehicleStandardValue = _serviceFactory.CreateVehicleStandardValueService().FindVehicleStandardValueByIdEgerLoad(Id);
                if (vehicleStandardValue != null)
                {
                    return View(vehicleStandardValue);
                }
                else
                {
                    Utility.AlertMessage(this, "Server Error encounter. Fail to view detail page.", "alert-danger");
                    return RedirectToAction(nameof(SearchVehicleStandardValue));
                }
            }
            catch (Exception e)
            {
                Utility.AlertMessage(this, "Server Error encounter. Fail to view detail page.", "alert-danger");
                return RedirectToAction(nameof(SearchVehicleStandardValue));
            }
        }
    }
}
