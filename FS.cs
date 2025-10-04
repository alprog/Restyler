using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public static class FS
    {
        public static string RootFolder = "C:\\finik\\source";
        public static string ProjectPath = Path.Join(RootFolder, "finik.vcxproj");
        public static List<string> IgnoredFolderNames = new List<string>() { "3rd-party\\", ".vs" };

        public static bool IsIgnoredPath(string path)
        {
            foreach (var ignoredFolderName in IgnoredFolderNames)
            {
                if (path.ToLower().Contains(ignoredFolderName))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<DirectoryInfo> GetAllDirectories()
        {
            var infos = new List<DirectoryInfo>();
            var directoryInfo = new DirectoryInfo(RootFolder);
            infos.AddRange(directoryInfo.GetDirectories("*", SearchOption.AllDirectories));
            infos.RemoveAll((DirectoryInfo info) => { return IsIgnoredPath(info.FullName); });
            return infos;
        }

        public static List<FileInfo> GetAllFiles()
        {
            var infos = new List<FileInfo>();
            var directoryInfo = new DirectoryInfo(RootFolder);
            infos.AddRange(directoryInfo.GetFiles("*.cpp", SearchOption.AllDirectories));
            infos.AddRange(directoryInfo.GetFiles("*.ixx", SearchOption.AllDirectories));
            infos.AddRange(directoryInfo.GetFiles("*.h", SearchOption.AllDirectories));
            infos.RemoveAll((FileInfo info) => { return IsIgnoredPath(info.FullName); });
            return infos;
        }
    }
}
