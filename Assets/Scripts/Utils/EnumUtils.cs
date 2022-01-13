using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public class EnumUtils
{
    public static string GetDescriptionFromEnumValue(Enum value)
    {
        DescriptionAttribute attribute = value.GetType()
            .GetField(value.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static T GetEnumValueFromDescription<T>(string description)
    {
        var type = typeof(T);
        if (!type.IsEnum)
            throw new ArgumentException();
        FieldInfo[] fields = type.GetFields();
        var field = fields
                        .SelectMany(f => f.GetCustomAttributes(
                            typeof(DescriptionAttribute), false), (
                                f, a) => new { Field = f, Att = a })
                        .Where(a => ((DescriptionAttribute)a.Att)
                            .Description == description).SingleOrDefault();
        return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
    }

    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static string[] GetAllEnumDescriptions<T>()
    {
        var enumValues = GetValues<T>();
        int enumValuesCount = enumValues.Count();

        string[] descriptions = new string[enumValues.Count()];
        for (int i = 0; i < enumValuesCount; i++)
        {
            descriptions[i] = GetDescriptionFromEnumValue(
                enumValues.ElementAt(i) as Enum
            );
        }
        return descriptions;
    }
}
