using System;
using System.Collections.Generic;
using System.IO;
using Jint;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Noesis.Javascript;
using RC.Framework;

namespace FlameVM.Core
{
    public class EngineVM
    {
        private libxFile compilePacket;
        private string mainTS;
        private readonly Dictionary<string, object> contextDictionary = new Dictionary<string, object>();
        private readonly Engine jsEngine;
        private string compiledJS;
        public EngineVM()
        {
            jsEngine = new Engine(cfg => cfg.AllowClr());




            BindContext<Action<string>>("VMLog", Log);
            BindContext<Action<string>>("VMWarn", Waring);
            BindContext<Action<string>>("VMError", Error);


            jsEngine.Execute(@"
            var Flame;
            (function (Flame) {
                var Engine = (function () 
                {
                    function Engine() {}
                    Engine.Log = function (s) { VMLog(s); };
                    Engine.Error = function (s) { VMError(s); };
                    Engine.Warning = function (s) { VMWarning(s); };
                    return Engine;
                }());
                Flame.Engine = Engine;
            })(Flame || (Flame = {}));");

            jsEngine.Execute("Flame.Engine.Log('VM Started')");
        }
        public void BindContext<T>(string key, T obj)
        {
            if (contextDictionary.ContainsKey(key))
            {
                Waring($"[{RCL.Wrap("Context", ConsoleColor.Cyan)}] '{key}' is already defined.");
                return;
            }
            contextDictionary.Add(key, obj);
            jsEngine.SetValue(key, obj);
            jsEngine.SetValue("Tools", TypeReference.CreateTypeReference(jsEngine, typeof(Tools)));
        }

        public void SetEngine(libxFile pack) { this.compilePacket = pack; }
        public void BindMain(string path)
        {
            if (!File.Exists(path))
            {
                Error($"'{path}' file is not found!");
                return;
            }
            this.mainTS = path;
        }
        public void Start()
        {
            jsEngine.SetValue("TCSResult", "");
            jsEngine.SetValue("TSSource", File.ReadAllText(mainTS));
            
            using (var context = new JavascriptContext())
            {
                try
                {
                    jsEngine.Execute(File.ReadAllText("lib\\env.lib"));
                    jsEngine.Execute(compilePacket.ToString()); // TSCompile
                    jsEngine.Execute("TCSResult = TypeScript.compile(TSSource, null, function(e) { VMError(e); });");
                }
                catch (JavaScriptException e)
                {
                    Terminal.WriteLine($"+==============================================+");
                    Terminal.WriteLine($"+=|           {RCL.Wrap("JavaScript Exception", ConsoleColor.DarkRed)}");
                    Terminal.WriteLine($"+=| Column   :{e.Column}");
                    Terminal.WriteLine($"+=| Location :{e.Location}");
                    Terminal.WriteLine($"+=| Line     :{e.LineNumber}");
                    Terminal.WriteLine($"+=| Error    :{e.Error}");
                    Terminal.WriteLine($"+=| Exception:{e.Message}");
                    Terminal.WriteLine($"+==============================================+");
                }
                compiledJS = jsEngine.GetValue("TCSResult").AsString();
            }
        }

        public static void Log(string s)
        {
            Terminal.WriteLine($"[{RCL.Wrap("VM", ConsoleColor.DarkMagenta)}][{RCL.Wrap("LOG", ConsoleColor.DarkGray)}]: {s}");
        }
        public static void Error(string s)
        {
            Terminal.WriteLine($"[{RCL.Wrap("VM", ConsoleColor.DarkMagenta)}][{RCL.Wrap("ERR", ConsoleColor.DarkRed)}]: {s}");
        }
        public static void Waring(string s)
        {
            Terminal.WriteLine($"[{RCL.Wrap("VM", ConsoleColor.DarkMagenta)}][{RCL.Wrap("WAR", ConsoleColor.DarkYellow)}]: {s}");
        }
    }
}