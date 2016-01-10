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
public class AssetBundleExternalEntry : AssetBundleEntry {
    
    private  string name;
    private  Path file;

    public AssetBundleExternalEntry(string name, Path file) {
        this.name = name;
        this.file = file;
    }

        public override  string getName() {
        return name;
    }

        public override  long getSize() {
        try {
            return Files.size(file);
        } catch (IOException ex) {
            return 0;
        }
    }

        public override  InputStream getInputStream()  {
        return Files.newInputStream(file);
    }
    
}
