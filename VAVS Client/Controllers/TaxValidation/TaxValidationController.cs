using Microsoft.AspNetCore.Mvc;
using VAVS_Client.Classes;
using VAVS_Client.Factories;
using VAVS_Client.Paging;
using VAVS_Client.Services;
using VAVS_Client.Util;

namespace VAVS_Client.Controllers.TaxValidationController
{
    public class TaxValidationController : Controller
    {
        private readonly ServiceFactory _serviceFactory;
        public TaxValidationController(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        public IActionResult PendingList(int? pageNo)
        {
            SessionService sessionService = _serviceFactory.CreateSessionServiceService();
            if (!sessionService.IsActiveSession(HttpContext))
            {
                Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
            int pageSize = Utility.DEFAULT_PAGINATION_NUMBER;
            try
            {
                TaxpayerInfo loginTaxPayerInfo = sessionService.GetLoginUserInfo(HttpContext);
                if(loginTaxPayerInfo.NRC != null)
                {
                    PersonalDetail personalDetail = _serviceFactory.CreatePersonalDetailService().FindPersonalDetailByNrc(Utility.ConcatNRCSemiComa(loginTaxPayerInfo.NRC));
                    ViewBag.Address = personalDetail.HousingNumber + "၊" + personalDetail.Quarter + "၊" + personalDetail.Street + "၊" + personalDetail.Township.TownshipName;
                    
                }
                return View(_serviceFactory.CreateTaxValidationService().GetTaxValidationPendigListPagin(HttpContext, pageNo, pageSize));
            }
            catch (Exception ne)
            {
                Console.WriteLine("Error: " + ne);
                Utility.AlertMessage(this, "Data Issue. Please fill VehicleData in database", "alert-danger");
                return View();
            }

        }

        public IActionResult ApproveList(int? pageNo)
        {
            SessionService sessionService = _serviceFactory.CreateSessionServiceService();
            if (!sessionService.IsActiveSession(HttpContext))
            {
                Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }

            int pageSize = Utility.DEFAULT_PAGINATION_NUMBER;
            try
            {
                TaxpayerInfo loginTaxPayerInfo = sessionService.GetLoginUserInfo(HttpContext);
                if (loginTaxPayerInfo.NRC != null)
                {
                    PersonalDetail personalDetail = _serviceFactory.CreatePersonalDetailService().FindPersonalDetailByNrc(Utility.ConcatNRCSemiComa(loginTaxPayerInfo.NRC));
                    ViewBag.Address = personalDetail.HousingNumber + "၊" + personalDetail.Quarter + "၊" + personalDetail.Street + "၊" + personalDetail.Township.TownshipName;
                    ViewBag.StateDivision = personalDetail.Township.StateDivision.StateDivisionName;
                    ViewBag.Township = personalDetail.Township.TownshipName;
                }
                return View(_serviceFactory.CreateTaxValidationService().GetTaxValidationApprevedListPagin(HttpContext, pageNo, pageSize));
            }
            catch (Exception ne)
            {
                Console.WriteLine("Error: " + ne);
                Utility.AlertMessage(this, "Data Issue. Please fill VehicleData in database", "alert-danger");
                return View();
            }

        }

        public IActionResult Details(int id)
        {
            SessionService sessionService = _serviceFactory.CreateSessionServiceService();
            if (!sessionService.IsActiveSession(HttpContext))
            {
                Utility.AlertMessage(this, "You haven't login yet.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
            Console.WriteLine("id: ..................." + id);
            TaxValidation taxValidation = _serviceFactory.CreateTaxValidationService().FindTaxValidationByIdEgerLoad(id);
            return View(taxValidation);
        }
    }
}
