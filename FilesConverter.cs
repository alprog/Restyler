using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public class FilesConverter
    {
        private CaseStyle CaseStyle;

        public FilesConverter(CaseStyle caseStyle)
        {
            this.CaseStyle = caseStyle;
        }

        public void Run()
        {
            ConvertFileNames();
            ConvertFolderNames();
            ConvertProjectIncludes();
        }

        void ConvertFileNames()
        {
            foreach (var fileInfo in FS.GetAllFiles())
            {
                var oldPath = fileInfo.FullName;
                var directory = fileInfo.DirectoryName;
                var newName = Path.GetFileNameWithoutExtension(fileInfo.Name).ToCase(CaseStyle);
                var extenstion = fileInfo.Extension;
                var newPath = Path.Join(directory, newName + extenstion);
                if (oldPath != newPath)
                {
                    File.Move(oldPath, newPath);
                }
            }
        }

        void ConvertFolderNames()
        {
            foreach (var dirInfo in FS.GetAllDirectories())
            {
                var oldPath = dirInfo.FullName;
                var newName = dirInfo.Name.ToCase(CaseStyle);
                var newPath = Path.Join(dirInfo.Parent.FullName, newName);
                if (oldPath != newPath)
                {
                    Directory.Move(oldPath, newPath);
                }
            }
        }

        string FixFolderName(string name)
        {
            if (name.EndsWith('\\'))
            {
                return name.Substring(0, name.Count() - 1);
            }
            return name;
        }

        void ConvertProjectIncludes()
        {
            var restyler = new Restyler();
            restyler.OpenFile(FS.ProjectPath);

            var converter = new PathConverter(CaseStyle);
            foreach (var folderName in FS.IgnoredFolderNames)
            {
                converter.IgnoredFolders.Add(FixFolderName(folderName));
            }

            var isIncludeLine = (string s) => s.Contains("ClCompile") || s.Contains("ClInclude");
            foreach (var line in restyler.GetAllLines(isIncludeLine))
            {
                var subLine = line.GetSubLineBetween("Include=\"", "\"");
                if (subLine.IsValid())
                {
                    subLine.ExcludeExtension();
                    subLine.Value = converter.Convert(subLine.Value);
                }
            }

            restyler.SaveAndClose();
        }


    }
}
