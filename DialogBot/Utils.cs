using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DialogBot
{
    public static class Utils
    {
        public static string NextTo(this string[] str, string pat)
        {
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == pat) return str[i + 1];
            }
            return "";
        }

        public static bool IsPresent(this string[] str, string pat)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == pat) return true;
            }
            return false;
        }
        public static bool IsPresent(this string[] str,Regex exp)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (exp.IsMatch(str[i])) return true;
            }
            return false;
        }
    }
}