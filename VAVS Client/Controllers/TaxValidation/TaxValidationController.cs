using Microsoft.AspNetCore.Mvc;
using VAVS_Client.Factories;
using VAVS_Client.Paging;
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
            int pageSize = Utility.DEFAULT_PAGINATION_NUMBER;
            try
            {
                
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
            int pageSize = Utility.DEFAULT_PAGINATION_NUMBER;
            try
            {
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

            Console.WriteLine("id: ..................." + id);
            TaxValidation taxValidation = _serviceFactory.CreateTaxValidationService().FindTaxValidationByIdEgerLoad(id);
            Console.WriteLine("tax validation null? " + (taxValidation == null));
            return View(taxValidation);
        }
    }
}
