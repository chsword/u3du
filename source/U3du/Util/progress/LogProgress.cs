/*
 ** 2014 December 02
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */
 

/**
 *
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System;

public class LogProgress : Progress {
    
    private static Logger logger;
    
    public LogProgress(Logger logger) {
       
    }

 
    public void setLabel(string label) {
         
    }

 
    public void setLimit(long limit) {
    }

 
    public void update(long current) {
    }
 
    public bool isCanceled() {
        return false;
    }
    
}

public class Logger
{
    public void Log(object info, string processing, string label)
    {
        throw new NotImplementedException();
    }
}
