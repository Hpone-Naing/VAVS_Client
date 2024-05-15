using VAVS_Client.Data;
using VAVS_Client.Util;
using VAVS_Client.APIService;
using VAVS_Client.Classes;

namespace VAVS_Client.Services.Impl
{
    public class PersonalDetailServiceImpl : AbstractServiceImpl<PersonalDetail>, PersonalDetailService
    {
        private readonly ILogger<PersonalDetailServiceImpl> _logger;
        private readonly FileService _fileService;
        private readonly HttpClient _httpClient;
        private readonly PersonalDetailAPIService _personalDetailAPIService;
        private readonly TaxValidationService _taxValidationService;

        public PersonalDetailServiceImpl(VAVSClientDBContext context, HttpClient httpClient, FileService fileService, ILogger<PersonalDetailServiceImpl> logger, PersonalDetailAPIService personalDetailAPIService, TaxValidationService taxValidationService) : base(context, logger)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _logger = logger;
            _personalDetailAPIService = personalDetailAPIService;
            _taxValidationService = taxValidationService;
        }

        public bool CreatePersonalDetail(PersonalDetail personalDetail)
        {
            _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][CreatePersonDetail] Create PersonDetail. <<<<<<<<<<");
            try
            {

                personalDetail.IsDeleted = false;
                personalDetail.RegistrationStatus = "Pending";
                personalDetail.PhoneNumber = personalDetail.MakePhoneNumberWithCountryCode();
                personalDetail.CreatedBy = 1;
                personalDetail.CreatedDate = DateTime.Now;
                _logger.LogInformation($">>>>>>>>>> Success. Create PersonDetail. <<<<<<<<<<");
                return Create(personalDetail);
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when creating PersonDetail. <<<<<<<<<<" + e);
                throw;
            }
        }

        public PersonalDetail FindPersonalDetailByNrc(string nrc)
        {
            Console.WriteLine("service nrc: " + nrc);
            Console.WriteLine("here FindPersonalDetailByNrc...................");
            _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][FindPersonDetailByNrc] Find person by Nrc. <<<<<<<<<<");
            try
            {
                string NRCTownshipNumber = nrc.Split(";")[0];
                string NRCTownshipInitial = nrc.Split(";")[1];
                string NRCType = nrc.Split(";")[2];
                string NRCNumber = nrc.Split(";")[3];
                PersonalDetail personalDetail = _context.PersonalDetails
                    .FirstOrDefault(personalDetil =>
                        personalDetil.IsDeleted == false &&
                        personalDetil.NRCTownshipNumber == NRCTownshipNumber &&
                        personalDetil.NRCTownshipInitial == NRCTownshipInitial &&
                        personalDetil.NRCType == NRCType &&
                        personalDetil.NRCNumber == NRCNumber);
                _logger.LogInformation($">>>>>>>>>> Success. Find person by Nrc. <<<<<<<<<<");
                Console.WriteLine("personal Detail == null? " + (personalDetail == null));
                return personalDetail;
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by Nrc. <<<<<<<<<<" + e);
                throw;
            }
        }

        public PersonalDetail FindPersonalDetailByPhoneNumber(string phoneNumber)
        {
            _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][FindPersonalDetailByPhoneNumber] Find person by phone number. <<<<<<<<<<");
            try
            {
                PersonalDetail personalDetail = _context.PersonalDetails
                    .FirstOrDefault(personalDetil =>
                        personalDetil.IsDeleted == false &&
                        personalDetil.PhoneNumber == Utility.MakePhoneNumberWithCountryCode(phoneNumber)
                        );
                _logger.LogInformation($">>>>>>>>>> Success. Find person by phone number. <<<<<<<<<<");
                return personalDetail;
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by phone number. <<<<<<<<<<" + e);
                throw;
            }
        }

        public async Task<PersonalDetail> GetPersonalInformationByNRC(string nrc)
        {
            _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][GetPersonalInformationByNRC] Get personal information by nrc. <<<<<<<<<<");
            try
            {
                //PersonalInformation personalInfo = await _personalDetailAPIService.GetPersonalInformationByNRC(nrc);
                PersonalDetail personalInfo = await _personalDetailAPIService.GetPersonalInformationByNRC(nrc);
                return personalInfo;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by phone number. <<<<<<<<<<" + e);
                throw new HttpRequestException($"Failed to send message. Status code: {e.StatusCode}");
            }
        }

        public async Task<PersonalDetail> GetPersonalInformationByNRCInDBAndAPI(string nrc)
        {
            try
            {
                string nrcConcatWithSemiComa = Utility.ConcatNRCSemiComa(nrc);
                Console.WriteLine("Nrc concat seicoma" + nrcConcatWithSemiComa);
                PersonalDetail personalDetail = FindPersonalDetailByNrc(nrcConcatWithSemiComa);
                Console.WriteLine("personal Detail == null? " + (personalDetail == null));
                if (personalDetail == null)
                {
                    personalDetail = await GetPersonalInformationByNRC(nrc);
                }
                return personalDetail;
            }
            catch(Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by nrc. <<<<<<<<<<" + e);
                throw;
            }
        }

        public async Task<PersonalDetail> GetPersonalInformationByPhoneNumber(string phoneNumber)
        {
            _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][GetPersonalInformationByPhoneNumber] Get personal information by phoneNumber. <<<<<<<<<<");
            try
            {
                //PersonalInformation personalInfo = await _personalDetailAPIService.GetPersonalInformationByNRC(nrc);
                PersonalDetail personalInfo = await _personalDetailAPIService.GetPersonalInformationByNRC(phoneNumber);
                return personalInfo;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by phone number. <<<<<<<<<<" + e);
                throw new HttpRequestException($"Failed to send message. Status code: {e.StatusCode}");
            }
        }

        public async Task<PersonalDetail> GetPersonalInformationByPhoneNumberInDBAndAPI(string phoneNumber)
        {
            try
            {
                _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][GetPersonalInformationByPhoneNumberInDBAndAPI] Get personal information by phoneNumber in database and api. <<<<<<<<<<");
                string phoneNumberWithCountryCode = Utility.MakePhoneNumberWithCountryCode(phoneNumber);
                PersonalDetail personalDetail = FindPersonalDetailByPhoneNumber(phoneNumberWithCountryCode);
                if (personalDetail == null)
                {
                    personalDetail = await GetPersonalInformationByPhoneNumber(phoneNumber);
                }
                return personalDetail;
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by phonenumber in database and api. <<<<<<<<<<" + e);
                throw;
            }
        }

        public bool UpdatePhoneNumberByNrc(string nrc, string oldPhoneNumber, string newPhoneNumber)
        {
            try
            {
                _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][UpdatePhoneNumberByNrc]Update phoneNumber by nrc in database. <<<<<<<<<<");
                Console.WriteLine("updated ph no nrc: " + nrc);
                PersonalDetail personalDetail = FindPersonalDetailByNrc(nrc);
                if (personalDetail == null)
                    return false;
                if (personalDetail.PhoneNumber != oldPhoneNumber)
                    return false;
                personalDetail.PhoneNumber = newPhoneNumber;
                Update(personalDetail);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when finding person by phonenumber in database and api. <<<<<<<<<<" + e);
                throw;
            }
        }

        public async Task<bool> AllowResetPhonenumber(ResetPhonenumber resetPhonenumber)
        {
            try
            {
                _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][AllowResetPhonenumber] Check allow Reset phonenumber. <<<<<<<<<<");

                PersonalDetail personalDetail = await GetPersonalInformationByNRCInDBAndAPI(resetPhonenumber.Nrc);
                if (personalDetail == null)
                    return false;

                TaxValidation taxValidation = _taxValidationService.FindTaxValidationByNrc(resetPhonenumber.Nrc);
                if (taxValidation == null)
                    return false;
                
                if (taxValidation.VehicleNumber == resetPhonenumber.TaxedVehicleNumber)
                {
                    if (personalDetail.PersonalPkid != null)
                    {
                        string concatNrcSemiComa = Utility.ConcatNRCSemiComa(resetPhonenumber.Nrc);
                        return UpdatePhoneNumberByNrc(concatNrcSemiComa, resetPhonenumber.OldPhonenumber, resetPhonenumber.NewPhonenumber);
                    }
                    bool result = await _personalDetailAPIService.ResetPhoneNumber(resetPhonenumber.Nrc, resetPhonenumber.OldPhonenumber, resetPhonenumber.NewPhonenumber);
                    return result;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur allow reset phonenumber. <<<<<<<<<<" + e);
                throw;
            }
        }
        public async Task<bool> ResetPhoneNumber(ResetPhonenumber resetPhonenumber)
        {
            try
            {
                _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][ResetPhoneNumber] Reset phonenumber. <<<<<<<<<<");
                PersonalDetail personalDetail = await GetPersonalInformationByNRCInDBAndAPI(resetPhonenumber.Nrc);
                if (personalDetail == null)
                    return false;

                TaxValidation taxValidation = _taxValidationService.FindTaxValidationByNrc(resetPhonenumber.Nrc);
                if (taxValidation == null)
                    return false;

                if (taxValidation.VehicleNumber == resetPhonenumber.TaxedVehicleNumber)
                {
                    if (personalDetail.PersonalPkid != null)
                    {
                        return UpdatePhoneNumberByNrc(Utility.ConcatNRCSemiComa(resetPhonenumber.Nrc), Utility.MakePhoneNumberWithCountryCode(resetPhonenumber.OldPhonenumber), Utility.MakePhoneNumberWithCountryCode(resetPhonenumber.NewPhonenumber));
                    }
                    bool result = await _personalDetailAPIService.ResetPhoneNumber(resetPhonenumber.Nrc, Utility.MakePhoneNumberWithCountryCode(resetPhonenumber.OldPhonenumber), Utility.MakePhoneNumberWithCountryCode(resetPhonenumber.NewPhonenumber));
                    return result;
                }
                return false;
            }
            catch(Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when reset phonenumber. <<<<<<<<<<" + e);
                throw;
            }
        }
    }
}
