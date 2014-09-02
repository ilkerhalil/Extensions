using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public class TableBuilder : IEnumerable<ITextRow>
    {
        protected class TextRow : List<string>, ITextRow
        {
            protected TableBuilder owner = null;
            public TextRow(TableBuilder Owner)
            {
                owner = Owner;
                if (owner == null) throw new ArgumentException("Owner");
            }
            public String Output()
            {
                var sb = new StringBuilder();
                Output(sb);
                return sb.ToString();
            }
            public void Output(StringBuilder sb)
            {
                //List<object> p1 = new List<object>();
                //if (this.Count == 1)
                //{
                //    this.ToList().ForEach(f => f.Split(' ').ToList().ForEach(s => p1.Add(s.Trim())));
                //    var p = string.Format(owner.FormatString, p1.ToArray());
                //    sb.Append(p);
                //}
                //else
                //{
                    var p = string.Format(owner.FormatString, this.ToArray());
                    sb.Append(p);
                //}
            }
            public Object Tag { get; set; }
        }

        public String Separator { get; set; }

        protected List<ITextRow> rows = new List<ITextRow>();
        protected List<int> colLength = new List<int>();

        public TableBuilder()
        {
            Separator = "  ";
        }

        public TableBuilder(String separator)
            : this()
        {
            Separator = separator;
        }

        public ITextRow AddRow(params string[][] cols)
        {
            TextRow row = new TextRow(this);
            foreach (object o in cols)
            {
                String str = o.ToString().Trim();
                row.Add(str);
                if (colLength.Count >= row.Count)
                {
                    int curLength = colLength[row.Count - 1];
                    if (str.Length > curLength) colLength[row.Count - 1] = str.Length;
                }
                else
                {
                    colLength.Add(str.Length);
                }
            }
            rows.Add(row);
            return row;
        }

        protected String _fmtString = null;
        public String FormatString
        {
            get
            {
                if (_fmtString == null)
                {
                    String format = "";
                    int i = 0;
                    foreach (int len in colLength)
                    {
                        format += String.Format("{{{0},-{1}}}{2}", i++, len, Separator);
                    }
                    format += "\r\n";
                    _fmtString = format;
                }
                return _fmtString;
            }
        }

        public String Output()
        {
            var sb = new StringBuilder();
            foreach (TextRow row in rows)
            {
                row.Output(sb);
            }
            return sb.ToString();
        }

        #region IEnumerable Members

        public IEnumerator<ITextRow> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        #endregion
    }
}