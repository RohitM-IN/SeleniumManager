namespace SeleniumManager.Core.Exception
{
    public class UnsupportedBrowserException : System.Exception
    {
        public UnsupportedBrowserException(string browserName) : base($"Browser '{browserName}' is not supported yet!") { }
    }
}
