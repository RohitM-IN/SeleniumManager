namespace SeleniumManager.Core.DataContract
{
    public class ConfigurationSettings
    {
        public string GridHost { get; set; }
        public Dictionary<string, int> statistics { get; set; }
        public List<string> Drivers { get; set; }
        public string Secret { get; set; }
        public int MaxConcurrency { get; set; }
        public Endpoints Endpoints { get; set; }
        public Options Options { get; set; } = new Options();
    }
}
