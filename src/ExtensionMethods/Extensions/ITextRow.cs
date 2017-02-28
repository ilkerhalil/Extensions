using System.Text;

namespace Extensions
{
    public interface ITextRow
    {
        string Output();
        void Output(StringBuilder sb);
        object Tag { get; set; }
    }
}
