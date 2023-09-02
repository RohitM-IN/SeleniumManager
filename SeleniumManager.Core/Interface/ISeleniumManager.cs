using OpenQA.Selenium;
using SeleniumManager.Core.Enum;

namespace SeleniumManager.Core.Interface
{
    public interface ISeleniumManager
    {
        #region Properties
        /// <summary>
        /// Gets Max Session available
        /// </summary>
        int MaxSessions { get; }

        /// <summary>
        /// Gets Free Session available
        /// </summary>
        int FreeSessions { get; }

        /// <summary>
        /// Gets total count of session available
        /// </summary>
        int TotalSessions { get; }

        /// <summary>
        /// Gets Total count of available sessions
        /// </summary>
        int AvailableSessions { get; }

        /// <summary>
        /// Gets Total count of concurrent sessions which are in use
        /// </summary>
        int ConcurrentSessions { get; }

        /// <summary>
        /// Gets When Last Hartbeat was taken.
        /// </summary>
        /// <value>the <c>string</c> represents <c>browser's name</c> and <c>long</c> represents the <c>count</c></value>
        DateTime LastSessionDetails { get; }

        /// <summary>
        /// This dictionary has list of browsers with max count of instance available
        /// </summary>
        /// <value>the <c>string</c> represents <c>browser's name</c> and <c>long</c> represents the <c>count</c></value>
        Dictionary<string, long> MaxStereotypes { get; }

        /// <summary>
        /// This dictionary has list of browsers with available instances.
        /// </summary>
        /// <value>the <c>string</c> represents <c>browser's name</c> and <c>long</c> represents the <c>count</c></value>
        Dictionary<string, long> AvailableStereotypes { get; }

        /// <summary>
        /// This dictionary has list of browsers with parallel count of instance running
        /// </summary>
        /// <value>the <c>string</c> represents <c>browser's name</c> and <c>long</c> represents the <c>count</c></value>
        Dictionary<string, long> ConcurrentStereotypes { get; }

        #endregion

        #region Methods

        /// <summary>
        /// This function will try to execute next in line of queue
        /// </summary>
        /// <exception cref="Exception"></exception>
        void TryExecuteNext();

        /// <summary>
        /// This function returns complete data of the status available from grid
        /// the endpoint is mostly example.com/status
        /// </summary>
        /// <returns>task of dynamic</returns>
        Task<dynamic?> GetHeartBeat();

        /// <summary>
        /// This function returns number of available instance in a Task
        /// </summary>
        /// <returns>Task of int</returns>
        Task<int> GetAvailableInstances();

        /// <summary>
        /// <para>
        /// This overridable function returns the webdriver from the given browser name. 
        /// By default it will try to get best available browser from the configured statistics.
        /// </para>
        /// </summary>
        /// <param name="browserName">Name of the browser</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="ArgumentException"></exception>
        IWebDriver CreateDriverInstance(string? browserName);

        /// <summary>
        /// This Function Enqueues an function 
        /// in which has the first parameter supports IWebDriver
        /// </summary>
        /// <param name="action">Action</param>
        /// <code> 
        /// manager = new SeleniumGridManager(); 
        /// manager.EnqueueAction(SomeFunction); 
        /// string SomeFunction(IWebDriver driver) { }
        /// </code>
        /// <returns>TaskCompletionSource string</returns>
        /// <exception cref="Exception"></exception>
        Task<string> EnqueueAction(Func<IWebDriver, string> action);

        /// <summary>
        /// This function checks if the browser is available or not.
        /// </summary>
        /// <param name="browserName">Name of the browser</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        string GetAvailableDriverName(string? browserName);

        /// <summary>
        /// This Function override Enqueues an function 
        /// in which has the first parameter supports IWebDriver and the other is the brousername you want
        /// </summary>
        /// <param name="action"></param>
        /// <param name="browserName">Name of the brouser for example check WebDriverType enum</param>
        /// <seealso cref="WebDriverType"/>
        /// <returns></returns>
        Task<string> EnqueueAction(Func<IWebDriver, string> action, string browserName);
        #endregion

    }
}
