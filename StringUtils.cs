using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public static class StringUtils
    {
        public static string Replace(this string str, int startIndex, int count, string newString)
        {
            return str.Substring(0, startIndex) + newString + str.Substring(startIndex + count);
        }

        public static int IndexAfter(this string str, string substring, int startIndex = 0)
        {
            int index = str.IndexOf(substring, startIndex);
            if (index >= 0)
            {
                index += substring.Count();
            }
            return index;
        }

        public static List<string> ParseWords(this string str)
        {
            var words = new List<string>();

            int start = 0;
            int index = 0;

            var flushWord = () =>
            {
                var word = str.Substring(start, index - start);
                word = Char.ToUpper(word[0]) + word.Substring(1);
                words.Add(word);
                start = index;
            };

            while (++index < str.Count())
            {
                char c = str[index];
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

        public static string ToSnakeCase(this string str)
        {
            return String.Join('_', str.ParseWords()).ToLower();
        }

        public static string ToPascalCase(this string str)
        {
            return String.Join(null, str.ParseWords());
        }

        public static string ToCase(this string str, CaseStyle caseStyle)
        {
            switch (caseStyle)
            {
                case CaseStyle.Pascal:
                    return str.ToPascalCase();

                case CaseStyle.Snake:
                    return str.ToSnakeCase();
            }

            throw new Exception("invalid case style");
        }
    }
}
