using System.Text.RegularExpressions;
using CultureInfo = System.Globalization.CultureInfo;

public static class StringExt
{
	public static string InsertSpacesToCamelCase(this string s) => Regex.Replace(s, @"(?<!^)(?=[A-Z])", " ");

	public static string CamelCaseToTitleCase(this string s) => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(InsertSpacesToCamelCase(s));
}