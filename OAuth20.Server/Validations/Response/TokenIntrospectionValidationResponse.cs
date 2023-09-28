using OAuth20.Server.Models;

namespace OAuth20.Server.Validations.Response
{
    public class TokenIntrospectionValidationResponse : BaseValidationResponse
    {
        /// <summary>
        /// Get or set client.
        /// </summary>
        public Client Client { get; set; }
    }
}
