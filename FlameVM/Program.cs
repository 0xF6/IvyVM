using System;
using System.IO;
using System.Threading;
using Flame.API.Core;
using FlameVM.Core;

namespace FlameVM
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.Title = "Flame VM - Context: [TypeScript]";

            FlameXConfig conf = FlameXConfig.Parse(File.ReadAllText("config\\vm.fx"));

            Thread.Sleep(2000);

            EngineVM VM = new EngineVM(conf);

            VM.SetEngine(libxFile.LoadFromFile("lib\\tsc.lib"));
            VM.BindMain("main.ts");
            VM.Start();
            Console.Read();
            return 0; 
        }
    }
}
