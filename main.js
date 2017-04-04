"use strict";
import colors from "colors";
import inventoryChangesRegistrator from "./iventory-changes-registrator.js";


export class tradesChecker
{
    public start()
    {
        if(!this.once)
        {
            this.once = true;
            console.log(this);
            //setInterval(this.check, this.checkTradesInterval);
        }
    }
    public check()
    {
        this.logined = true; //временно. потом убрать
        if(this.logined)
        {
            //setInterval проверки трейдов с записями изменений в базу
            console.log(colors.green('[' + this.logfile + '] Checking trades...'));
        }
    }
}
