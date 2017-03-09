"use strict";
exports.__esModule = true;
var Dummy;
(function (Dummy) {
    var TestClass = (function () {
        function TestClass(i) {
        }
        TestClass.testMethod = function () {
        };
        TestClass.testMethod2 = function () {
            return 1 + 1;
        };
        TestClass.testMethod3 = function () {
            return true;
        };
        return TestClass;
    }());
    Dummy.TestClass = TestClass;
})(Dummy || (Dummy = {}));
