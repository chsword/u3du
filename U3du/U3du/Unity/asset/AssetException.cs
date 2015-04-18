/*
 ** 2013 July 12
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

using System.IO;

public class AssetException : IOException {

    /**
     * Creates a new instance of
     * <code>AssetException</code> without detail message.
     */
    public AssetException() {
    }

    /**
     * Constructs an instance of
     * <code>AssetException</code> with the specified detail message.
     *
     * @param msg the detail message.
     */
    public AssetException(string msg)
        : base(msg)
    {
    
    }
}
