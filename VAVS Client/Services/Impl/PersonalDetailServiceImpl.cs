using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic.FileIO;
using System.Net.Http;
using System.Net;
using Tesseract;
using VAVS_Client.Classes;
using VAVS_Client.Data;
using VAVS_Client.Models;
using VAVS_Client.Util;
using Newtonsoft.Json;
using VAVS_Client.APIService;
using IPinfo.Models;

namespace VAVS_Client.Services.Impl
{
    public class PersonalDetailServiceImpl : AbstractServiceImpl<PersonalDetail>, PersonalDetailService
    {
        private readonly ILogger<PersonalDetailServiceImpl> _logger;
        private readonly FileService _fileService;
        private readonly HttpClient _httpClient;
        private readonly PersonalDetailAPIService _personalDetailAPIService;

        public PersonalDetailServiceImpl(VAVSClientDBContext context, HttpClient httpClient, FileService fileService, ILogger<PersonalDetailServiceImpl> logger, PersonalDetailAPIService personalDetailAPIService) : base(context, logger)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _logger = logger;
            _personalDetailAPIService = personalDetailAPIService;
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
            Console.WriteLine("here FindPersonalDetailByNrc...................");
            _logger.LogInformation(">>>>>>>>>> [PersonDetailServiceImpl][FindPersonDetailByNrc] Find person by Nrc. <<<<<<<<<<");
            Console.WriteLine("service nrc: " + nrc);
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

    }
}
