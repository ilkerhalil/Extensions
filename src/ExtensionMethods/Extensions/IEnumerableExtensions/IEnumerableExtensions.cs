
using System.ComponentModel;

namespace Extensions.IEnumerableExtensions
{
    using DataTableExtensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using ObjectExtensions;
    using Properties;

    using OfficeOpenXml;
    using OfficeOpenXml.Table;

    public static class IEnumerableExtensions
    {

        public static Stream ToExcel(this IEnumerable<dynamic> collection, string sheetName = "Sheet1")
        {
            return collection.ToDataTable().ToExcel(sheetName);
        }

        public static void ToExcel<T>(this IEnumerable<T> collection, string savePath)
        {

            using (var fs = new FileStream(savePath, FileMode.OpenOrCreate))
            {
                var package = new ExcelPackage(fs);
                var sheet = package.Workbook.Worksheets.Add("Sheet 1");
                sheet.Cells.LoadFromCollection(collection, true);
                package.Save();
            }

        }

        public static Stream ToExcel<T>(this IEnumerable<T> collection, string sheetName = "Sheet1", TableStyles tableStyle = TableStyles.Dark1)
        {
            if (!Enum.IsDefined(typeof(TableStyles), tableStyle))
                throw new InvalidEnumArgumentException(nameof(tableStyle), (int)tableStyle, typeof(TableStyles));
            if (!Enum.IsDefined(typeof(TableStyles), tableStyle))
                throw new InvalidEnumArgumentException(nameof(tableStyle), (int)tableStyle, typeof(TableStyles));
            var fs = new MemoryStream();
            var package = new ExcelPackage(fs);
            var sheet = package.Workbook.Worksheets.Add(sheetName);
            sheet.Cells.LoadFromCollection(collection, true, tableStyle);
            package.Save();
            return fs;

        }
        public static string ToString<T>(this IEnumerable<T> collection,
      Func<T, string> stringElement, string separator) => ToString(collection, t => t.ToString(), separator);

        public static string ToHtmlTable<T>(this IEnumerable<T> collection, params string[] style)
        {
            return collection.ToHtmlTable(null, null, false, style);
        }

        private static string ToHtmlTable<T>(this IEnumerable<T> collection, string hrefIdColumnName, string hrefParam, bool visibleFooter, params string[] style)
        {
            var sr = new StringBuilder();
            sr.AppendLine("<style type='text/css'>");
            style.ToList().ForEach(str =>
            {
                sr.AppendLine(str);
            });
            sr.AppendLine("</style>");
            sr.AppendLine("<table cellspacing=\"3\" cellpadding=\"3\" border=\"1\" align=\"left\" style=\"{0}\">");
            sr.AppendLine("<thead>");
            sr.AppendLine("<tr>");
            var elementType = typeof(T);
            foreach (var item in from item in elementType.GetProperties() let colType = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType select item)
            {
                sr.AppendLine($"<th>{item.Name}</th>");
            }
            sr.AppendLine("</tr>");
            sr.AppendLine("</thead>");
            sr.AppendLine("<tbody>");
            foreach (var item in collection)
            {
                sr.AppendLine($"<tr class=\"{("R1")}\">");
                var properties = item.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(item, null) ?? string.Empty;
                    if (property.Name == hrefIdColumnName)
                    {
                        var a = string.Format("<a href='{1}{0}'>{0}</a>", value, hrefParam);
                        sr.AppendLine($"<td>{a}</td>");
                        continue;
                    }
                    sr.AppendLine($"<td>{value}</td>");
                }
                sr.AppendLine("</tr>");
            }
            if (visibleFooter)
            {
                var footer = $"<tfoot><tr>Total:<b> {collection.Count()} Items </b></tr></tfoot>";
                sr.AppendLine(footer);
            }
            sr.AppendLine("</tbody>");
            sr.AppendLine("</table>");
            return sr.ToString();
        }

        public static string ToVerticalHtmlTable<T>(this IEnumerable<T> collection, string hrefIdColumnName = "", string hrefParam = "", string[] headerColumn = null, bool isFooter = false, params string[] style)
        {

            var sr = new StringBuilder();
            if (style.Any())
            {
                sr.AppendLine("<style type='text/css'>");
                style.ToList().ForEach(str => sr.AppendLine(str));
                sr.AppendLine("</style>");
            }
            else if (!style.Any())
            {
                sr.AppendLine("<style type='text/css'>");
                sr.AppendLine(string.Empty);
                sr.AppendLine("</style>");
            }
            sr.AppendLine("<table>");
            if (headerColumn != null)
            {
                if (headerColumn.Any())
                {
                    sr.AppendLine("<thead>");
                    sr.AppendLine("<tr>");
                    headerColumn.ToList().ForEach(f => sr.AppendLine($"<th>{f}</th>"));
                    sr.AppendLine("</tr>");
                    sr.AppendLine("</thead>");
                }

            }
            sr.AppendLine("<tbody>");
            Type elementType = typeof(T);
            var enumerable = collection as T[] ?? collection.ToArray();
            var array = enumerable.ToArray();
            foreach (var item in elementType.GetProperties())
            {

                sr.AppendLine("<tr id='header'>");
                sr.AppendLine($"<td>{item.Name}</td>");
                array.ToList().ForEach(ar => ar.GetType().GetProperties().Where(w => w.Name == item.Name).ToList()
                    .ForEach(f =>
                     {
                         var value = f.GetValue(ar, null) ?? string.Empty;
                         if (f.Name == hrefIdColumnName)
                         {
                             var a = string.Format("<a href='{1}{0}'>{0}</a>", value, hrefParam);
                             sr.AppendLine($"<td>{a}</td>");
                             return;
                         }
                         sr.AppendLine($"<td>{value}</td>");
                     }));
                sr.AppendLine("</tr>");
            }



            if (isFooter)
            {
                var footer = $"<tfoot><tr><td>Total:<b> {enumerable.Count()} Items </b></td></tr></tfoot>";
                sr.AppendLine(footer);
            }
            sr.AppendLine("</tbody>");
            sr.AppendLine("</table>");
            return sr.ToString();
        }


