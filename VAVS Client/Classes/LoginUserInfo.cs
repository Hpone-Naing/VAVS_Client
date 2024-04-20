namespace VAVS_Client.Classes
{
    public class LoginUserInfo
    {
        public string Name { get; set; }
        public string NRC { get; set; }
        public DateTime LoggedInTime { get; set; }
        public String UserType { get; set; }
        public bool RememberMe { get; set; }
    }
}
