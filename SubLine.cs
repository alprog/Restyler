using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public struct SubLine
    {
        Line Line;
        int StartIndex;
        int Count;

        public SubLine(Line line, int startIndex, int count)
        {
            this.Line = line;
            this.StartIndex = startIndex;
            this.Count = count;
        }

        public bool IsValid()
        {
            return StartIndex >= 0;
        }

        public string Value
        {
            get
            {
                return Line.Value.Substring(StartIndex, Count);
            }
            set
            {
                Line.Value = Line.Value.Replace(StartIndex, Count, value);
                Count = value.Count();
            }
        }

        public void ExcludeExtension()
        {
            int index = Value.LastIndexOf('.');
            if (index >= 0)
            {
                this.Count = index;
            }
        }

        public static implicit operator string(SubLine line)
        {
            return line.Value;
        }
    }
}
