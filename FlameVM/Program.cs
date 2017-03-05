using System;
using System.Threading;
using FlameVM.Core;
using Noesis.Javascript;

namespace FlameVM
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.Title = "Flame VM - Context: [TypeScript]";

            Thread.Sleep(2000);

            EngineVM VM = new EngineVM();

            VM.SetEngine(libxFile.LoadFromFile("lib\\tsc.lib"));
            VM.BindMain("main.ts");
            VM.Start();
            Console.Read();
            return 0; 
        }
    }
}
