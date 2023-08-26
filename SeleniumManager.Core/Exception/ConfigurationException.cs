namespace SeleniumManager.Core.Exception
{
    public class ConfigurationException : System.Exception
    {
        public ConfigurationException(string message) : base(message) { }
        public ConfigurationException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
