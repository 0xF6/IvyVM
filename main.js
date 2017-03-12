/**
 * Created by Yuuki Wesp on 01.03.2017.
 */
'use strict';
let asm = require("./_assembler/asm-core").ast;


asm.Welcome();
asm.runVM();


let ts = require('typescript');
let TCSResult = '';
let compilerOptions = { module: ts.ModuleKind.None, removeComments: true };


TCSResult = ts.transpile(data, compilerOptions, undefined, undefined,'FlameScript');