/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OAuth20.Server.OauthRequest
{
    public class TokenRequest
    {
        [JsonPropertyName("client_id")]
        public string client_id { get; set; }

        [JsonPropertyName("client_secret")]
        public string client_secret { get; set; }

        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("grant_type")]
        public string grant_type { get; set; }

        [JsonPropertyName("redirect_uri")]
        public string redirect_uri { get; set; }

        [JsonPropertyName("code_verifier")]
        public string code_verifier { get; set; }

        [JsonPropertyName("scope")]
        public IList<string> scope { get; set; }

        [JsonPropertyName("device_code")]
        public string device_code { get; set; }
    }
}
