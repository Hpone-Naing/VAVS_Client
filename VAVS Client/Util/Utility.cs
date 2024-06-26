﻿using VAVS_Client.Classes;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Config;
using FireSharp.Interfaces;
using System.Text.RegularExpressions;

namespace VAVS_Client.Util
{
    public class Utility
    {
        public static int DEFAULT_PAGINATION_NUMBER = 5;
        /*
         * Resend code and register
         */
        public static int MAXIMUM_REGISTRATION_TIME = 5;
        public static int MAXIMUM_RESEND_CODE_TIME = 3;
        public static int OTP_EXPIRE_MINUTE = 0;
        public static int OTP_EXPIRE_SECOND = 120;
        public static int NEXT_RESENDCODE_TIME_IN_MINUTE = 3; 
        public static int NEXT_RESENDCODE_TIME_IN_SECOND = 0;
        public static int NEXT_REGISTER_TIME_IN_MINUTE = 5;
        public static int NEXT_REGISTER_TIME_IN_SECOND = 0;
        public static string REGISTRATION_AUTH_FIREBASE_PATH = "DeviceInfos/";
        public static string LOGIN_AUTH_FIREBASE_PATH = "LoginAuths/";
        public static string LoginUserInfo_FIREBASE_PATH = "LoginUserInfos/";
        public static string ResetPhonenumberAuth_FIREBASE_PATH = "resetPhoneNumberAuth/";
        public static string TOKEN = "Heo2fgUj2IajZp4Qvbr0wzV9rnygp5GdvnrmOsdT";
        public static string RESET_PHONE_TOKEN = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";
        /*
         * API KEYS
         */
        public static string SMSPOH_API_KEY = "RkyHNSAGkqib9loT_GeOmP9lfafWXPELNvMoc1GTYIX-z2qGJFCfQkfng4Hvlc0o";
        public static string SEARCH_VEHICLE_STANDARD_VALUE_API_KEY = "V3H!cl3$t@ND@rd";
        public static string GenerateOtp() => new Random().Next(1000000).ToString("D6");
        public static string GetIPAddress()
        {
            return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList
                                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();

        }

        public static string MakeNRC(string townshipNumber, string townshipInitial, string type, string number)
        {
            return String.Concat(townshipNumber, townshipInitial, type, number);
        }

        public static string ConcatNRCSemiComa(string nrc)
        {            
                string pattern = @"/";
                string replacedString = Regex.Replace(nrc, pattern, "/;");
                pattern = @"\(";
                replacedString = Regex.Replace(replacedString, pattern, ";(");
                pattern = @"\)";
                replacedString = Regex.Replace(replacedString, pattern, ");");
                return replacedString;
        }

        public static string[] SplitNrc(string nrc)
        {

            return new string[] { nrc.Split('/')[0]+"/", nrc.Split('(')[0].Split('/')[1], "("+nrc.Split(')')[0].Split('/')[1].Split('(')[1]+")", nrc.Split(')')[1] };
            
        }



        public static IFirebaseConfig GetFirebaseConfig()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "MWhnm4SP7v7w2JWXMKjw9nN8ZSVhiMeSpHku4v5T",
                BasePath = "https://dis-vavs-default-rtdb.firebaseio.com/"
            };
            return config;
        }

        public static void AlertMessage(Controller controller, string message, string color, string option = null)
        {
            controller.TempData["Message"] = message;
            controller.TempData["CssColor"] = color;
            controller.TempData["Option"] = option;
        }

        public static string MakeMessage(string msg, string param1, string option = "")
        {
            return string.Concat(msg, ": ", param1, " " + option);
        }

        public static string MakeMessage(string msg1, string param1, string msg2, string param2, string option = "")
        {
            return string.Concat(msg1, ": ", param1, msg2, ": " + param2 + " " + option);
        }

        public static string MakePhoneNumberWithCountryCode(string phoneNumber)
        {
            int index = phoneNumber.IndexOf("09");
            if (index != -1)
            {
                return phoneNumber.Substring(0, index) + "+959" + phoneNumber.Substring(index + 2);
            }
            else
            {
                return phoneNumber;
            }
        }

        public static long MakeDigit(string digit, string option = null)
        {
            string makedDigit;

            if(option == null)
                makedDigit = digit;
            else
                makedDigit = digit + "00000";

            long result;
            if (long.TryParse(makedDigit, out result))
            {
                return result;
            }
            else
            {
                return 0;
                //throw new ArgumentException("Digit is not valid.");
            }
        }
    }
}
