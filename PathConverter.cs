using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public class PathConverter
    {
        private CaseStyle CaseStyle;
        public HashSet<string> IgnoredFolders = new HashSet<string>();

        public PathConverter(CaseStyle caseStyle)
        {
            this.CaseStyle = caseStyle;
        }

        public string Convert(string path)
        {
            var list = path.Split('\\').ToList();
            int count = list.Count();
            for (int i = 0; i < count; i++)
            {
                list[i] = list[i].ToCase(CaseStyle);
                if (IgnoredFolders.Contains(list[i].ToLower()))
                {
                    break;
                }
            }
            return String.Join('\\', list);
        }

    }
}
