using VAVS_Client.Classes;
using VAVS_Client.Factories;
using VAVS_Client.Util;
using Microsoft.AspNetCore.Mvc;
using VAVS_Client.ViewModels;

namespace VAVS_Client.Controllers.Auth
{
    public class LoginController : Controller
    {
        public ServiceFactory factoryBuilder;

        public LoginController(ServiceFactory serviceFactory)
        {
            factoryBuilder = serviceFactory;
        }

        private LoginView MakeLoginView()
        {
            LoginView viewModel = new LoginView
            {
                User = new User(),
                PersonalDetail = new PersonalDetail()
            };
            return viewModel;
        }

        private void MakeViewBag()
        {
            ViewBag.IsRememberMe = SessionUtil.IsRememberMe(HttpContext);
            ViewBag.StateDivisions = factoryBuilder.CreateStateDivisionService().GetSelectListStateDivisions();
            ViewBag.Townships = factoryBuilder.CreateTownshipService().GetSelectListTownships();
        }

        public IActionResult Index()
        {
            return View("ChooseUser");
        }

        public async Task<IActionResult> LoginUser()
        {
            bool isUseVpn = await factoryBuilder.CreateDeviceInfoService().VpnTurnOn();
            if (isUseVpn)
            {
                MakeViewBag();
                Utility.AlertMessage(this, "Please turn off vpn.", "alert-danger", "true");
                return View("Login", MakeLoginView());
            }

            MakeViewBag();
            return View("Login", MakeLoginView());
        }

        [HttpPost]
        public IActionResult Login(string phoneNumber)
        {
            try
            {
                var loginUser = factoryBuilder.CreatePersonalDetailService().FindPersonalDetailByPhoneNumber(Utility.MakePhoneNumberWithCountryCode(phoneNumber));
                if (loginUser != null)
                {
                    return RedirectToAction("CheckLoginAuthentication", "Login");
                    /*string hashedEnteredPassword = HashUtil.ComputeSHA256Hash(user.Password);
                    if (loginUser.IsAuthenticateUser(hashedEnteredPassword))
                    {
                        bool isRememberMe = Request.Form["RememberMe"] == "true";
                        LoginUserInfo userInfo = new LoginUserInfo
                        {
                            UserID = loginUser.UserID,
                            LoggedInTime = DateTime.Now,
                            RememberMe = isRememberMe,
                            UserType = loginUser.UserType.UserTypeName
                        };
                        SessionUtil.SetLoginUserInfo(HttpContext, userInfo);
                        return RedirectToAction("SearchVehicle", "VehicleData");
                    }*/
                }
                MakeLoginView();
                MakeViewBag();
                Utility.AlertMessage(this, "Phone number haven't registered yet.", "alert-danger");
                return View("Login");
            }
            catch (Exception e)
            {
                Utility.AlertMessage(this, "SQl Connection Error.Please refresh browser.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
        }

        private LoginAuth InitializeLoginAuth(string phoneNumber, string hashedOtp)
        {
            return new LoginAuth()
            {
                PhoneNumber = phoneNumber,
                ResendOTPCount = 0,
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
        public async Task<IActionResult> CheckLoginAuthentication(string phoneNumber, bool resend)
        {
            try
            {
                string nrcTownshipNumber = Request.Form["NRCTownshipNumber"];
                string nrcTownshipInitial = Request.Form["NRCTownshipInitial"];
                string nrcType = Request.Form["NRCType"];
                string nrcNumber = Request.Form["NRCNumber"];
                Console.WriteLine("Nrc: " + Utility.MakeNRC(nrcTownshipNumber, nrcTownshipInitial, nrcType, nrcNumber));
                PersonalInformation personalInformation = await factoryBuilder.CreatePersonalDetailService().GetPersonalInformationByNRC(Utility.MakeNRC(nrcTownshipNumber, nrcTownshipInitial, nrcType, nrcNumber));
                if (personalInformation != null)
                {
                    LoginUserInfo userInfo = new LoginUserInfo
                    {
                        Name = personalInformation.Name,
                        NRC = Utility.MakeNRC(nrcTownshipNumber, nrcTownshipInitial, nrcType, nrcNumber),
                        LoggedInTime = DateTime.Now,
                    };
                    SessionUtil.SetLoginUserInfo(HttpContext, userInfo);
                    return RedirectToAction("SearchVehicleStandardValue", "VehicleStandardValue");
                }
                MakeViewBag();
                Utility.AlertMessage(this, "You haven't registered yet!. Please register", "alert-danger", "true");
                return RedirectToAction("LoginUser", "Login");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MakeViewBag();
                Utility.AlertMessage(this, "Internal Server error.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult RemoveOneTapLogin()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Logout()
        {
            try
            {
                LoginUserInfo loginUserInfo = SessionUtil.GetLoginUserInfo(HttpContext);
                var loginUser = new User();//factoryBuilder.CreateUserService().FindUserByUserName(loginUserInfo.PhoneNumber);

                if (loginUserInfo.RememberMe)
                {
                    SessionUtil.SetLoginUserInfo(HttpContext, loginUserInfo);
                    return RedirectToAction("Index", "Login");
                }
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            catch (Exception e)
            {
                Utility.AlertMessage(this, "Logout Fail. Internal Server Error.", "alert-danger");
                return RedirectToAction("Index", "Login");
            }
        }

    }
}
