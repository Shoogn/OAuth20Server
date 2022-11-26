using System.Collections.Generic;

namespace OAuth20.Server.OauthResponse
{
    public class AuthorizeResponse
    {
        /// <summary>
        /// code or implicit grant or client creditional 
        /// </summary>
        public string ResponseType { get; set; } 
        public string Code { get; set; }
        /// <summary>
        /// required if it was present in the client authorization request
        /// </summary>
        public string State { get; set; }

        public string RedirectUri { get; set; }
        public IList<string> RequestedScopes { get; set; }
        public string GrantType { get; set; }
        public string Error { get; set; } = string.Empty;
        public string ErrorUri { get; set; }
        public string ErrorDescription { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
