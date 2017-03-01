/**
 * Created by Yuuki Wesp on 01.03.2017.
 */
'use strict';
let exec = require('child_process').execFile;
class Assembler
{
    log(s)
    {
        Assembler.internal_log(s, "log");
    }
    error(s)
    {
        Assembler.internal_log(s, "err");
    }
    warn(s)
    {
        Assembler.internal_log(s, "war");
    }
    nl(i)
    {
        if(!i) i = 1;
        for (let o = 0; o != i; o++)
        console.log("");
    }

    Welcome()
    {
        this.log("Start run script!");
        this.nl(3);
        this.log("-============================-");
        this.log("-|     Wrapper \x1b[36mVM\x1b[0m \x1b[31mFlame\x1b[0m     |-");
        this.log("-|    (C) \x1b[32mYuuki Wesp\x1b[0m 2017   |-");
        this.log("-============================-");
        this.nl(3);
    }

    static internal_log(s, type)
    {
        console.log(`[\x1b[31m${type}\x1b[0m]: ${s}.`);
    }

    runVM()
    {
        let that = this;
        exec('bin/flame.vm', function(err, data)
        {
            that.error(err);
            if(data)
                that.log(data.toString());
        });
    }
}

module.exports.ast = new Assembler();