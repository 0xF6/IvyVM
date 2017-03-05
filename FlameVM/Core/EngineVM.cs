using System;
using System.Collections.Generic;
using System.IO;
using Jint;
using Jint.Runtime;
using Noesis.Javascript;
using RC.Framework;

namespace FlameVM.Core
{
    public class EngineVM
    {
        private libxFile compilePacket;
        private string mainTS;
        private readonly Dictionary<string, object> contextDictionary = new Dictionary<string, object>();
        private Engine jsEngine;
        private string compiledJS;
        public EngineVM()
        {
            jsEngine = new Engine();
            BindContext<Action<string>>("VMLog", Log);
            BindContext<Action<string>>("VMWarn", Waring);
            BindContext<Action<string>>("VMError", Error);
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
                // Some trivial typescript:
                var typescriptSource = "window.alert('hello world!');";
                context.SetParameter("typescriptSource", typescriptSource);
                context.SetParameter("result", "");

                // Build some js to execute:
                string script = File.ReadAllText("lib\\env.lib");

                // Execute the js
                context.Run(script);

                // Retrieve the result (which should be the compiled JS)
                var js = context.GetParameter("result");
            }

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