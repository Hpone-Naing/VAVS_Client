using VAVS_Client.Classes;
using VAVS_Client.Data;
using VAVS_Client.Paging;
using VAVS_Client.Util;

namespace VAVS_Client.Services.Impl
{
    public class TaxValidationServiceImpl : AbstractServiceImpl<TaxValidation>, TaxValidationService
    {

        private readonly ILogger<TaxValidationServiceImpl> _logger;
        private readonly TaxPayerInfoService _taxPayerInfoService;
        public TaxValidationServiceImpl(VAVSClientDBContext context, ILogger<TaxValidationServiceImpl> logger, TaxPayerInfoService taxPayerInfoService) : base(context, logger)
        {

            _logger = logger;
            _taxPayerInfoService = taxPayerInfoService;
        }
        public bool IsTaxedVehicle(string vehicleNumber)
        {
            _logger.LogInformation(">>>>>>>>>> [TaxValidationServiceImpl][IsTaxedVehicle] Find Taxed Vehicle by VehicleNumber. <<<<<<<<<<");

            try
            {
                TaxValidation taxValidation = FindByString("VehicleNumber", vehicleNumber);
                if (taxValidation != null)
                {
                    return true;
                }
                _logger.LogInformation(">>>>>>>>>> Success. Find Taxed Vehicle by VehicleNumber. <<<<<<<<<<");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding Taxed Vehicle by VehicleNumber. <<<<<<<<<<" + e);
                throw;
            }

        }

        public TaxValidation FindTaxValidationByNrc(string nrc)
        {
            _logger.LogInformation(">>>>>>>>>> [TaxValidationServiceImpl][FindTaxValidationByNrc] Find Taxed Vehicle by nrc. <<<<<<<<<<");

            try
            {
                TaxValidation taxValidation = FindByString("PersonNRC", nrc);
                _logger.LogInformation(">>>>>>>>>> Success. Find Taxed Vehicle by nrc. <<<<<<<<<<");
                return taxValidation;

            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding Taxed Vehicle by nrc. <<<<<<<<<<" + e);
                throw;
            }

        }

        public TaxValidation FindTaxValidationByIdEgerLoad(int id)
        {

            try
            {
                TaxValidation taxValidation = _context.TaxValidations.Where(taxValidation => taxValidation.TaxValidationPkid == id && taxValidation.IsDeleted != false)
                    .Include(taxValidation => taxValidation.PersonalDetail)
                    .Include(taxValidation => taxValidation.Township)
                    .Include(taxValidation => taxValidation.Township.StateDivision)
                    .FirstOrDefault();
                return taxValidation;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                throw;
            }

        }

        public PagingList<TaxValidation> GetTaxValidationPendigListPagin(HttpContext httpContext, int? pageNo, int PageSize)
        {
            try
            {
                try
                {
                    LoginUserInfo loginTaxPayerInfo = _taxPayerInfoService.GetLoginUserByHashedToken(SessionUtil.GetToken(httpContext));
                    List<TaxValidation> taxValidationPendingList =  _context.TaxValidations.Where(taxValidation => taxValidation.PersonNRC == loginTaxPayerInfo.TaxpayerInfo.NRC && taxValidation.QRCodeNumber == null && taxValidation.DemandNumber == null).ToList();
                    return GetAllWithPagin(taxValidationPendingList, pageNo, PageSize);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occur " + e);
                    throw;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur. TaxValidation Pending  List paginate eger load list. <<<<<<<<<<" + e);
                throw;
            }
        }

        public  PagingList<TaxValidation> GetTaxValidationApprevedListPagin(HttpContext httpContext, int? pageNo, int PageSize)
        {
            try
            {
                try
                {
                    LoginUserInfo loginTaxPayerInfo = _taxPayerInfoService.GetLoginUserByHashedToken(SessionUtil.GetToken(httpContext));
                    List<TaxValidation> taxValidationPendingList = _context.TaxValidations.Where(taxValidation => taxValidation.PersonNRC == loginTaxPayerInfo.TaxpayerInfo.NRC && taxValidation.QRCodeNumber != null && taxValidation.DemandNumber != null).ToList();
                    return GetAllWithPagin(taxValidationPendingList, pageNo, PageSize);
                }
                catch (Exception e)
                {
                    _logger.LogError(" Error occur " + e);
                    throw;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur. TaxValidation Pending  List paginate eger load list. <<<<<<<<<<" + e);
                throw;
            }
        }
    }
}
