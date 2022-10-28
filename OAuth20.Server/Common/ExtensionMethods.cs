using System.ComponentModel;
using System;
using System.Linq;

namespace OAuth20.Server.Common
{
    public static class ExtensionMethods
    {
        public static string GetEnumDescription(this Enum en)
        {
            if (en == null) return null;

            var type = en.GetType();

            var memberInfo = type.GetMember(en.ToString());
            var description = (memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),
                false).FirstOrDefault() as DescriptionAttribute)?.Description;

            return description;
        }



        public static bool IsRedirectUriStartWithHttps(this string redirectUri)
        {
            if(redirectUri != null && redirectUri.StartsWith("https")) return true;

            return false;
        }
    }
}
