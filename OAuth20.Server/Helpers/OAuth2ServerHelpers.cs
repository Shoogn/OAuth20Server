using OAuth20.Server.Common;
using System.Collections.Generic;

namespace OAuth20.Server.Helpers
{
    public class OAuth2ServerHelpers
    {
        public static IList<string> CodeChallenegMethodsSupport = new List<string>()
        {
            Constants.Plain,
            Constants.SHA256
        };
    }
}
