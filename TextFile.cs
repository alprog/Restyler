using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public class TextFile
    {
        public FileInfo FileInfo;
        public List<Line> Lines = new List<Line>();
        private bool Changed = false;

        public TextFile(FileInfo fileInfo)
        {
            this.FileInfo = fileInfo;
            var rawLines = File.ReadAllLines(fileInfo.FullName);
            foreach (var rawLine in rawLines)
            {
                Lines.Add(new Line(this, rawLine));
            }
        }

        public void SetChanged()
        {
            this.Changed = true;
        }
       
        public void SaveIfChanged()
        {
            if (Changed)
            {
                var rawLines = new List<string>();
                foreach (var line in Lines)
                {
                    rawLines.Add(line);
                }
                File.WriteAllLines(FileInfo.FullName, rawLines);
                Changed = false;
            }
        }
    }
}
