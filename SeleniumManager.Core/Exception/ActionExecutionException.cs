namespace SeleniumManager.Core.Exception
{
    public class ActionExecutionException : System.Exception
    {
        public ActionExecutionException(string message) : base(message) { }
        public ActionExecutionException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
