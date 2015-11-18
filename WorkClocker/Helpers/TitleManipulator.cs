using System;
using System.Collections.Generic;

namespace WorkClocker.Helpers
{
    internal class TitleManipulator
    {
        private static readonly Dictionary<string,string> ExeDictionary = new Dictionary<string, string>()
        {
            {"chrome", "Google Chrome"},
            {"sublime_text", "Sublime Text"},
            {"devenv", "Visual Studio"}
        };

        private static readonly Dictionary<string, Func<string, string>> TitleDictionary = new Dictionary
            <string, Func<string, string>>()
        {
            {"chrome", title => title.Substring(0, title.Length - 16)}
        };

        private static void CleanExe(ref WindowExe e)
        {
            if (ExeDictionary.ContainsKey(e.Exe))
                e.Exe = ExeDictionary[e.Exe];
        }
        
        private static void CleanAppTitle(ref WindowExe e)
        {
            if (!TitleDictionary.ContainsKey(e.Exe))
                return;

            e.Title = TitleDictionary[e.Exe].Invoke(e.Title);
        }

        public static void CleanTitle(ref WindowExe e)
        {
            CleanAppTitle(ref e);
            CleanExe(ref e);
        }
    }
}
