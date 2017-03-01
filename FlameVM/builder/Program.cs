using System.IO;
using System.Threading;

namespace builder
{
    using System;
    using RC.Framework;

    public class Program
    {
        public static int Main(string[] args)
        {
            Console.Title = "builder";
            try
            {
                Assembler.Asm();
            }
            catch (Exception e)
            {
                Assembler.Error($"{RCL.Wrap("!!!", ConsoleColor.Red)} CATCHING EXCEPTION WARIOUT {RCL.Wrap("!!!", ConsoleColor.Red)}");
                Assembler.Error(e.ToRCLString());
                Assembler.Error($"{RCL.Wrap("!!!", ConsoleColor.Red)} STOP {RCL.Wrap("!!!", ConsoleColor.Red)}");
                Console.ReadKey();
                return -1; 
            }
            return 1;
        }

        
    }

    public static class Assembler
    {
        public static void Asm()
        {
            var starttime = DateTime.Now;
            Log("start build...");
            if (!Directory.Exists("..\\FlameVM\\bin\\Debug"))
                throw new DirectoryNotFoundException("..\\FlameVM\\bin\\Debug is not found!");
            Log("detected FlameVM binaries..");
            if (Directory.Exists("..\\bin"))
            {
                DirectoryInfo di = new DirectoryInfo("..\\bin");
                foreach (FileInfo file in di.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }
            Directory.CreateDirectory("..\\bin");
            Log("clear bin folder..");

            var files1 = Directory.GetFiles("..\\FlameVM\\bin\\Debug", "*.dll");
            var files2 = Directory.GetFiles("..\\FlameVM\\bin\\Debug", "*.exe");

            foreach (var s in files1)
                File.Copy(s, $"..\\bin\\{Path.GetFileName(s)}");
            foreach (var s in files2)
                File.Copy(s, $"..\\bin\\{Path.GetFileName(s)}");

            Log("move binaries..");
            File.Move("..\\bin\\FlameVM.exe", "..\\bin\\flame.vm");
            Log($"build complete! time: {(DateTime.Now - starttime).TotalSeconds} sec.");
            Thread.Sleep(1000);
        }

        public static void Log(string s)
        {
            Terminal.WriteLine($"[{RCL.Wrap("asm", ConsoleColor.DarkMagenta)}][{RCL.Wrap("LOG", ConsoleColor.DarkGray)}]: {s}");
        }
        public static void Error(string s)
        {
            Terminal.WriteLine($"[{RCL.Wrap("asm", ConsoleColor.DarkMagenta)}][{RCL.Wrap("ERR", ConsoleColor.DarkRed)}]: {s}");
        }
        public static void Waring(string s)
        {
            Terminal.WriteLine($"[{RCL.Wrap("asm", ConsoleColor.DarkMagenta)}][{RCL.Wrap("WAR", ConsoleColor.DarkYellow)}]: {s}");
        }
    }
}
