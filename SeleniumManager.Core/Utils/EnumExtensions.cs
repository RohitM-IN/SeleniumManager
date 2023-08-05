using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum value)
    {
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
}

