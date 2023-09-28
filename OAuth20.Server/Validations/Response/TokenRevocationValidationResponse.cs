using OAuth20.Server.Models;

namespace OAuth20.Server.Validations.Response
{
    public class TokenRevocationValidationResponse : BaseValidationResponse
    {
        public Client Client { get; set; }
    }
}
