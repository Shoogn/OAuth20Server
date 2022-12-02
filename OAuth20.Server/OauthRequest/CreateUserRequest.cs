namespace OAuth20.Server.OauthRequest
{
    public class CreateUserRequest
    {
        public string UserName { get; set;}
        public string Password { get; set;}
        public string Email { get; set;}
        public string PhoneNumber { get; set; }
    }
}
