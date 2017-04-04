using System.Drawing;

namespace FlameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Linq;
    using Rc.Framework.Screens;

    public class FlameXConfig
    {
        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();
        private readonly List<string> includes = new List<string>();
        private readonly Dictionary<string, string> byteset = new Dictionary<string, string>();

        private static readonly Regex IncludeRex = new Regex(@"^#include <([A-Za-z]{1,}\.[a-z]{1,4})>\(\);$");
        private static readonly Regex CommentRex = new Regex(@"^(\/\*.{1,}?\*\/).+$", RegexOptions.Singleline);
        private static readonly Regex ByteSetRex = new Regex(@"^#byteset (0x[0-9A-F]{1,}) >> (0x[0-9A-F]{1,});$");
        private static readonly Regex HarmonyRex = new Regex(@"^#harmony (true|false|[0-1]{1});$");
        private static readonly Regex VarsRex = new Regex(@"^<""([A-Za-z.]{1,})"">\(\""([A-Za-z0-9.><_\\\/:{}\?\=\-\*\&\^\%\$\#\@\!\`\~\+]{1,})\""\);$");

        private FlameXConfig() { }

        private readonly object Guarder = new object();

        public string get(string key)
        {
            lock (Guarder)
            {
                if (!_dictionary.ContainsKey(key.ToLower()))
                    throw new Exception($"{key} is not defined.");
                return _dictionary[key.ToLower()];
            }
        }
        private void set(string key, string value)
        {
            lock (Guarder)
            {
                if (_dictionary.ContainsKey(key.ToLower()))
                    throw new Exception($"{key} is already defined.");
                _dictionary.Add(key.ToLower(), value);
            }
        }

        public string this[string key]
        {
            get { return this.get(key); }
            private set { this.set(key, value); }
        }

        public bool Is(string key)
        {
            lock (Guarder)
            {
                if (!_dictionary.ContainsKey(key)) return false;
                bool res = false;
                return bool.TryParse(_dictionary[key], out res) || res;
            }
        }

        public bool IsIncluded(string lib) => includes.Contains(lib);
        public string[] getIncludes() => includes.ToArray();
        public string[] getKeys() => _dictionary.Keys.ToArray();
        
        public static FlameXConfig Parse(string siu)
        {
            siu = siu.Replace("\r", "");
            FlameXConfig x = new FlameXConfig();
            while (CommentRex.IsMatch(siu))
                siu = siu.Replace(CommentRex.Match(siu).Groups[1].Value, "");
            int lineNum = 0;
            foreach (var s in siu.Split('\n'))
            {
                lineNum++;
                if (string.IsNullOrWhiteSpace(s))
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
                if(s.StartsWith("/*", StringComparison.Ordinal))
                    continue;
                if (s.StartsWith("//", StringComparison.Ordinal))
                    continue;
                Screen.WriteLine($"[{RCL.Wrap("FX-Config", Color.DarkCyan)}][{RCL.Wrap("ERROR", Color.Red)}]: Syntax ERROR, line: ['{s}':{lineNum}]. Ignored!");
            }
            return x;
        }
    }
}
