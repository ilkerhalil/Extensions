using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions
{
    public class TableBuilder : IEnumerable<ITextRow>
    {
        private class TextRow : List<string>, ITextRow
        {
            private readonly TableBuilder _owner;
            public TextRow(TableBuilder owner)
            {
                _owner = owner;
                if (_owner == null) throw new ArgumentException("Owner");
            }
            public string Output()
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
                    var p = string.Format(_owner.FormatString, ToArray());
                    sb.Append(p);
                //}
            }
            public object Tag { get; set; }
        }

        private string Separator { get; set; }

        private readonly List<ITextRow> _rows = new List<ITextRow>();
        private readonly List<int> _colLength = new List<int>();

        public TableBuilder()
        {
            Separator = "  ";
        }

        public TableBuilder(string separator)
            : this()
        {
            Separator = separator;
        }

        public ITextRow AddRow(params string[][] cols)
        {
            var row = new TextRow(this);
            foreach (object o in cols)
            {
                var str = o.ToString().Trim();
                row.Add(str);
                if (_colLength.Count >= row.Count)
                {
                    var curLength = _colLength[row.Count - 1];
                    if (str.Length > curLength) _colLength[row.Count - 1] = str.Length;
                }
                else
                {
                    _colLength.Add(str.Length);
                }
            }
            _rows.Add(row);
            return row;
        }

        private string _fmtString;

        private string FormatString
        {
            get
            {
                if (_fmtString != null) return _fmtString;
                var format = "";
                var i = 0;
                foreach (var len in _colLength)
                {
                    format += $"{{{i++},-{len}}}{Separator}";
                }
                format += "\r\n";
                _fmtString = format;
                return _fmtString;
            }
        }

        public string Output()
        {
            var sb = new StringBuilder();
            foreach (var textRow in _rows)
            {
                var row = (TextRow) textRow;
                row.Output(sb);
            }
            return sb.ToString();
        }

        #region IEnumerable Members

        public IEnumerator<ITextRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        #endregion
    }
}