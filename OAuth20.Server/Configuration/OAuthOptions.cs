namespace OAuth20.Server.Configuration
{
   
    public class OAuthOptions
    {
        /// <summary>
        /// This indicate which provider our identity provider will use
        /// InMemoey Or BackStore
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// If is not availabe so, no user can login or register
        /// This will shout down the application domain
        /// </summary>
        public bool IsAvaliable { get; set; }

    }
}
