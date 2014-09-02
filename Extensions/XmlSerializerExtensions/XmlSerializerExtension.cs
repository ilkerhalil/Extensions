using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Extensions.XmlSerializerExtensions
{
    public static class XmlSerializerExtensions
    {
        public static XElement SerializeAsXElement(this XmlSerializer xs, object o) {
           XDocument d = new XDocument();
           using (XmlWriter w = d.CreateWriter()) xs.Serialize(w, o);
           XElement e = d.Root;
           e.Remove();
           return e;
       }
    }
}
