using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    class ModuleFolderConverter
    {
        public void Run()
        {
            var restyler = new Restyler();
            restyler.OpenFiles(FS.GetAllFiles());

            var exportModuleSeq = "export module ";
            var exportLines = restyler.GetAllLines((string line) => line.Contains("export module "));

            var map = new Dictionary<string, string>();

            foreach (var line in exportLines)
            {
                var subline = line.GetSubLineBetween(exportModuleSeq, ";");
                if (subline.IsValid())
                {
                    var fileInfo = line.TextFile.FileInfo;
                 
                    var dirPath = fileInfo.DirectoryName;
                    if (!dirPath.Contains("Core"))
                    {
                        continue;
                    }

                    var relDirPath = dirPath.Substring(FS.RootFolder.Count() + 1);

                    var modulePath = Path.Combine("Finik", relDirPath).Replace('\\', '.');

                    var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    if (!fileName.StartsWith("_"))
                    {
                        modulePath += "." + fileName;
                    }

                    if (subline.Value.Contains(':'))
                    {
                        int index = modulePath.LastIndexOf('.');
                        modulePath = modulePath.Replace(index, 1, ":");
                    }

                    if (subline.Value != modulePath)
                    {
                        if (modulePath.EndsWith(subline.Value))
                        {
                            map[subline.Value] = modulePath;
                            subline.Value = modulePath;
                        }
                        else
                        {
                            Console.WriteLine(subline.Value);
                            Console.WriteLine(modulePath);
                        }
                    }
                }
            }

            var importModuleSeq = "import ";
            var importLines = restyler.GetAllLines((string line) => line.Contains(importModuleSeq));
            foreach (var line in importLines)
            {
                var subline = line.GetSubLineBetween(importModuleSeq, ";");
                if (subline.IsValid())
                {
                    if (map.TryGetValue(subline.Value, out string replaceValue))
                    {
                        subline.Value = replaceValue;
                    }
                }
            }

            restyler.SaveAndClose();
        }
    }
}
