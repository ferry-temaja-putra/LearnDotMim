namespace AuthSyncSharedData
{
    public class LoginInfo
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool InputValid => 
            !string.IsNullOrEmpty(UserName) && 
            !string.IsNullOrEmpty(Password);
    }
}
