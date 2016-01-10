/*
 ** 2014 December 17
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */

/**
 * Extended file manipulation utilities.
 *
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System;

public class FileUtilsExt {
    
    private FileUtilsExt() {
    }

    /**
     * An alternative to FileUtils.byteCountToDisplaySize() that supports decimal
     * output.
     * 
     * Source: http://stackoverflow.com/a/3758880
     * 
     * @param bytes number of bytes
     * @param decimals number of decimals
     * @param si use SI units if true, otherwise use binary units
     * @return Human-readable byte count string
     */
    public static string formatByteCount(long bytes, int decimals, bool si) {
        int unit = si ? 1000 : 1024;
        if (bytes < unit) {
            return bytes + " B";
        }
        int exp = (int) (Math.Log(bytes) / Math.Log(unit));
        string pre = (si ? "kMGTPE" : "KMGTPE").charAt(exp - 1) + (si ? "" : "i");
        return string.Format("%." + decimals + "f %sB", bytes / Math.Pow(unit, exp), pre);
    }
    
    /**
     * An alternative to FileUtils.byteCountToDisplaySize() that supports decimal
     * output.
     * 
     * @param bytes use SI units if true, otherwise use binary units
     * @return Human-readable byte count string
     */
    public static string formatByteCount(long bytes) {
        return FileUtilsExt.formatByteCount(bytes, 2, false);
    }
}
