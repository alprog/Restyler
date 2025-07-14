
using System;
using System.IO;
using Restyler;

var rootFolder = "C:/finik/source";
var projectPath = Path.Join(rootFolder, "finik.vcxproj");
var ignoredFolderNames = new List<string>() { "3rd-party\\", ".vs" };

bool IsIgnoredPath(string path)
{
    foreach (var ignoredFolderName in ignoredFolderNames)
    {
        if (path.ToLower().Contains(ignoredFolderName))
        {
            return true;
        }
    }
    return false;
}

List<DirectoryInfo> GetAllDirectory()
{
    var infos = new List<DirectoryInfo>();
    var directoryInfo = new DirectoryInfo(rootFolder);
    infos.AddRange(directoryInfo.GetDirectories("*", SearchOption.AllDirectories));
    infos.RemoveAll((DirectoryInfo info) => { return IsIgnoredPath(info.FullName); });
    return infos;
}

List<FileInfo> GetAllFiles()
{
    var infos = new List<FileInfo>();
    var directoryInfo = new DirectoryInfo(rootFolder);
    infos.AddRange(directoryInfo.GetFiles("*.cpp", SearchOption.AllDirectories));
    infos.AddRange(directoryInfo.GetFiles("*.ixx", SearchOption.AllDirectories));
    infos.AddRange(directoryInfo.GetFiles("*.h", SearchOption.AllDirectories));
    infos.RemoveAll((FileInfo info) => { return IsIgnoredPath(info.FullName); });
    return infos;
}

void ConvertFileNames(CaseStyle caseStyle)
{
    foreach (var fileInfo in GetAllFiles())
    {
        var oldPath = fileInfo.FullName;
        var directory = fileInfo.DirectoryName;
        var newName = Path.GetFileNameWithoutExtension(fileInfo.Name).ToCase(caseStyle);
        var extenstion = fileInfo.Extension;
        var newPath = Path.Join(directory, newName + extenstion);
        File.Move(oldPath, newPath);
    }
}

void ConvertFolderNames(CaseStyle caseStyle)
{
    foreach (var dirInfo in GetAllDirectory())
    {
        var oldPath = dirInfo.FullName;
        var newName = dirInfo.Name.ToCase(caseStyle);
        var newPath = Path.Join(dirInfo.Parent.FullName, newName);
        if (oldPath != newPath)
        {
            Directory.Move(oldPath, newPath);
        }
    }
}

void ConvertProjectIncludes(CaseStyle caseStyle)
{
    var restyler = new Restyler.Restyler();
    restyler.OpenFile(projectPath);

    var converter = new PathConverter(caseStyle);
    converter.IgnoredFolders.Add("3rd-party");

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

void ConvertFiles(CaseStyle caseStyle)
{
    ConvertFileNames(caseStyle);
    ConvertFolderNames(caseStyle);
    ConvertProjectIncludes(caseStyle);
}

void Main()
{
    ConvertFiles(CaseStyle.Pascal);
}

Main();