        public static string ToHtmlTable<T>(this IEnumerable<T> list, string tableSyle, string headerStyle, string rowStyle, string alternateRowStyle)
        {
            var result = new StringBuilder();
            var root = string.IsNullOrEmpty(tableSyle) ? string.Format("<table id=\"" + typeof(T).Name + "Table\">") : string.Format("<table id=\"" + typeof(T).Name + "Table\" class=\"{0}\">", tableSyle);
            result.AppendLine(root);
            var propertyArray = typeof(T).GetProperties();
            foreach (var prop in propertyArray)
            {
                var header = string.IsNullOrEmpty(headerStyle) ? $"<th>{prop.Name}</th>"
                    :
                    $"<th class=\"{headerStyle}\">{prop.Name}</th>";
                result.AppendLine(header);
            }
            for (var i = 0; i < list.Count(); i++)
            {
                var tr = !string.IsNullOrEmpty(rowStyle) && !string.IsNullOrEmpty(alternateRowStyle) ?
                    $"<tr class=\"{(i % 2 == 0 ? rowStyle : alternateRowStyle)}\">"
                    : "<tr>";
                foreach (var prop in propertyArray)
                {
                    var value = prop.GetValue(list.ElementAt(i), null); result.AppendFormat("<td>{0}</td>", value ?? string.Empty);
                }
                result.AppendLine("</tr>");
            }
            result.Append("</table>");
            return result.ToString();
        }

        public static string Compare2ToVerticalHtmlTable<T>(this IEnumerable<T> collection, string hrefIdColumnName = "", string hrefParam = "", string[] headerColumn = null, bool isFooter = false, string dateFormat = "d MMMM yyyy", params string[] style)
        {
            var sr = new StringBuilder();
            //var collectionArray = collection.ToArray();
            sr.AppendLine();
            sr.AppendLine("<style type='text/css'>");
            string css;

            css = style.Length > 0 ? css = string.Join("", style) : Resources.css;
            sr.AppendLine(css);
            sr.AppendLine("</style>");
            sr.AppendLine("<table>");
            if (headerColumn != null)
            {
                if (headerColumn.Any())
                {
                    sr.AppendLine("<thead>");
                    sr.AppendLine("<tr>");
                    headerColumn.ToList().ForEach(f => sr.AppendLine($"<th>{f}</th>"));
                    sr.AppendLine("</tr>");
                    sr.AppendLine("</thead>");
                }

            }
            sr.AppendLine("<tbody>");
            var elementType = typeof(T);
            var enumerable = collection as T[] ?? collection.ToArray();
            var array = enumerable;
            var iRowOdd = 0;
            foreach (var item in elementType.GetProperties())
            {
                iRowOdd = iRowOdd + 1;
                sr.AppendLine($"<tr id=\"R{((iRowOdd % 2 == 0) ? "0" : "1")} \">");
                sr.AppendLine($"<td><span>{item.Name}</span></td>");
                array.Take(2).ToList().ForEach(ar => ar.GetType().GetProperties().Where(w => w.Name == item.Name).ToList()
                    .ForEach(f =>
                    {
                        var value = f.GetValue(ar, null) ?? string.Empty;
                        if (f.Name == hrefIdColumnName)
                        {

                            var a = string.Format("<a href='{1}{0}'>{0}</a>", value, hrefParam);
                            sr.AppendLine($"<td><span>{a}</span></td>");
                            return;
                        }
                        DateTime date;
                        if (DateTime.TryParse(value.ToString(), out date))
                        {
                            sr.AppendLine($"<td><span>{date.ToString(dateFormat)}</span></td>");
                            return;
                        }
                        sr.AppendLine($"<td><span>{value}</span></td>");
                    }));

                var v1 = array[0].GetType().GetProperty(item.Name).GetValue(array[0], null) ?? string.Empty;
                var v2 = array[1].GetType().GetProperty(item.Name).GetValue(array[1], null) ?? string.Empty;
                sr.AppendLine((v1.Equals(v2)) ? "<td><span style=\"color:green;font-size:20px\">&#10003</span></td>" : "<td><span style=\"color:red;font-size:20px\">!!!</span></td>");

                sr.AppendLine("</tr>");
            }
            if (isFooter)
            {
                var footer = $"<tfoot><tr><td>Total:<b> {enumerable.Count()} Items </b></td></tr></tfoot>";
                sr.AppendLine(footer);
            }
            sr.AppendLine("</tbody>");
            sr.AppendLine("</table>");


            return sr.ToString();
        }


        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            var i = 0;
            var splits = from item in list
                         group item by i++ % parts into part
                         select part.AsEnumerable();
            return splits;
        }

        public static string ToStringExtension<T>(this IEnumerable<T> collection, string seperator = "-")
        {
            var builder = new StringBuilder();
            foreach (var itemToString in collection.Select(item => item.ToStringExtension()))
            {
                builder.Append(itemToString);
                var sprtr = string.Empty;
                for (var i = 0; i < itemToString.Length / 10; i++)
                    sprtr += seperator;
                builder.AppendLine(sprtr);
            }
            return builder.ToString();
        }
    }
}
