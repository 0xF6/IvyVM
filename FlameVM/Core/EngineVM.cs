using System;
using System.Collections.Generic;
using System.IO;
using Flame.API.Core;
#pragma warning disable 1998
using System.Threading.Tasks;
using EdgeJs;
using Jint;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Noesis.Javascript;
using RC.Framework;

namespace FlameVM.Core
{
    public class EngineVM
    {
        private static string HeaderNodeJS = @"
            let Log = exports.flameVM.Engine.Log;
            let Error = exports.flameVM.Engine.Error;
            let Warning = exports.flameVM.Engine.Warning;";


        private libxFile compilePacket;
        private string mainTS;
        private readonly Dictionary<string, object> contextDictionary = new Dictionary<string, object>();
        private readonly Engine jsEngine;
        public EngineVM(FlameXConfig config)
        {
            jsEngine = new Engine(cfg =>
            {
                if (config.Is("harmony"))
                    cfg.AllowClr();
            });



            Edge.Func(@"
            var Flame;
            (function (Flame) {
                var Engine = (function () 
                {
                    function Engine() {}
                    Engine.Log = function (s) { exports.flameVM.VMLog(s); };
                    Engine.Error = function (s) { exports.flameVM.VMError(s); };
                    Engine.Warning = function (s) { exports.flameVM.VMWarning(s); };
                    return Engine;
                }());
                Flame.Engine = Engine;
            })(Flame || (Flame = {}));
            return function(data, callback) 
            {
                exports.flameVM = {};
                exports.flameVM.methods = [];
                exports.flameVM.Engine = Flame.Engine;
                callback(null, null);
            }  
            ").Invoke(null).Wait();


            BindContext<Action<string>>("VMLog", Log);
            BindContext<Action<string>>("VMWarn", Waring);
            BindContext<Action<string>>("VMError", Error);

            BindContext<Func<string>>(nameof(VMGetVersion), VMGetVersion);


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
            jsEngine.Execute("var exports = {}; exports.__esModule = false;");
            jsEngine.Execute("Flame.Engine.Log('[VM] Initialization is complete.');");
        }

        public void BindContext<T>(T t)
        {
            this.BindContext(nameof(T), t);
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

            if (typeof(T) == typeof(Action<string>))  this.BindDelegate((Action<string>)(object)obj, key);
            if (typeof(T) == typeof(Action<int>))     this.BindDelegate((Action<int>)   (object)obj, key);
            if (typeof(T) == typeof(Action<bool>))    this.BindDelegate((Action<bool>)  (object)obj, key);
            if (typeof(T) == typeof(Action<object>))  this.BindDelegate((Action<object>)(object)obj, key);
            if (typeof(T) == typeof(Action<byte>))    this.BindDelegate((Action<byte>)  (object)obj, key);
        }

        private void BindDelegate<T>(Action<T> a, string name)
        {
            var wrapFucn = (Func<object, Task<object>>)(async (s) =>
            {
                a((T)s);
                return null;
            });

            Edge.Func($"return function(data, callback)" +
            "{" +
           $"    exports.flameVM.{name} = data;" +
           $"    callback(null, null);" +
            "}").Invoke(wrapFucn).Wait();
        }

        public static Func<object, Task<object>> Execute(string code) { return Edge.Func(HeaderNodeJS + code); }
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

            var sq = (Func<object, Task<object>>)(async (i) => {
                Terminal.WriteLine($"[{RCL.Wrap("Core", ConsoleColor.DarkMagenta)}]: result:{i}!");
                return null;
            });

            var qwe0 = EngineVM.Execute(
            @"return function(data, callback) 
            {
                Log('sqe0 execute!');
                exports.nure = 1;

                data(exports.nure++, true);
                callback(null, null);
            }  
            ");
            var qwe1 = EngineVM.Execute(@"
            return function(data, callback) 
            {
                Log('sqe1 execute!');
                data(exports.nure++, true);
                callback(null, null);
            }  
            ");

            qwe0(sq).Wait();
            qwe1(sq).Wait();
            qwe1(sq).Wait();
            qwe1(sq).Wait();
            qwe1(sq).Wait();

            var func = EngineVM.Execute(@"
            let ts = require('typescript');
            let TCSResult = '';
            let compilerOptions = { module: ts.ModuleKind.None, removeComments: true };


            return function(data, callback) 
            {
                TCSResult = ts.transpile(data, compilerOptions, undefined, undefined,'FlameScript');
                callback(null, TCSResult);
            }
            ");
            string tsc = (string)func(File.ReadAllText(mainTS)).Result;

            


            //try
            //{
            //    jsEngine.Execute(tsc);
            //    jsEngine.Execute("YRes = Dummy.TestClass.testMethod2();");
            //    //jsEngine.Execute("TCSResult = TypeScript.compile(TSSource, null, function(e) { VMError(e); });");
            //}
            //catch (JavaScriptException e)
            //{
            //    e.Print();
            //}
        }


        public static string VMGetVersion() => "Node 7.1, V8 ES5, .NET 4.6.2, RC.Core 9.2, Flame.API 1.2, FlameVM 0.4";
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