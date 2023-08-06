using SeleniumManager.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public static class WebDriverTypeExtensions
{
    private static readonly Dictionary<WebDriverType, string> customDescriptions = new Dictionary<WebDriverType, string>();

    public static string GetDescription(this WebDriverType value)
    {
        if (customDescriptions.TryGetValue(value, out string customDescription))
        {
            return customDescription;
        }

        Type enumType = value.GetType();
        string name = Enum.GetName(enumType, value);

        if (name != null)
        {
            FieldInfo field = enumType.GetField(name);
            if (field != null)
            {
                DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>();
                if (attr != null)
                {
                    return attr.Description;
                }
            }
        }

        return value.ToString();
    }

    public static void SetCustomDescription(WebDriverType type, string description)
    {
        customDescriptions[type] = description;
    }
}

