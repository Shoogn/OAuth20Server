using System.Collections.Generic;

namespace OAuth20.Server.Models
{
    public class Client
    {
        public Client()
        {

        }

        public string ClientName { get; set; }
        public string ClientId { get; set; }

        /// <summary>
        /// Client Password
        /// </summary>
        public string ClientSecret { get; set; }

        public IList<string> GrantType { get; set; }

        /// <summary>
        /// by default false
        /// </summary>
        public bool IsActive { get; set; } = false;
        public IList<string> AllowedScopes { get; set; }

        public string ClientUri { get; set; }
        public string RedirectUri { get; set; }

        public bool UsePkce { get; set; }
    }
}
