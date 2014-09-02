using System;
using System.Linq;
using System.Text;

namespace Extensions
{
    public interface ITextRow
    {
        String Output();
        void Output(StringBuilder sb);
        Object Tag { get; set; }
    }
}
