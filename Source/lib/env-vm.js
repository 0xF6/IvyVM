// -==================================-
// Created by Yuuki Wesp on 06.03.2017.
// -==================================-
var Flame;
(function (Flame) {
    var Engine = (function () {
        function Engine() {
        }
        Engine.Log = function (s) { VMLog(s); };
        Engine.Error = function (s) { VMError(s); };
        Engine.Warning = function (s) { VMWarning(s); };
        return Engine;
    }());
    Flame.Engine = Engine;
})(Flame || (Flame = {}));
