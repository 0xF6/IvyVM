using System;
using System.Threading;
using Noesis.Javascript;

namespace FlameVM
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.Title = "Flame VM - Context: [TypeScript]";

            Thread.Sleep(2000);


            using (var context = new JavascriptContext())
            {
                // Some trivial typescript:
                var typescriptSource = "window.alert('hello world!');";
                context.SetParameter("typescriptSource", typescriptSource);
                context.SetParameter("result", "");

                // Build some js to execute:
                string script = tscJs + @"
result = TypeScript.compile(""typescriptSource"")";

                // Execute the js
                context.Run(script);

                // Retrieve the result (which should be the compiled JS)
                var js = context.GetParameter("result");
            }

            return 0; 
        }
    }
}
