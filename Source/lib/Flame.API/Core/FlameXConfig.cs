using System.Text.RegularExpressions;
using RC.Framework;

namespace Flame.API.Core
{
    using System;
    using System.Collections.Generic;

    public class FlameXConfig
    {
        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();
        private readonly List<string> includes = new List<string>();
        private readonly Dictionary<string, string> byteset = new Dictionary<string, string>();

        private static readonly Regex IncludeRex = new Regex(@"^#include <([A-Za-z]{1,}\.[a-z]{1,4})>;$");
        private static readonly Regex CommentRex = new Regex(@"^(\/\*.{1,}?\*\/).+$", RegexOptions.Singleline);
        private static readonly Regex ByteSetRex = new Regex(@"^#byteset (0x[0-9A-F]{1,}) >> (0x[0-9A-F]{1,});$");
        private static readonly Regex HarmonyRex = new Regex(@"^#harmony (true|false|[0-1]{1});$");
        private static readonly Regex VarsRex    = new Regex(@"^<""([A-Za-z]{1,}"")>\(\""([A-Za-z]{1,})\""\);$");

        private FlameXConfig() { }

        public string get(string key)
        {
            key = key.ToLower();
            if (!_dictionary.ContainsKey(key))
                throw new Exception($"{key} is not defined.");
            return _dictionary[key];
        }

        private void set(string key, string value)
        {
            key = key.ToLower();
            if (_dictionary.ContainsKey(key))
                throw new Exception($"{key} is already defined.");
            _dictionary.Add(key, value);
        }

        public string this[string key]
        {
            get         { return this.get(key); }
            private set { this.set(key, value); }
        }

        public bool Is(string key)
        {
            if (!_dictionary.ContainsKey(key)) return false;
            bool res = false;
            if (Boolean.TryParse(_dictionary[key], out res))
                return true;
            return res;
        }

        public static FlameXConfig Parse(string siu)
        {
            siu = siu.Replace("\r", "");
            FlameXConfig x = new FlameXConfig();
            while (CommentRex.IsMatch(siu))
                siu = siu.Replace(CommentRex.Match(siu).Groups[1].Value, "");
            foreach (var s in siu.Split('\n'))
            {
                if(string.IsNullOrWhiteSpace(s))
                    continue;
                if (string.IsNullOrEmpty(s))
                    continue;
                if (IncludeRex.IsMatch(s))
                {
                    x.includes.Add(IncludeRex.Match(s).Groups[1].Value);
                    continue;
                }
                if (ByteSetRex.IsMatch(s))
                    continue;
                if (HarmonyRex.IsMatch(s))
                {
                    x["harmony"] = HarmonyRex.Match(s).Groups[1].Value;
                    continue;
                }
                if (VarsRex.IsMatch(s))
                {
                    x[VarsRex.Match(s).Groups[1].Value] = VarsRex.Match(s).Groups[2].Value;
                    continue;
                }
                Terminal.WriteLine($"[{RCL.Wrap("FX-Config", ConsoleColor.DarkCyan)}][{RCL.Wrap("ERROR", ConsoleColor.Red)}]: Syntax ERROR, line: ['{s}']. Ignored!");
            }
            return x;
        }
    }
}