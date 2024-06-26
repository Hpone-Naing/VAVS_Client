﻿using Microsoft.AspNetCore.Mvc;
using VAVS_Client.Classes;
using VAVS_Client.Factories;
using VAVS_Client.Util;
using Tesseract;
using System.Drawing;
using System.Drawing.Imaging;

namespace VAVS_Client.Controllers.Auth
{
    public class RegistrationAuthCodeController : Controller
    {
        private readonly ServiceFactory _serviceFactory;

        public RegistrationAuthCodeController(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        
        private void MakeViewBag()
        {
            ViewBag.StateDivisions = _serviceFactory.CreateStateDivisionService().GetSelectListStateDivisions();
            ViewBag.Townships = _serviceFactory.CreateTownshipService().GetSelectListTownships();
        }

        private DeviceInfo InitializeDeviceInfo(string ipAddress, string publicIpAddress, string hashedOtp)
        {
            return new DeviceInfo()
            {
                IpAddress = ipAddress,
                PublicIpAddress = publicIpAddress,
                RegistrationCount = 0,
                ResendCodeTime = 0,
                OTP = hashedOtp
            };
        }
        private Otp MakeOtp(HttpContext httpContext)
        {
            string digit1 = httpContext.Request.Form["digit1"];
            string digit2 = httpContext.Request.Form["digit2"];
            string digit3 = httpContext.Request.Form["digit3"];
            string digit4 = httpContext.Request.Form["digit4"];
            string digit5 = httpContext.Request.Form["digit5"];
            string digit6 = httpContext.Request.Form["digit6"];
            return new Otp()
            {
                Digit1 = digit1,
                Digit2 = digit2,
                Digit3 = digit3,
                Digit4 = digit4,
                Digit5 = digit5,
                Digit6 = digit6
            };
        }

        
        
        [HttpPost]
        public async Task<IActionResult> CheckRegisterAuthentication(PersonalDetail personalDetail, bool resend)
        {
            try
            {
                /*
                 * Check vpn turn on or off
                 */
                /*bool isUseVpn = await _serviceFactory.CreateDeviceInfoService().VpnTurnOn();                
                if(isUseVpn)
                {
                    MakeViewBag();
                    Utility.AlertMessage(this, "Please turn off vpn.", "alert-danger", "true");
                    return RedirectToAction("Index", "Login");
                }*/
                /* 
                 * Check user already register or not by nrc 
                 */
                if (await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByNRCInDBAndAPI(Utility.MakeNRC(personalDetail.NRCTownshipNumber+"/", personalDetail.NRCTownshipInitial, personalDetail.NRCType, personalDetail.NRCNumber)) != null)
                {
                    Console.WriteLine("here nrc in db and api not null");
                    MakeViewBag();
                    Utility.AlertMessage(this, "Already registered.", "alert-primary", "true");
                    return RedirectToAction("LoginUser", "Login");
                }
                /* 
                 * Check user already register or not by phone number 
                 */
                if (await _serviceFactory.CreatePersonalDetailService().GetPersonalInformationByPhoneNumberInDBAndAPI(personalDetail.PhoneNumber) != null)
                {
                    Console.WriteLine("here phone in db and api not null");

                    MakeViewBag();
                    Utility.AlertMessage(this, "Already registered.", "alert-primary", "true");
                    return RedirectToAction("LoginUser", "Login");
                }
                Console.WriteLine("nrc tonhip number..............." + personalDetail.NRCTownshipNumber);
                var ipAddress = Utility.GetIPAddress();
                string publicIpAddress = await _serviceFactory.CreateDeviceInfoService().GetPublicIPAddress();
                DateTime currentTime = DateTime.Now;
                DateTime expireTime = currentTime.AddSeconds(Utility.OTP_EXPIRE_SECOND);
                string storedExpireTime = HttpContext.Session.GetString("ExpireTime");
                /*
                 * Save Nrc photo
                 */
                if(personalDetail.NrcFrontImageFile != null && personalDetail.NrcBackImageFile != null)
                {
                    FileService fileService = _serviceFactory.CreateFileService();
                    string frontFileName = fileService.GetFileName(personalDetail.NrcFrontImageFile);
                    string backFileName = fileService.GetFileName(personalDetail.NrcBackImageFile);
                    List<(string fileName, IFormFile file)> files = new List<(string fileName, IFormFile file)> {
                         (frontFileName, personalDetail.NrcFrontImageFile),
                         (backFileName, personalDetail.NrcBackImageFile)
                    };
                    personalDetail.NRCFrontImagePath = frontFileName;
                    personalDetail.NRCBackImagePath = backFileName;
                    fileService.SaveFile(personalDetail.MakeNrc(), files);
                }
                /*
                 * Check User's deviceInfo null or not
                 */
                DeviceInfo existingDeviceInfo = _serviceFactory.CreateDeviceInfoService().GetDeviceInfoByIPAddress(ipAddress);
                if (existingDeviceInfo == null)
                {
                    string otp = Utility.GenerateOtp();
                    Console.WriteLine("Otp is1.............: " + otp);
                    /*
                     * Send otp code via sms
                     */
                    await _serviceFactory.CreateSMSVerificationService().SendSMSOTP(Utility.MakePhoneNumberWithCountryCode(personalDetail.PhoneNumber), Utility.MakeMessage("Your OTP code is: ", otp));
                    
                    _serviceFactory.CreateDeviceInfoService().CreateDeviceInfo(InitializeDeviceInfo(ipAddress, publicIpAddress, HashUtil.ComputeSHA256Hash(otp)));
                    HttpContext.Session.SetString("ExpireTime", expireTime.ToString("yyyy-MM-ddTHH:mm:ss"));
                    ViewData["ExpireTime"] = HttpContext.Session.GetString("ExpireTime");
                    Console.WriteLine("nrctonhipno:........................" + personalDetail.NRCTownshipNumber);
                    return View("RegistrationAuthCode", personalDetail);
                }
                /*
                 * Check user can re register or not
                 */
                if (existingDeviceInfo.ReRegistrationTime != null)
                {
                    if (existingDeviceInfo.IsExceedMaximunRegistration() || !existingDeviceInfo.AllowNextTimeRegister())
                    {
                        _serviceFactory.CreateDeviceInfoService().UpdateRegistrationTime(ipAddress);
                        Utility.AlertMessage(this, "Try re register after" + DateTime.Parse(existingDeviceInfo.ReRegistrationTime).ToString(), "alert-danger", "true");
                        return RedirectToAction("LoginUser", "Login");
                    }
                    if (existingDeviceInfo.AllowNextTimeRegister())
                    {
                        _serviceFactory.CreateDeviceInfoService().UpdateRegistrationTime(ipAddress);
                        HttpContext.Session.SetString("ExpireTime", expireTime.ToString("yyyy-MM-ddTHH:mm:ss"));
                        ViewData["ExpireTime"] = HttpContext.Session.GetString("ExpireTime");
                        Console.WriteLine("nrctonhipno:........................" + personalDetail.NRCTownshipNumber);

                        return View("RegistrationAuthCode", personalDetail);
                    }
                }
                /*
                 * Check user can request re resend code or not
                 */
                if (existingDeviceInfo.ReResendCodeTime != null)
                {
                    if (existingDeviceInfo.IsExceedMaximunResendCode() || !existingDeviceInfo.AllowNextTimeResendOTP())
                    {
                        _serviceFactory.CreateDeviceInfoService().UpdateResendCodeTime(ipAddress);
                        _serviceFactory.CreateFileService().DeleteDirectory(personalDetail.MakeNrc());
                        Utility.AlertMessage(this, "Try resend code after " + DateTime.Parse(existingDeviceInfo.ReResendCodeTime).ToString(), "alert-danger", "true");
                        return RedirectToAction("LoginUser", "Login");
                    }
                    if (existingDeviceInfo.AllowNextTimeResendOTP())
                    {
                        string otp = Utility.GenerateOtp();
                        Console.WriteLine("Otp is11..................: " + otp);
                        /*
                         * Send otp code via sms
                         */
                        await _serviceFactory.CreateSMSVerificationService().SendSMSOTP(Utility.MakePhoneNumberWithCountryCode(personalDetail.PhoneNumber), Utility.MakeMessage("Your OTP code is: ", otp));

                        _serviceFactory.CreateDeviceInfoService().UpdateResendCodeTime(ipAddress, HashUtil.ComputeSHA256Hash(otp));
                        HttpContext.Session.SetString("ExpireTime", expireTime.ToString("yyyy-MM-ddTHH:mm:ss"));
                        ViewData["ExpireTime"] = HttpContext.Session.GetString("ExpireTime");
                        Console.WriteLine("nrctonhipno:........................" + personalDetail.NRCTownshipNumber);

                        return View("RegistrationAuthCode", personalDetail);
                    }
                }
                /*
                 * OTP code expire and user can request re resend code
                 */
                if ((string.IsNullOrEmpty(storedExpireTime) || (!string.IsNullOrEmpty(storedExpireTime) && currentTime > DateTime.Parse(storedExpireTime))))
                {
                    string otp = Utility.GenerateOtp();
                    Console.WriteLine("Otp is3.................................: " + otp);
                    /*
                     * Send otp code via sms
                     */
                    await _serviceFactory.CreateSMSVerificationService().SendSMSOTP(Utility.MakePhoneNumberWithCountryCode(personalDetail.PhoneNumber), Utility.MakeMessage("Your OTP code is: ", otp));

                    if (resend)
                    {
                        _serviceFactory.CreateDeviceInfoService().UpdateResendCodeTime(ipAddress, HashUtil.ComputeSHA256Hash(otp));
                    } else
                    {
                        _serviceFactory.CreateDeviceInfoService().UpdateOtp(ipAddress, HashUtil.ComputeSHA256Hash(otp));
                    }
                    HttpContext.Session.SetString("ExpireTime", expireTime.ToString("yyyy-MM-ddTHH:mm:ss"));
                    ViewData["ExpireTime"] = HttpContext.Session.GetString("ExpireTime");
                    Console.WriteLine("nrctonhipno:........................" + personalDetail.NRCTownshipNumber);

                    return View("RegistrationAuthCode", personalDetail);
                }
                /*
                 * OTP code not expire and check valid OTP or not. If valid save PersonalDetail
                 */
                if (currentTime < DateTime.Parse(storedExpireTime))
                {
                    if (MakeOtp(HttpContext).IsValidOtp(existingDeviceInfo.OTP))
                    {
                        /*
                         * Valid OTP and send Username and Password for login via sms
                         */
                        if (_serviceFactory.CreatePersonalDetailService().CreatePersonalDetail(personalDetail))
                        {
                            _serviceFactory.CreateDeviceInfoService().UpdateRegistrationTime(ipAddress);
                            HttpContext.Session.Remove("ExpireTime");
                            Utility.AlertMessage(this, "Registration Success. Please Enter NRC number to login.", "alert-success", "true");
                            return RedirectToAction("LoginUser", "Login");
                        }
                        MakeViewBag();
                        Utility.AlertMessage(this, "Registration Fail. Internal Server error", "alert-danger");
                        return RedirectToAction("Index", "Login");
                    }
                    ViewData["ExpireTime"] = HttpContext.Session.GetString("ExpireTime");
                    Utility.AlertMessage(this, "Incorrect OTP code.", "alert-danger");
                    return View("RegistrationAuthCode", personalDetail);
                }

                ViewData["ExpireTime"] = HttpContext.Session.GetString("ExpireTime");
                Utility.AlertMessage(this, "OTP code expire.", "alert-danger");
                return View("RegistrationAuthCode", personalDetail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _serviceFactory.CreateFileService().DeleteDirectory(personalDetail.MakeNrc());
                MakeViewBag();
                Utility.AlertMessage(this, "Internal Server error.", "alert-danger", "true");
                return RedirectToAction("LoginUser", "Login");
            }
        }
    }
}
