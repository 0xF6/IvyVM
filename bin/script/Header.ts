// -==================================-
// Created by Yuuki Wesp on 09.03.2017.
// -==================================-

declare function VMLog(s: string): void;
declare function VMError(s: string): void;
declare function VMWarning(s: string): void;
declare function VMGetVersion(): string;
declare function VMFlameUnhandledException(errorCode: number): void;
declare function VMFlameInternalFatalLog(s: string): void;
namespace Flame
{
    export class RuntimeException extends Error
    {
        private _errorCode: number;
        constructor(errorCode: number)
        {
            super();
            this._errorCode = errorCode;
        }

        public getErrorCode(): number
        {
            return this._errorCode;
        }
    }
}
declare class Engine
{
    public static Log(s: string): void;
    public static Error(s: string): void;
    public static Warning(s: string): void;
}
declare interface flameVM
{
    methods: Array<any>;
    Engine: Engine;
}
declare interface exports
{
    __esModule: boolean;
    flameVM: flameVM;
}


export abstract class FlameBlock
{
    protected internalInit()
    {
        try
        {
            this.Init();
        }
        catch(e)
        {
            VMFlameInternalFatalLog(e.toString());
            if(typeof e != typeof Flame.RuntimeException)
                VMFlameUnhandledException(0x0);
            else
                VMFlameUnhandledException((<Flame.RuntimeException>e).getErrorCode());
        }
    }
    protected internalStart()
    {
        try
        {
            this.Start();
        }
        catch(e)
        {
            VMFlameInternalFatalLog(e.toString());
            if(typeof e != typeof Flame.RuntimeException)
            VMFlameUnhandledException(0x1);
            else
            VMFlameUnhandledException((<Flame.RuntimeException>e).getErrorCode());
        }
    }
    protected internalEnd()
    {
        try
        {
            this.End();
        }
        catch(e)
        {
            VMFlameInternalFatalLog(e.toString());
            if(typeof e != typeof Flame.RuntimeException)
                VMFlameUnhandledException(0x2);
            else
                VMFlameUnhandledException((<Flame.RuntimeException>e).getErrorCode());
        }
    }

    abstract Init(): void;
    abstract Start(): void;
    abstract End(): void;
}