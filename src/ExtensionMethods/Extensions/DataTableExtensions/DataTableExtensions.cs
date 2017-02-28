namespace Extensions.DataTableExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using OfficeOpenXml;
    using OfficeOpenXml.Table;

    public static class DataTableExtensions
    {
        public static string ConvertDataTableToString(this DataTable dt)
        {
            var stringBuilder = new StringBuilder();
            dt.Rows.Cast<DataRow>().ToList().ForEach(dataRow =>
            {
                dt.Columns.Cast<DataColumn>().ToList().ForEach(column => stringBuilder.AppendFormat("{0}:{1} ", column.ColumnName, dataRow[column]));
                stringBuilder.Append(Environment.NewLine);
            });
            return stringBuilder.ToString();
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            var tb = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static Type GetCoreType(Type t)
        {
            if (t == null || !IsNullable(t)) return t;
            return !t.IsValueType ? t : Nullable.GetUnderlyingType(t);
        }

        public static DataTable ToDataTable(this IEnumerable<dynamic> items)
        {
            var data = items.ToArray();
            if (!data.Any()) return null;

            var dt = new DataTable();
            foreach (var key in ((IDictionary<string, object>)data[0]).Keys)
            {
                dt.Columns.Add(key);
            }
            foreach (var d in data)
            {
                dt.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
            }
            return dt;
        }

        public static Stream ToExcel(this DataTable dataTable, string sheetName = "Sheet1", TableStyles tableStyle= TableStyles.Medium28)
        {
            var fs = new MemoryStream();
            var package = new ExcelPackage(fs);
            var sheet = package.Workbook.Worksheets.Add(sheetName);
            if (dataTable?.Rows != null && dataTable.Rows.Count > 0) { sheet.Cells.LoadFromDataTable(dataTable, true, tableStyle); }
            else sheet.Cells["A2"].Value = "Gösterilecek data yok..!";
            sheet.Column(1).AutoFit();
            package.Save();
            return fs;
        }
    }
}
