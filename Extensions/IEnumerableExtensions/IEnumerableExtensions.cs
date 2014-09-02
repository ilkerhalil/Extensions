
namespace Extensions.IEnumerableExtensions
{
    using Extensions.DataTableExtensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using ClosedXML.Excel;
    using Extensions.ObjectExtensions;
    using Extensions.Properties;

    using OfficeOpenXml;
    using OfficeOpenXml.Table;

    public static class IEnumerableExtensions
    {
        //public static Stream ToExcel(this IEnumerable<dynamic> collection, string sheetName = "Sheet1")
        //{
        //    var ms = new MemoryStream();
           
        //    return ms;

        //}


        public static Stream ToExcel(this IEnumerable<dynamic> collection, string sheetName = "Sheet1")
        {
            return collection.ToDataTable().ToExcel(sheetName);
        }

        public static void ToExcel<T>(this IEnumerable<T> collection, string savePath)
        {
            var dataTble = collection.ToDataTable();
            var wb = new XLWorkbook();
            wb.Worksheets.Add(dataTble);
            wb.SaveAs(savePath);
        }

        public static Stream ToExcel<T>(this IEnumerable<T> collection)
        {

            //var fs = File.Create("tmpXlsFile");
            //var dataTable = collection.ToDataTable();
            //using (var wb = new XLWorkbook(fs, XLEventTracking.Disabled))
            //{
            //    wb.Worksheets.Add(dataTable);
            //    wb.Save();
            //}
            //return fs;
            var fs = new MemoryStream();
            var package = new ExcelPackage(fs);
            var sheet = package.Workbook.Worksheets.Add("Sheet1");
            sheet.Cells.LoadFromCollection(collection, true, TableStyles.Dark1);
            package.Save();
            //using (var wb = new XLWorkbook(XLEventTracking.Disabled))
            //{
            //    wb.Worksheets.Add(dataTable, sheetName);
            //    wb.SaveAs(fs);
            //}
            //fs.Seek(0, SeekOrigin.Begin);
            return fs;

        }
        public static string ToString<T>(this IEnumerable<T> collection,
      Func<T, string> stringElement, string separator)
        {
            return ToString(collection, t => t.ToString(), separator);
        }

        public static string ToHtmlTable<T>(this IEnumerable<T> collection, params string[] style)
        {
            return collection.ToHtmlTable(null, null, false, style);
        }
        public static string ToHtmlTable<T>(this IEnumerable<T> collection, string hrefIdColumnName, string hrefParam, bool visibleFooter, params string[] style)
        {
            StringBuilder sr = new StringBuilder();
            sr.AppendLine("<style type='text/css'>");
            style.ToList().ForEach(str =>
            {
                sr.AppendLine(str);
            });
            sr.AppendLine("</style>");
            sr.AppendLine("<table cellspacing=\"3\" cellpadding=\"3\" border=\"1\" align=\"left\" style=\"{0}\">");
            sr.AppendLine("<thead>");
            sr.AppendLine("<tr>");
            Type elementType = typeof(T);
            foreach (var item in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;
                sr.AppendLine(string.Format("<th>{0}</th>", item.Name));
            }
            sr.AppendLine("</tr>");
            sr.AppendLine("</thead>");
            sr.AppendLine("<tbody>");
            foreach (var item in collection)
            {
                var rowNumber = 0;
                sr.AppendLine(string.Format("<tr class=\"{0}\">", rowNumber % 2 == 0 ? "R1" : "R2"));
                var properties = item.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    object value = property.GetValue(item, null) ?? string.Empty;
                    if (property.Name == hrefIdColumnName)
                    {
                        string a = string.Format("<a href='{1}{0}'>{0}</a>", value, hrefParam);
                        sr.AppendLine(string.Format("<td>{0}</td>", a));
                        continue;
                    }
                    sr.AppendLine(string.Format("<td>{0}</td>", value));
                }
                sr.AppendLine("</tr>");
                rowNumber++;
            }
            if (visibleFooter)
            {
                var footer = string.Format("<tfoot><tr>Total:<b> {0} Items </b></tr></tfoot>", collection.Count());
                sr.AppendLine(footer);
            }
            sr.AppendLine("</tbody>");
            sr.AppendLine("</table>");
            return sr.ToString();
        }

        public static string ToVerticalHtmlTable<T>(this IEnumerable<T> collection, string hrefIdColumnName = "", string hrefParam = "", string[] headerColumn = null, bool isFooter = false, params string[] style)
        {

            StringBuilder sr = new StringBuilder();
            var collectionArray = collection.ToArray();
            if (style.Count() > 0)
            {
                sr.AppendLine("<style type='text/css'>");
                style.ToList().ForEach(str =>
                {
                    sr.AppendLine(str);
                });
                sr.AppendLine("</style>");
            }
            else if (style.Count() == 0)
            {
                var css = Resources.css;
                sr.AppendLine("<style type='text/css'>");
                sr.AppendLine(string.Empty);
                sr.AppendLine("</style>");
            }
            sr.AppendLine("<table>");
            if (headerColumn != null)
            {
                if (headerColumn.Count() > 0)
                {
                    sr.AppendLine("<thead>");
                    sr.AppendLine("<tr>");
                    headerColumn.ToList().ForEach(f =>
                        {
                            sr.AppendLine(string.Format("<th>{0}</th>", f));
                        });
                    sr.AppendLine("</tr>");
                    sr.AppendLine("</thead>");
                }

            }
            sr.AppendLine("<tbody>");
            Type elementType = typeof(T);
            var array = collection.ToArray();
            //int i = 0;
            foreach (var item in elementType.GetProperties())
            {

                sr.AppendLine("<tr id='header'>");
                Type ColType = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;
                sr.AppendLine(string.Format("<td>{0}</td>", item.Name));
                List<object> values = new List<object>();
                array.ForEach(ar => ar.GetType().GetProperties().Where(w => w.Name == item.Name).ToList()
                    .ForEach(f =>
                     {
                         object value = f.GetValue(ar, null) ?? string.Empty;
                         if (f.Name == hrefIdColumnName)
                         {
                             string a = string.Format("<a href='{1}{0}'>{0}</a>", value, hrefParam);
                             sr.AppendLine(string.Format("<td>{0}</td>", a));
                             return;
                         }
                         sr.AppendLine(string.Format("<td>{0}</td>", value));
                     }));
                sr.AppendLine("</tr>");
            }



            if (isFooter)
            {
                var footer = string.Format("<tfoot><tr><td>Total:<b> {0} Items </b></td></tr></tfoot>", collection.Count());
                sr.AppendLine(footer);
            }
            sr.AppendLine("</tbody>");
            sr.AppendLine("</table>");
            return sr.ToString();
        }


