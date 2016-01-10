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
 * @unity FileIdentifier 
 */
public class FileIdentifier : UnityStruct {

    // Path to the asset file? Unused in1 asset Format <= 5.
    private string assetPath;
    
    // Globally unique identifier of the referred asset. Unity displays these
    // as simple 16 byte hex strings with each byte swapped, but they can also
    // be represented according to the Guid standard.
    private  UnityGUID guid = new UnityGUID();
    
    // Path to the asset file. Only used if "type" is 0.
    private string filePath;
    
    // Reference type. Possible values are probably 0 to 3.
    private int type;
    
    private AssetFile asset;
    
    public FileIdentifier(VersionInfo versionInfo) {
        base(versionInfo);
    }
    
        public override  void read(DataReader in1)  {
        if (versionInfo.getAssetVersion() > 5) {
            assetPath = in1.readstringNull();
        }
        
        guid.read(in1);
        type = in1.readInt();
        filePath = in1.readstringNull();
    }

        public override  void write(DataWriter out1)  {
        if (versionInfo.getAssetVersion() > 5) {
            out1.writestringNull(assetPath);
        }
        
        guid.write(out1);
        out1.writeInt(type);
        out1.writestringNull(filePath);
    }

    public Guid getGUID() {
        return guid.getUUID();
    }

    public void setGUID(Guid guid) {
        this.guid.setUUID(guid);
    }

    public string getFilePath() {
        return filePath;
    }

    public void setFilePath(string filePath) {
        this.filePath = filePath;
    }

    public string getAssetPath() {
        return assetPath;
    }

    public void setAssetPath(string assetPath) {
        this.assetPath = assetPath;
    }

    public int getType() {
        return type;
    }

    public void setType(int type) {
        this.type = type;
    }

    public AssetFile getAssetFile() {
        return asset;
    }

    void setAssetFile(AssetFile asset) {
        this.asset = asset;
    }
}
