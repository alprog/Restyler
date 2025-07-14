using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public class Line
    {
        public TextFile TextFile;
        private string m_value;

        public Line(TextFile textFile, string value)
        {
            this.TextFile = textFile;
            this.m_value = value;
        }

        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (m_value != value)
                {
                    m_value = value;
                    TextFile.SetChanged();
                }
            }
        }

        public SubLine GetSubLineBetween(string after, string before)
        {
            int startIndex = m_value.IndexAfter(after);
            int endIndex = startIndex >= 0 ? m_value.IndexOf(before, startIndex) : -1;
            int count = endIndex - startIndex;
            return GetSubLine(startIndex, count);
        }

        public SubLine GetSubLine(int startIndex, int count)
        {
            return new SubLine(this, startIndex, count);
        }

        public static implicit operator string(Line line)
        {
            return line.m_value;
        }

        public bool Test(Func<string, bool> predicate)
        {
            return predicate(Value);
        }
    }
}
