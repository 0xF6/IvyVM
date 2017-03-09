let ts = require('typescript');
let TCSResult = '';
let compilerOptions = { module: ts.ModuleKind.CommonJS };


return function(data, callback) 
{
    TCSResult = ts.transpile(data, compilerOptions, undefined, undefined,'FlameScript');
    callback(null, TCSResult);
}