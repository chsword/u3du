/*
 ** 2014 September 21
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

public class RuntimeTypeException : Exception {

    /**
     * Creates a new instance of <code>SerializationException</code> without
     * detail message.
     */
    public RuntimeTypeException() {
    }

    /**
     * Constructs an instance of <code>SerializationException</code> with the
     * specified detail message.
     *
     * @param msg the detail message.
     */
    public RuntimeTypeException(string msg)
        : base(msg)
    {
        ;
    }

    //public RuntimeTypeException(Throwable cause)
    //    : base(msg)
    //{
 
    //}

    //public RuntimeTypeException(string message, Throwable cause)
    //    :        base(message, cause);
    //{
 
    //}
}
