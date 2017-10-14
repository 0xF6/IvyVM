

namespace FlameVM.Core.Wraps
{
    using System;
    using RC.Framework;

    public class VMCore
    {
        private static readonly string Header = $"[{RCL.Wrap("VMCore", ConsoleColor.DarkMagenta)}]";

        public static string VMGetVersion() => "Node 7.1, V8 ES5, .NET 4.6.2, RC.Core 9.2, Flame.API 1.2, FlameVM 0.4";

        public static void VMLog   (string s) => Terminal.WriteLine($"{Header}[{RCL.Wrap("LOG", ConsoleColor.DarkGray)  }]: {s}");
        public static void VMError (string s) => Terminal.WriteLine($"{Header}[{RCL.Wrap("ERR", ConsoleColor.DarkRed)   }]: {s}");
        public static void VMWaring(string s) => Terminal.WriteLine($"{Header}[{RCL.Wrap("WAR", ConsoleColor.DarkYellow)}]: {s}");
        public static void VMVMInfo(string s) => Terminal.WriteLine($"{Header}[{RCL.Wrap("INF", ConsoleColor.Gray)      }]: {s}");

        public static void VMFlameInternalFatalLog(string s)
        {
            Terminal.WriteLine($"{Header}[{RCL.Wrap("INF", ConsoleColor.Gray)}]: {s}");
        }

        public static void VMFlameUnhandledException(int errorCode)
        {
            
        }

        public static void Bind(EngineVM engine)
        {
            _bindAction(engine);
            _bindFunction(engine);
        }

        public static void Initialization(EngineVM engine)
        {
            engine.jsEngine.Execute(@"
            var Flame;
            (function (Flame) {
                var Engine = (function () 
                {
                    function Engine() {}
                    Engine.Log = function (s) { VMLog(s); };
                    Engine.Error = function (s) { VMError(s); };
                    Engine.Warning = function (s) { VMWarning(s); };
                    Engine.Info = function (s) { VMInfo(s); };
                    return Engine;
                }());
                Flame.Engine = Engine;
            })(Flame || (Flame = {}));");
            engine.jsEngine.Execute("var exports = {}; exports.__esModule = false;");
            engine.jsEngine.Execute("Flame.Engine.Info('[VM] Initialization is complete.');");
        }

        private static void _bindAction(EngineVM vm)
        {
            vm.BindContext<Action<string>>(VMLog);
            vm.BindContext<Action<string>>(VMError);
            vm.BindContext<Action<string>>(VMWaring);
            vm.BindContext<Action<string>>(VMVMInfo);
        }
        private static void _bindFunction(EngineVM vm)
        {
            vm.BindContext<Func<string>>(VMGetVersion);
        }



    }
}