/*
 ** 2014 December 03
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
public class AssetBundleInternalEntry : AssetBundleEntry {
    
    private  AssetBundleReader reader;
    private  AssetBundleEntryInfo info;

    public AssetBundleInternalEntry(AssetBundleReader reader, AssetBundleEntryInfo info) {
        this.reader = reader;
        this.info = info;
    }
    
        public override  string getName() {
        return info.getName();
    }
    
        public override  long getSize() {
        return info.getSize();
    }

        public override  InputStream getInputStream()  {
        return reader.getInputStreamForEntry(info);
    }

        public override  string ToString() {
        return getName();
    }
}
