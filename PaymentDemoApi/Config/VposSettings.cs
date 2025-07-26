namespace VposApi.Config
{
    public class VposSettings
    {
        public string NonSecureEndpoint { get; set; }
        public string SecureEndpoint { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }   
        public string IPAddress { get; set; }
        
        public string OkUrl { get; set; }  
        public string FailUrl { get; set; }
        public string OkPage { get; set; }
        public string FailPage { get; set; }

        public string TDPayOkUrl { get; set; }
        public string TDPayFailUrl { get; set; }
        public string TDPayOkPage { get; set; }
        public string TDPayFailPage { get; set; }
    }
}