        public static string ToHtmlTable<T>(this IEnumerable<T> list, string tableSyle, string headerStyle, string rowStyle, string alternateRowStyle)
        {
            var result = new StringBuilder();
            string root = String.IsNullOrEmpty(tableSyle) ? string.Format("<table id=\"" + typeof(T).Name + "Table\">") : string.Format("<table id=\"" + typeof(T).Name + "Table\" class=\"{0}\">", tableSyle);
            result.AppendLine(root);
            var propertyArray = typeof(T).GetProperties();
            foreach (var prop in propertyArray)
            {
                string header = String.IsNullOrEmpty(headerStyle) ? string.Format("<th>{0}</th>", prop.Name) : string.Format("<th class=\"{0}\">{1}</th>", headerStyle, prop.Name);
                result.AppendLine(header);
            }
            for (int i = 0; i < list.Count(); i++)
            {
                string tr = !String.IsNullOrEmpty(rowStyle) && !String.IsNullOrEmpty(alternateRowStyle) ? string.Format("<tr class=\"{0}\">", i % 2 == 0 ? rowStyle : alternateRowStyle) : "<tr>";
                foreach (var prop in propertyArray)
                {
                    object value = prop.GetValue(list.ElementAt(i), null); result.AppendFormat("<td>{0}</td>", value ?? String.Empty);
                }
                result.AppendLine("</tr>");
            }
            result.Append("</table>");
            return result.ToString();
        }

        public static string Compare2ToVerticalHtmlTable<T>(this IEnumerable<T> collection, string hrefIdColumnName = "", string hrefParam = "", string[] headerColumn = null, bool isFooter = false, string dateFormat = "d MMMM yyyy", params string[] style)
        {
            StringBuilder sr = new StringBuilder();
            var collectionArray = collection.ToArray();
            sr.AppendLine();
            sr.AppendLine("<style type='text/css'>");
            string css;
             
            css = style.Length > 0 ? css = string.Join("", style) : css = Resources.css;
            sr.AppendLine(css);
            sr.AppendLine("</style>");
            sr.AppendLine("<table>");
            if (headerColumn != null)
            {
                if (headerColumn.Count() > 0)
                {
                    sr.AppendLine("<thead>");
                    sr.AppendLine("<tr>");
                    headerColumn.ToList().ForEach(f =>
                    {
                        sr.AppendLine(string.Format("<th>{0}</th>", f));
                    });
                    sr.AppendLine("</tr>");
                    sr.AppendLine("</thead>");
                }

            }
            sr.AppendLine("<tbody>");
            Type elementType = typeof(T);
            var array = collection.ToArray();
            int iRowOdd = 0;
            foreach (var item in elementType.GetProperties())
            {
                iRowOdd = iRowOdd + 1;
                sr.AppendLine(string.Format("<tr id=\"R{0} \">", (iRowOdd % 2 == 0) ? "0" : "1"));
                Type ColType = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;
                sr.AppendLine(string.Format("<td><span>{0}</span></td>", item.Name));
                List<object> values = new List<object>();
                array.Take(2).ForEach(ar => ar.GetType().GetProperties().Where(w => w.Name == item.Name).ToList()
                    .ForEach(f =>
                    {
                        object value = f.GetValue(ar, null) ?? string.Empty;
                        if (f.Name == hrefIdColumnName)
                        {

                            string a = string.Format("<a href='{1}{0}'>{0}</a>", value, hrefParam);
                            sr.AppendLine(string.Format("<td><span>{0}</span></td>", a));
                            return;
                        }
                        DateTime date;
                        if (DateTime.TryParse(value.ToString(), out date))
                        {
                            sr.AppendLine(string.Format("<td><span>{0}</span></td>", date.ToString(dateFormat)));
                            return;
                        }
                        sr.AppendLine(string.Format("<td><span>{0}</span></td>", value));
                    }));

                var v1 = array[0].GetType().GetProperty(item.Name).GetValue(array[0], null) ?? string.Empty;
                var v2 = array[1].GetType().GetProperty(item.Name).GetValue(array[1], null) ?? string.Empty;
                sr.AppendLine((v1.Equals(v2)) ? "<td><span style=\"color:green;font-size:20px\">&#10003</span></td>" : "<td><span style=\"color:red;font-size:20px\">!!!</span></td>");

                sr.AppendLine("</tr>");
                //sr.AppendLine("<tr id='column'>");
            }
            if (isFooter)
            {
                var footer = string.Format("<tfoot><tr><td>Total:<b> {0} Items </b></td></tr></tfoot>", collection.Count());
                sr.AppendLine(footer);
            }
            sr.AppendLine("</tbody>");
            sr.AppendLine("</table>");


            return sr.ToString();
        }


        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
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
