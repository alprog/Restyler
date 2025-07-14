using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public class Restyler
    {
        private List<TextFile> OpenedFiles = new List<TextFile>();

        public void OpenFile(FileInfo fileInfo)
        {
            OpenedFiles.Add(new TextFile(fileInfo));
        }

        public void OpenFile(string path)
        {
            OpenFile(new FileInfo(path));
        }

        public void OpenFiles(List<FileInfo> fileInfos)
        {
            foreach (var fileInfo in fileInfos)
            {
                OpenFile(fileInfo);
            }
        }

        public List<Line> GetAllLines()
        {
            var lines = new List<Line>();
            foreach (var file in OpenedFiles)
            {
                lines.AddRange(file.Lines);
            }
            return lines;
        }

        public List<Line> GetAllLines(Func<string, bool> predicate)
        {
            var lines = new List<Line>();
            foreach (var file in OpenedFiles)
            {
                foreach (var line in file.Lines)
                {
                    if (predicate(line))
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        public void SaveAndClose()
        {
            foreach (var file in OpenedFiles)
            {
                file.SaveIfChanged();
            }
            OpenedFiles.Clear();
        }
    }
}
