namespace SeleniumManager.Core.Exception
{
    public class GridAuthenticationException : System.Exception
    {
        public GridAuthenticationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public GridAuthenticationException(string message)
            : base(message)
        {
        }
    }
}
