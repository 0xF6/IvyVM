// -==================================-
// Created by Yuuki Wesp on 06.03.2017.
// -==================================-

declare function VMLog(s: string): void;
declare function VMError(s: string): void;
declare function VMWarning(s: string): void;
declare function VMGetVersion(): string;

namespace Flame
{
    export class Engine
    {
        public static Log(s: string): void { VMLog(s); }
        public static Error(s: string): void { VMError(s); }
        public static Warning(s: string): void { VMWarning(s); }
    }
}