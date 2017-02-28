using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Extensions.ObjectExtensions
{
    public static class ObjectExtensions
    {
        public static string ToXmlSerialize(this object o)
        {
            var sw = new StringWriter();
            var serializer = new XmlSerializer(o.GetType());
            serializer.Serialize(sw, o);
            return sw.ToString();
        }
        public static XElement ToXElementSerialize(this object o)
        {
            var xs = new XmlSerializer(o.GetType());
            var d = new XDocument();
            using (var w = d.CreateWriter()) xs.Serialize(w, o);
            var e = d.Root;
            e.Remove();
            return e;
        }
        public static object ToXElementDeSerialize(this object o, string element)
        {
            var xs = new XmlSerializer(o.GetType());
            var sr = new StringReader(element);
            return xs.Deserialize(sr);
        }

        public static bool CheckNull(this object o)
        {
            return o == null;
        }

        public static string ToStringExtension(this object obj, string dateFormat = "dd.MM.yyyy")
        {
            if (obj == null) return "Empty";
            var sb = new StringBuilder();
            foreach (var property in obj.GetType().GetProperties())
            {
                sb.Append(property.Name);
                sb.Append(": ");

                if (property.GetIndexParameters().Length > 0)
                {
                    sb.Append("Indexed Property cannot be used");
                }
                else
                {
                    var value = property.GetValue(obj, null) ?? string.Empty;
                    if (property.PropertyType == typeof(bool?))
                    {
                        bool p;
                        if (bool.TryParse(value.ToString(), out p))
                        {
                            sb.Append(p ? "Var" : "Yok");
                        }
                    }
                    else if (property.PropertyType == typeof(DateTime?))
                    {
                        DateTime date;
                        sb.Append(DateTime.TryParse(value.ToString(), out date)
                            ? date.ToString(dateFormat)
                            : string.Empty);
                    }
                    else if (property.PropertyType == typeof(decimal?))
                    {
                        decimal money;
                        var moneyFormat = decimal.TryParse(value.ToString(), out money) ? money.ToString("C") : string.Empty;
                        sb.Append(moneyFormat);
                    }
                    else
                    {
                        sb.Append(value);
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }


    }
}
