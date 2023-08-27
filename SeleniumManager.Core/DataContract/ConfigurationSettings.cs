namespace SeleniumManager.Core.DataContract
{
    public class ConfigurationSettings
    {
        private string _GridHost;

        public string GridHost
        {
            get
            {
                UriBuilder uriBuilder = new UriBuilder(_GridHost);
                uriBuilder.Scheme = uriBuilder.Scheme; // Set the desired scheme (http or https)

                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    uriBuilder.UserName = UserName;
                    uriBuilder.Password = Password;
                }


                return uriBuilder.ToString().TrimEnd('/');
            }
            set
            {
                _GridHost = value;
            }
        }
        public Dictionary<string, double> statistics { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Endpoints Endpoints { get; set; }
        public Options Options { get; set; } = new Options();
    }
}
