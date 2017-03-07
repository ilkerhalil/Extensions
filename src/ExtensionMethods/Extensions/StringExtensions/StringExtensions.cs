using System.Runtime.Serialization.Json;
using Extensions.Models;

namespace Extensions.StringExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Properties;
    using PoorMansTSqlFormatterLib;
    using System.Globalization;
    using System.Threading;
    using System.Xml;
    using System.Xml.Xsl;

    public static class StringExtensions
    {

        public static bool IsNumeric(this string value)
        {
            decimal output;
            return decimal.TryParse(value, out output);
        }
        /// <summary>
        /// Parametre olarak verilen ve türkçe karakterler içeren string ifadeyi İngilizce 
        ///karşılığı olan karaktere cevirir.
        /// </summary>
        public static string ConvertTurkishToEnglish(this string str)
        {
            var turkishChar = new[] { 'ç', 'Ç', 'Ğ', 'ı', 'İ', 'ö', 'Ö', 'ş', 'Ş', 'ü', 'Ü' };
            var englishChar = new[] { 'c', 'C', 'G', 'i', 'I', 'o', 'O', 's', 'S', 'u', 'U' };

            for (var i = 0; i < turkishChar.Length; i++)
                str = str.Replace(turkishChar[i], englishChar[i]);
            return str;
        }
        /// <summary>
        ///String ifadenin verilen RegEx ifadesine uygun olup olmadini kontrol eder 
        /// </summary>


        public static bool RegExControl(this string str, string regExPattern)
        {
            return Regex.Match(str, regExPattern).Success;
        }
        

        /// <summary>
        /// string ifadenin null veya empty olup olmadigini kontrol eder
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        /// <summary>
        /// n kadar string formata gore formatlayip dondurur
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
        /// <summary>
        /// string ifadeyi datetime ifadesine cevirir
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static DateTime? ToNullableDate(this string dateString)
        {
            if (string.IsNullOrEmpty((dateString ?? "").Trim()))
                return null;

            DateTime resultDate;
            if (DateTime.TryParse(dateString, out resultDate))
                return resultDate;

            return null;
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            var array = source.Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }

        public static string AppendJokerSqlText(this string name)
        {
            var strBldr = new StringBuilder();
            foreach (var item in name)
            {
                switch (item)
                {
                    case 'i':
                        strBldr.Append("[ıiIİ]");
                        break;
                    case 'ı':
                        strBldr.Append("[ıiIİ]");
                        break;
                    case 'İ':
                        strBldr.Append("[ıiIİ]");
                        break;
                    case 'I':
                        strBldr.Append("[ıiIİ]");
                        break;
                    case 'o':
                        strBldr.Append("[oöOÖ]");
                        break;
                    case 'O':
                        strBldr.Append("[oöOÖ]");
                        break;
                    case 'ö':
                        strBldr.Append("[oöOÖ]");
                        break;
                    case 'Ö':
                        strBldr.Append("[oöOÖ]");
                        break;
                    case 'u':
                        strBldr.Append("[uüUÜ]");
                        break;
                    case 'U':
                        strBldr.Append("[uüUÜ]");
                        break;
                    case 'ü':
                        strBldr.Append("[uüUÜ]");
                        break;
                    case 'Ü':
                        strBldr.Append("[uüUÜ]");
                        break;
                    case 's':
                        strBldr.Append("[sSşŞ]");
                        break;
                    case 'ş':
                        strBldr.Append("[sSşŞ]");
                        break;
                    case 'S':
                        strBldr.Append("[sSşŞ]");
                        break;
                    case 'Ş':
                        strBldr.Append("[sSşŞ]");
                        break;
                    case 'c':
                        strBldr.Append("[cCçÇ]");
                        break;
                    case 'ç':
                        strBldr.Append("[cCçÇ]");
                        break;
                    case 'C':
                        strBldr.Append("[cCçÇ]");
                        break;
                    case 'Ç':
                        strBldr.Append("[cCçÇ]");
                        break;
                    case 'g':
                        strBldr.Append("[gGğĞ]");
                        break;
                    case 'G':
                        strBldr.Append("[gGğĞ]");
                        break;
                    case 'ğ':
                        strBldr.Append("[gGğĞ]");
                        break;
                    case 'Ğ':
                        strBldr.Append("[gGğĞ]");
                        break;
                    case ' ':
                        strBldr.Append("% ");
                        break;
                    default:
                        strBldr.Append(item);
                        break;
                }
            }
            return strBldr.ToString();
        }

        public static string GetRandomText(this string targetContent, int size, bool? lowerCase = true)
        {

            var stringBuilder = new StringBuilder();
            var random = new Random((int)DateTime.Now.Ticks);
            for (var i = 0; i < size; i++)
            {
                var pChar = targetContent.ToCharArray()[random.Next(0, targetContent.ToCharArray().Length - 1)];
                stringBuilder.Append(pChar);
            }
            if (lowerCase.HasValue)
                return lowerCase.Value ? stringBuilder.ToString().ToLower() : stringBuilder.ToString();
            return stringBuilder.ToString();
        }

        public static GetCountryAndCityResponse GetCountryAndCityRequest(this string ipAdress)
        {
            try
            {
                var requestUrl = string.Format(Resources.StringExtensions_GeoLocationUrl, ipAdress);
                var webRequest = WebRequest.Create(requestUrl);
                using (var response = (HttpWebResponse)webRequest.GetResponse())
                {
                    if (response.GetResponseStream() != null)
                    {
                        var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        return ParseGetCountryAndCityResult(result);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private static GetCountryAndCityResponse ParseGetCountryAndCityResult(string result)
        {
            var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(GetCountryAndCityResponse));
            var memoryStream =  new MemoryStream(Encoding.Default.GetBytes(result));
            return dataContractJsonSerializer.ReadObject(memoryStream) as GetCountryAndCityResponse;
        }

        public static string ConvertToText(this string html)
        {
            var xmlDoc = new XmlDocument();
            var xsl = new XmlDocument();
            xmlDoc.LoadXml(html);
            xsl.CreateEntityReference("nbsp");
            xsl.Load("Markdown.xslt");
#pragma warning disable 618
            var xslt = new XslTransform();
#pragma warning restore 618
            xslt.Load(xsl, null, null);
            var writer = new StringWriter();
            xslt.Transform(xmlDoc, null, writer, null);
            var text = writer.ToString();
            writer.Close();
            return text;
        }

        public static string CombineToPath(this string path, string root)
        {
            return Path.IsPathRooted(path) ? path : Path.Combine(root, path);
        }

        public static void IfNotNull(this string target, Action<string> continuation)
        {
            if (target != null)
            {
                continuation(target);
            }
        }

        public static string ToFullPath(this string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Retrieve the parent directory of a directory or file
        /// Shortcut to Path.GetDirectoryName(path)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ParentDirectory(this string path)
        {
            return Path.GetDirectoryName(path.TrimEnd(Path.DirectorySeparatorChar));
        }

  




        public static bool IsEmpty(this string stringValue)
        {
            return string.IsNullOrEmpty(stringValue);
        }

        public static bool IsNotEmpty(this string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }

        public static void IsNotEmpty(this string stringValue, Action<string> action)
        {
            if (stringValue.IsNotEmpty())
                action(stringValue);
        }

        public static bool ToBool(this string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue) && bool.Parse(stringValue);
        }

        

        /// <summary>
        /// Performs a case-insensitive comparison of strings
        /// </summary>
        public static bool EqualsIgnoreCase(this string thisString, string otherString)
        {
            return thisString.Equals(otherString, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Converts the string to Title Case
        /// </summary>
        public static string Capitalize(this string stringValue)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stringValue);
        }

        /// <summary>
        /// Formats a multi-line string for display on the web
        /// </summary>
        /// <param name="plainText"></param>
        public static string ConvertCRLFToBreaks(this string plainText)
        {
            return new Regex("(\r\n|\n)").Replace(plainText, "<br/>");
        }

        

        public static string ToGmtFormattedDate(this DateTime date)
        {
            return date.ToString("yyyy'-'MM'-'dd hh':'mm':'ss tt 'GMT'");
        }

        public static string[] ToDelimitedArray(this string content)
        {
            return content.ToDelimitedArray(',');
        }

        private static string[] ToDelimitedArray(this string content, char delimiter)
        {
            var array = content.Split(delimiter);
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }

            return array;
        }

        public static bool IsValidNumber(this string number)
        {
            return IsValidNumber(number, Thread.CurrentThread.CurrentCulture);
        }

        private static bool IsValidNumber(this string number, CultureInfo culture)
        {
            var validNumberPattern =
            @"^-?(?:\d+|\d{1,3}(?:"
            + culture.NumberFormat.NumberGroupSeparator +
            @"\d{3})+)?(?:\"
            + culture.NumberFormat.NumberDecimalSeparator +
            @"\d+)?$";

            return new Regex(validNumberPattern, RegexOptions.ECMAScript).IsMatch(number);
        }

        public static IList<string> GetPathParts(this string path)
        {
            return path.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string DirectoryPath(this string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Reads text and returns an enumerable of strings for each line
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadLines(this string text)
        {
            var reader = new StringReader(text);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// Reads text and calls back for each line of text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static void ReadLines(this string text, Action<string> callback)
        {
            var reader = new StringReader(text);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                callback(line);
            }
        }

        /// <summary>
        /// Just uses MD5 to create a repeatable hash
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>


        /// <summary>
        /// Splits a camel cased string into seperate words delimitted by a space
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        /// <summary>
        /// Splits a pascal cased string into seperate words delimitted by a space
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SplitPascalCase(this string str)
        {
            return SplitCamelCase(str);
        }

        public static TEnum ToEnum<TEnum>(this string text) where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum) throw new ArgumentException("{0} is not an Enum".FormatString(enumType.Name));
            return (TEnum)Enum.Parse(enumType, text, true);
        }

        /// <summary>
        /// Wraps a string with parantheses. Originally used to file escape file names when making command line calls
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string FileEscape(this string file)
        {
            return "\"{0}\"".FormatString(file);
        }

        public static string ToMoney(this string money)
        {
            decimal p;
            return FormatString("{0:C}", Decimal.TryParse(money, out p) ? p : 0);
        }


        public static string FormattingSqlQuery(this string query)
        {

            var manager = new SqlFormattingManager();
            var formattedQuery = manager.Format(query);
            return formattedQuery;
        }

        public static bool IsXml(this string text)
        {
            try
            {
                var p = XDocument.Parse(text);
            }
            catch (XmlException)
            {
                return false;
            }
            return true;

        }


    }
}
