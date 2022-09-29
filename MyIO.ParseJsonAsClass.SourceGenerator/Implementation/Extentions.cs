using System;
using System.Collections.Generic;
using System.Text;

namespace MyIO.ParseJsonAsClass.SourceGenerator.Implementation
{
    internal static class Extentions
    {
        internal static string ToTitleCase(this string str)
        {
            var sb = new StringBuilder(str.Length);
            var flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(flag ? char.ToUpper(c) : c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }

    }
}
