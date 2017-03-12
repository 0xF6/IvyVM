// -==================================-
// Created by Yuuki Wesp on 09.03.2017.
// -==================================-
"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var Flame;
(function (Flame) {
    var EmmitException = (function (_super) {
        __extends(EmmitException, _super);
        function EmmitException(errorCode) {
            var _this = _super.call(this) || this;
            _this._errorCode = errorCode;
            return _this;
        }
        EmmitException.prototype.getErrorCode = function () {
            return this._errorCode;
        };
        return EmmitException;
    }(Error));
    Flame.EmmitException = EmmitException;
})(Flame || (Flame = {}));
var FlameBlock = (function () {
    function FlameBlock() {
    }
    FlameBlock.prototype.internalInit = function () {
        try {
            this.Init();
        }
        catch (e) {
            VMFlameInternalFatalLog(e.toString());
            if (typeof e != typeof Flame.EmmitException)
                VMFlameUnhandledException(0x0);
            else
                VMFlameUnhandledException(e.getErrorCode());
        }
    };
    FlameBlock.prototype.internalStart = function () {
        try {
            this.Start();
        }
        catch (e) {
            VMFlameInternalFatalLog(e.toString());
            if (typeof e != typeof Flame.EmmitException)
                VMFlameUnhandledException(0x1);
            else
                VMFlameUnhandledException(e.getErrorCode());
        }
    };
    FlameBlock.prototype.internalEnd = function () {
        try {
            this.End();
        }
        catch (e) {
            VMFlameInternalFatalLog(e.toString());
            if (typeof e != typeof Flame.EmmitException)
                VMFlameUnhandledException(0x2);
            else
                VMFlameUnhandledException(e.getErrorCode());
        }
    };
    return FlameBlock;
}());
exports.FlameBlock = FlameBlock;
