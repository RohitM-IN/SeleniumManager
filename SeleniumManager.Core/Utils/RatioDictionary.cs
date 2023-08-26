namespace SeleniumManager.Core.Utils
{
    public class RatioDictionary
    {
        public static Dictionary<string, int> GetRatioDictionary(Dictionary<string, double> dict, int maxNumber)
        {
            // Calculate the total sum of values in the input dictionary
            double totalValue = dict.Values.Sum();

            // Calculate a ratio factor based on the total number and sum of values
            double ratioFactor = maxNumber / totalValue;

            Dictionary<string, int> share = new Dictionary<string, int>();
            int totalAllocated = 0;

            foreach (KeyValuePair<string, double> item in dict)
            {
                // Calculate the number of instances for the current browser type
                int instances = (int)Math.Floor(item.Value * ratioFactor);

                // Store the instances count in the share dictionary
                share[item.Key] = instances;

                // Keep track of the total allocated instances
                totalAllocated += instances;
            }

            int remainingValue = maxNumber - totalAllocated;

            // Distribute the remaining instances to the highest value
            if (remainingValue > 0)
            {
                string highestValueKey = share.OrderByDescending(x => x.Value).First().Key;
                share[highestValueKey] += remainingValue;
            }

            return share;
        }

        private static int sum(IEnumerable<int> values)
        {
            int sum = 0;
            foreach (int value in values)
            {
                sum += value;
            }
            return sum;
        }
    }
}
