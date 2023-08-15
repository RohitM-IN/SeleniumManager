namespace SeleniumManager.Core.Utils
{
    public class RatioDictionary
    {
        public static Dictionary<string, int> GetRatioDictionary(Dictionary<string, int> dict, int maxNumber)
        {
            Dictionary<string, int> share = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> item in dict)
            {
                share[item.Key] = (int)(item.Value * maxNumber / sum(dict.Values));
            }

            int remainingValue = maxNumber - sum(share.Values);
            if (remainingValue > 0)
            {
                string highestValueKey = "";
                int highestValue = 0;
                foreach (KeyValuePair<string, int> item in share)
                {
                    if (item.Value > highestValue)
                    {
                        highestValue = item.Value;
                        highestValueKey = item.Key;
                    }
                }

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
