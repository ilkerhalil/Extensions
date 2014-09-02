using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Extensions
{
    internal class Helper
    {
        public static IEnumerable<PropertyInfo> GetProperties(object item)
        {
            return item.GetType().GetProperties();
        }
        public static IEnumerable<PropertyInfo> GetProperties<T>(T item)
        {
            return item.GetType().GetProperties();
        }
    }
}
