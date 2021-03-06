﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jint.Runtime;
using RC.Framework;

namespace FlameVM.Core
{
    public static class Tools
    {
        public static List<string> types = new List<string>()
        {
            nameof(String),
            nameof(Int16),
            nameof(Int32),
            nameof(Int64),
            nameof(IntPtr),
            nameof(UInt16),
            nameof(UInt32),
            nameof(UInt64),
            nameof(Boolean),
            nameof(Exception),
            nameof(DirectoryNotFoundException),
            "List",
            "Dictionary",

            nameof(Program)
        };
        public static List<string> typesEnum = new List<string>()
        {
            nameof(ConsoleColor),
            "Main"
        };
        public static List<string> typesNameSpace = new List<string>()
        {
            "System",
            "RC",
            "Framework",
            "Text",
            "Threading",
            "Linq",
            "Collections",
            "Generic",
            "IO",
            "builder"
        };

        public static string ToRCLString(this Exception e)
        {
            string ex = e.ToString();
            ex = types.Aggregate(ex, (current, s) => current.Replace(s, RCL.Wrap(s, ConsoleColor.DarkGreen)));
            ex = typesEnum.Aggregate(ex, (current, s) => current.Replace(s, RCL.Wrap(s, ConsoleColor.DarkYellow)));
            ex = typesNameSpace.Aggregate(ex, (current, s) => current.Replace(s, RCL.Wrap(s, ConsoleColor.Gray)));
            return ex;
        }

        public static void Print(this JavaScriptException e)
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
    }
}