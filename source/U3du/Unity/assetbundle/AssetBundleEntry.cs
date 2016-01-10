/*
 ** 2014 September 29
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
public abstract class AssetBundleEntry {
    
    public abstract string getName();
    
    public abstract long getSize();

    public abstract InputStream getInputStream() ;
    
    public bool isLibrary() {
        string ext = FilenameUtils.getExtension(getName());
        return ext.Equals("dll") || ext.Equals("mdb");
    }
}
