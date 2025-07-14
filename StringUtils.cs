using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Restyler
{
    public static class StringUtils
    {
        private static List<string> DoubleAbbreviations = new List<string> { "DX" };

        private static CharType GetCharType(char c)
        {
            if (Char.IsAsciiLetterLower(c))
            {
                return CharType.LowerAlpha;
            }

            if (Char.IsAsciiLetterUpper(c))
            {
                return CharType.UpperAlpha;
            }

            if (Char.IsDigit(c))
            {
                return CharType.Digit;
            }

            return CharType.Symbol;
        }

        private static bool IsWord(string str)
        {
            return Char.IsLetter(str.Last()); // check last letter because 2nd and 3rd are also words
        }

        private static string CapitalizeWord(this string str)
        {
            if (str.Count() == 2)
            {
                var allCaps = str.ToUpperInvariant();
                if (DoubleAbbreviations.Contains(allCaps))
                {
                    return allCaps;
                }
            }

            return Char.ToUpperInvariant(str[0]) + str.Substring(1).ToLowerInvariant();
        }

        private static void AddWordSeparators(List<string> tokens)
        {
            bool IsPrevWord = false;
            for (int i = tokens.Count() - 1; i >= 0; i--)
            {
                bool IsCurWord = IsWord(tokens[i]);
                if (IsCurWord && IsPrevWord)
                {
                    tokens.Insert(i + 1, "_");
                }
                IsPrevWord = IsCurWord;
            }
        }

        private static void RemoveWordsSeparators(List<string> tokens)
        {
            for (int i = tokens.Count() - 2; i >= 1; i--)
            {
                if (tokens[i] == "_")
                {
                    if (IsWord(tokens[i - 1]) && IsWord(tokens[i + 1]))
                    {
                        tokens.RemoveAt(i);
                    }
                }
            }
        }

        private static void CombineWithNextToken(List<string> tokens, int index)
        {
            tokens[index] = tokens[index] + tokens[index + 1];
            tokens.RemoveAt(index + 1);
        }

        private static void CombineSpecialTokens(List<string> tokens)
        {
            var index = tokens.IndexOf("3");
            if (index >= 0 && index < tokens.Count() - 1)
            {
                if (tokens[index + 1].ToLowerInvariant() == "rd")
                {
                    CombineWithNextToken(tokens, index);
                }
            }
        }

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

        public static List<string> Tokenize(this string str)
        {
            var tokens = new List<string>();

            var tokenStart = -1;
            var tokenType = CharType.None;

            for (int i = 0; i < str.Count(); i++)
            {
                var type = GetCharType(str[i]);
                if (type == tokenType)
                {
                    continue; // full match
                }
                if (tokenType == CharType.UpperAlpha && type == CharType.LowerAlpha && i == tokenStart + 1)
                {
                    tokenType = CharType.LowerAlpha;
                    continue; // capitalized word
                }

                if (tokenType != CharType.None)
                {
                    int length = i - tokenStart;
                    tokens.Add(str.Substring(tokenStart, length));
                }

                // start new token
                tokenStart = i;
                tokenType = type;
            }

            if (tokenType != CharType.None)
            {
                tokens.Add(str.Substring(tokenStart));
            }

            CombineSpecialTokens(tokens);

            return tokens;
        }


        public static string ToSnakeCase(this string str)
        {
            var tokens = str.Tokenize();
            AddWordSeparators(tokens);
            return String.Concat(tokens).ToLowerInvariant();
        }

        public static string ToAllCaps(this string str)
        {
            var tokens = str.Tokenize();
            AddWordSeparators(tokens);
            return String.Concat(tokens).ToUpperInvariant();
        }

        public static string ToPascalCase(this string str)
        {
            var tokens = str.Tokenize();
            RemoveWordsSeparators(tokens);
            for (int i = 0; i < tokens.Count(); i++)
            {
                tokens[i] = CapitalizeWord(tokens[i]);
            }
            return String.Concat(tokens);
        }

        public static string ToCase(this string str, CaseStyle caseStyle)
        {
            switch (caseStyle)
            {
                case CaseStyle.Pascal:
                    return str.ToPascalCase();

                case CaseStyle.Snake:
                    return str.ToSnakeCase();

                case CaseStyle.Caps:
                    return str.ToAllCaps();
            }

            throw new Exception("invalid case style");
        }
    }
}
