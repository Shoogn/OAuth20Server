namespace OAuth20.Server.OAuthResponse
{
    public class UserInfoResponse 
    {
        public bool Succeeded { get; set; }
        public string Error { get; set; } = string.Empty;

        public string ErrorDescription { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);


        /// <summary>
        /// Returned result as json.
        /// </summary>
        public string Claims { get; set; }
    }
}
