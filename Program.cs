
using System.IO;

string Replace(string str, int startIndex, int endIndex, string newString)
{
    return str.Substring(0, startIndex) + newString + str.Substring(endIndex);
}

List<string> ParseWords(string name)
{
    var words = new List<string>();

    int start = 0;
    int index = 0;

    var flushWord = () => 
    {
        var word = name.Substring(start, index - start);
        word = Char.ToUpper(word[0]) + word.Substring(1);
        words.Add(word);
        start = index;
    };

    while (++index < name.Count())
    {
        char c = name[index];
        if (c == '_')
        {
            flushWord();
            start++;
            index++;
        }
        else if (Char.IsUpper(c))
        {
            flushWord();
        }
    }
    flushWord();

    return words;
}

string ToSnakeCase(string name)
{
    return String.Join('_', ParseWords(name)).ToLower();
}

string ToPascalCase(string name)
{
    return String.Join(null, ParseWords(name));
}

var rootFolder = "C:/finik/source";
var projectPath = Path.Join(rootFolder, "finik.vcxproj");
var ignoredFolderName = "3rd-party";

List<FileInfo> GetAllFiles()
{
    var infos = new List<FileInfo>();
    var directoryInfo = new DirectoryInfo(rootFolder);
    infos.AddRange(directoryInfo.GetFiles("*.cpp", SearchOption.AllDirectories));
    infos.AddRange(directoryInfo.GetFiles("*.ixx", SearchOption.AllDirectories));
    infos.AddRange(directoryInfo.GetFiles("*.h", SearchOption.AllDirectories));
    infos.RemoveAll((FileInfo info) => { return info.FullName.Contains(ignoredFolderName); });
    return infos;
}

void Main()
{
    foreach (var fileInfo in GetAllFiles())
    {
        var oldPath = fileInfo.FullName;
        var directory = fileInfo.DirectoryName;
        var newName = ToPascalCase(Path.GetFileNameWithoutExtension(fileInfo.Name));
        var extenstion = fileInfo.Extension;
        var newPath = Path.Join(directory, newName + extenstion);
        File.Move(oldPath, newPath);
    }

    var projectLines = File.ReadAllLines(projectPath);

    var sequence = " Include=";
    for (int i = 0; i < projectLines.Count(); i++)
    {
        var line = projectLines[i];

        if (line.Contains(ignoredFolderName))
        {
            continue;
        }

        if (!line.Contains("ClCompile") && !line.Contains("ClInclude"))
        {
            continue;
        }

        int startIndex = line.IndexOf(sequence);
        if (startIndex < 0)
        {
            continue;
        }
        startIndex += sequence.Count() + 1;

        var endIndex = line.IndexOf('.', startIndex);
        startIndex = Math.Max(startIndex, line.LastIndexOf('\\', endIndex) + 1);

        var newName = ToPascalCase(line.Substring(startIndex, endIndex - startIndex));
        projectLines[i] = Replace(line, startIndex, endIndex, newName);
    }
    File.WriteAllLines(projectPath, projectLines);
}

Main();