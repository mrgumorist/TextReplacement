using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TextParser
{
    public static class MyExtensions
    {
        public static string ReplaceBaseTexts(this string str) =>
            str.Replace("{NewLine}", Environment.NewLine);

        public static string RemoveSpecialSymbols(this string str) =>
            str
            .Replace("{", "")
            .Replace("}", "")
            .Replace(".", "");

        public static string ToYesNoString(this bool value)
        {
            return value ? "Yes" : "No";
        }

        public static string RemoveType<T>(this string str) where T : class =>
            str.Replace($"{typeof(T).Name}", "");
    }

    internal class Program
    {
        public static string ReplaceText<T>(string text, T obj, string numberFormat = "0.00", string arraySeparator = ",") where T : class
        {
            //ASK for typeof(T) or typeof(T).Name
            Regex regex = new Regex($@"\{{{typeof(T).Name}..*?\}}");
            var matches = regex.Matches(text).GroupBy(m=>m.Value).Select(g => g.First());

            var props = typeof(T).GetProperties();

            text = text.ReplaceBaseTexts();

            foreach (var match in matches)
            {
                var property = props.FirstOrDefault(p => p.Name == match.ToString().RemoveSpecialSymbols().RemoveType<T>());

                if (property != null)
                {
                    var propertyType = property.PropertyType;

                    switch (true)
                    {
                        case true when typeof(bool).IsAssignableFrom(propertyType):
                            bool boolPropertyValue = (bool)obj.GetType().GetProperty(property.Name).GetValue(obj, null);
                            text = text.Replace(match.ToString(), boolPropertyValue.ToYesNoString());
                            break;
                        case true when typeof(double).IsAssignableFrom(propertyType):
                            double doublePropertyValue = (double)obj.GetType().GetProperty(property.Name).GetValue(obj, null);
                            text = text.Replace(match.ToString(), doublePropertyValue.ToString(numberFormat));
                            break;
                        case true when typeof(decimal).IsAssignableFrom(propertyType):
                            decimal decimalPropertyValue = (decimal)obj.GetType().GetProperty(property.Name).GetValue(obj, null);
                            text = text.Replace(match.ToString(), decimalPropertyValue.ToString(numberFormat));
                            break;
                        case true when typeof(List<string>).IsAssignableFrom(propertyType):
                            List<string> listPropertyValue = (List<string>)obj.GetType().GetProperty(property.Name).GetValue(obj, null);
                            text = text.Replace(match.ToString(), string.Join(arraySeparator, listPropertyValue.ToArray()));
                            break;
                        default:
                            string strintPropertyValue = obj.GetType().GetProperty(property.Name).GetValue(obj, null).ToString();
                            text = text.Replace(match.ToString(), strintPropertyValue);
                            break;
                    }
                }
            }

            return text;
        }

        static void Main(string[] args)
        {

            string Template = "Today we have a new song '{Song.Name}'. Author {Song.Author} of {Song.Name} is a very famous person!{NewLine}" +
                "Duration of song: {Song.Duration}{NewLine}" +
                "Popular: {Song.IsPopular}{NewLine}" +
                "Rating: {Song.Rating}(From 0 to 5){NewLine}" +
                "KeyWords: {Song.KeyWords}";

            Song song = new Song() { Author = "Author1", Name = "MySong", Duration = 183, IsPopular = true, Rating = 3.2m, KeyWords = new System.Collections.Generic.List<string>() { "A", "B", "C" } };

            var result = ReplaceText(Template, song, arraySeparator: ";");

            Console.WriteLine(result);
        }
    }
}
