using OAuth20.Server.Enumeration;

namespace OAuth20.Server.Validations.Response
{
    public class BearerTokenUsageTypeValidationResponse : BaseValidationResponse
    {
        public string Token { get; set; }
        public BearerTokenUsageTypeEnum BearerTokenUsageType { get; set; }
    }
}
