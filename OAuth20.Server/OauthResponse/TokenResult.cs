using System;

namespace OAuth20.Server.OauthResponse
{
    public class TokenResult
    {
        /// <summary>
        /// Identity Token 
        /// Access Token
        /// </summary>
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Error { get; set; } = string.Empty;
        public string ErrorDescription { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
