namespace OAuth20.Server.Models
{
    public class CheckClientResult
    {
        public Client Client { get; set; }

        /// <summary>
        /// The clinet is found in my Clients Store
        /// </summary>
        public bool IsSuccess { get; set; }
        public string Error { get; set; }

        public string ErrorDescription { get; set; }
    }
}
