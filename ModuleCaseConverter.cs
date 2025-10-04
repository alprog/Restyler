using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    class ModuleCaseConverter
    {
        private CaseStyle CaseStyle;

        public ModuleCaseConverter(CaseStyle caseStyle)
        {
            this.CaseStyle = caseStyle;
        }

        public void Run()
        {
            var restyler = new Restyler();
            restyler.OpenFiles(FS.GetAllFiles());

            var beginSequences = new List<string> { "module ", "import " };

            foreach (var beginSequence in beginSequences)
            {
                var lines = restyler.GetAllLines((string line) => line.Contains(beginSequence));

                foreach (var line in lines)
                {
                    var subline = line.GetSubLineBetween(beginSequence, ";");
                    if (subline.IsValid())
                    {
                        var value = subline.Value;
                        if (value == "std" || value[0] == '<')
                        {
                            continue;
                        }

                        subline.Value = value.ToCase(CaseStyle);
                    }
                }
            }

            restyler.SaveAndClose();
        }
    }
}
