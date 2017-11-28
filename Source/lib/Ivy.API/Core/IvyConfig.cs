namespace Ivy.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Linq;
    using NLog;

    public class IvyConfig
    {
        private static readonly Logger logger = LogManager.GetLogger("IvyConfig");


        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();
        private readonly List<string> includes = new List<string>();

        private static readonly Regex IncludeRex = new Regex(@"^#include <([A-Za-z]{1,}\.[a-z]{1,4})>\(\);$");
        private static readonly Regex CommentRex = new Regex(@"^(\/\*.{1,}?\*\/).+$", RegexOptions.Singleline);
        private static readonly Regex HarmonyRex = new Regex(@"^#harmony (true|false|[0-1]{1});$");
        private static readonly Regex VarsRex = new Regex(@"^<""([A-Za-z.]{1,})"">\(\""([A-Za-z0-9.><_\\\/:{}\?\=\-\*\&\^\%\$\#\@\!\`\~\+]{1,})\""\);$");

        private IvyConfig() { }
        /// <summary>
        /// Sync Guarder
        /// </summary>
        private readonly object Guarder = new object();
        /// <summary>
        /// Get value from key
        /// </summary>
        public string get(string key)
        {
            key = key.ToLowerInvariant();
            lock (Guarder)
            {
                if (!_dictionary.ContainsKey(key))
                    throw new Exception($"{key} is not defined.");
                return _dictionary[key];
            }
        }
        /// <summary>
        /// Get value from key
        /// </summary>
        public string get(string key, string defaultValue)
        {
            key = key.ToLowerInvariant();
            lock (Guarder)
            {
                return !_dictionary.ContainsKey(key) ? defaultValue : _dictionary[key];
            }
        }
        /// <summary>
        /// Set value from key
        /// </summary>
        private void set(string key, string value)
        {
            key = key.ToLowerInvariant();
            lock (Guarder)
            {
                if (_dictionary.ContainsKey(key))
                    throw new Exception($"{key} is already defined.");
                _dictionary.Add(key, value);
            }
        }
        /// <summary>
        /// Get value from key
        /// </summary>
        public string this[string key]
        {
            get => this.get(key);
            private set => this.set(key, value);
        }
        /// <summary>
        /// Check whether this key in the configuration, if there is something we consider bool
        /// </summary>
        public bool Is(string key)
        {
            lock (Guarder)
            {
                if (!_dictionary.ContainsKey(key)) return false;
                return bool.TryParse(_dictionary[key], out var res) || res;
            }
        }
        /// <summary>
        /// Check whether this link on the library configuration
        /// </summary>
        public bool IsIncluded(string lib) => includes.Contains(lib);
        public string[] getIncludes() => includes.ToArray();
        public string[] getKeys() => _dictionary.Keys.ToArray();

        public static IvyConfig LoadFrom(string path)
        {
            return Parse(File.ReadAllText(path));
        }

        /// <summary>
        /// Parse file
        /// </summary>
        public static IvyConfig Parse(string content)
        {
            content = content.Replace("\r", "");
            var x = new IvyConfig();
            while (CommentRex.IsMatch(content))
                content = content.Replace(CommentRex.Match(content).Groups[1].Value, "");
            var lineNum = 0;
            foreach (var s in content.Split('\n'))
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
                if (s.StartsWith(" ", StringComparison.Ordinal))
                    continue;
                if (s.StartsWith("*", StringComparison.Ordinal))
                    continue;
                if (s.StartsWith("\t", StringComparison.Ordinal))
                    continue;
                if (s.StartsWith("*/", StringComparison.Ordinal))
                    continue;
                logger.Error($"Syntax ERROR, line: ['{s}':{lineNum}]. Ignored!");
            }
            return x;
        }
    }
}
