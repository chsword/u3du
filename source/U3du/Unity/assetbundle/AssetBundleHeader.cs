/*
 ** 2013 June 16
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */
/**
 * Structure for Unity asset bundles.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 * @unity UnityWebStreamHeader
 */
public class AssetBundleHeader : Struct {
    
    public static  string SIGNATURE_WEB = "UnityWeb";
    public static  string SIGNATURE_RAW = "UnityRaw";
    
    // UnityWeb or UnityRaw
    private string signature;
    
    // file version
    // 3 in1 Unity 3.5 and 4
    // 2 in1 Unity 2.6 to 3.4
    // 1 in1 Unity 1 to 2.5
    private int streamVersion;
    
    // player version string
    // 2.x.x for Unity 2
    // 3.x.x for Unity 3/4
    private UnityVersion unityVersion;
    
    // engine version string
    private UnityVersion unityRevision;
    
    // minimum number of bytes to read for streamed bundles,
    // equal to completeFileSize for normal bundles
    private long minimumStreamedBytes;
    
    // offset to the bundle data or size of the bundle header
    private int headerSize;
    
    // equal to 1 if it's a streamed bundle, number of levelX + mainData assets
    // otherwise
    private int numberOfLevelsToDownload;
    
    // list of compressed and uncompressed offsets
    private List<Pair<Long, Long>> levelByteEnd = new ArrayList<>();
    
    // equal to file size, sometimes equal to uncompressed data size without the header
    private long completeFileSize;
    
    // offset to the first asset file within the data area? Equals compressed
    // file size if completeFileSize contains the uncompressed data size
    private long dataHeaderSize;
    
        public override  void read(DataReader in1)  {
        signature = in1.readstringNull();
        streamVersion = in1.readInt();
        unityVersion = new UnityVersion(in1.readstringNull());
        unityRevision = new UnityVersion(in1.readstringNull());
        minimumStreamedBytes = in1.readUnsignedInt();
        headerSize = in1.readInt();
        
        numberOfLevelsToDownload = in1.readInt();
        int numberOfLevels = in1.readInt();
        
        assert numberOfLevelsToDownload == numberOfLevels || numberOfLevelsToDownload == 1;
        
        for (int i = 0; i < numberOfLevels; i++) {
            levelByteEnd.add(new ImmutablePair(in1.readUnsignedInt(), in1.readUnsignedInt()));
        }
        
        if (streamVersion >= 2) {
            completeFileSize = in1.readUnsignedInt();
            assert minimumStreamedBytes <= completeFileSize;
        }
        
        if (streamVersion >= 3) {
            dataHeaderSize = in1.readUnsignedInt();
        }
        
        in1.readByte();
    }

        public override  void write(DataWriter out1)  {
        out1.writestringNull(signature);
        out1.writeInt(streamVersion);
        out1.writestringNull(unityVersion.ToString());
        out1.writestringNull(unityRevision.ToString());
        out1.writeUnsignedInt(minimumStreamedBytes);
        out1.writeInt(headerSize);
        
        out1.writeInt(numberOfLevelsToDownload);
        out1.writeInt(levelByteEnd.size());
        
        for (Pair<Long, Long> offset : levelByteEnd) {
            out1.writeUnsignedInt(offset.getLeft());
            out1.writeUnsignedInt(offset.getRight());
        }
        
        if (streamVersion >= 2) {
            out1.writeUnsignedInt(completeFileSize);
        }
        
        if (streamVersion >= 3) {
            out1.writeUnsignedInt(dataHeaderSize);
        }
        
        out1.writeUnsignedByte(0);
    }
    
    public bool hasValidSignature() {
        return signature.Equals(SIGNATURE_WEB) || signature.Equals(SIGNATURE_RAW);
    }
    
    public void setCompressed(bool compressed) {
        signature = compressed ? SIGNATURE_WEB : SIGNATURE_RAW;
    }
    
    public bool isCompressed() {
        return signature.Equals(SIGNATURE_WEB);
    }

    public string getSignature() {
        return signature;
    }

    public void setSignature(string signature) {
        this.signature = signature;
    }

    public int getStreamVersion() {
        return streamVersion;
    }

    public void setStreamVersion(int Format) {
        this.streamVersion = Format;
    }

    public UnityVersion getUnityVersion() {
        return unityVersion;
    }

    public void setUnityVersion(UnityVersion version) {
        this.unityVersion = version;
    }

    public UnityVersion getUnityRevision() {
        return unityRevision;
    }

    public void setUnityRevision(UnityVersion revision) {
        this.unityRevision = revision;
    }

    public int getHeaderSize() {
        return headerSize;
    }

    public void setHeaderSize(int dataOffset) {
        this.headerSize = dataOffset;
    }
    
    public List<Pair<Long, Long>> getLevelByteEnd() {
        return levelByteEnd;
    }
    
    public int getNumberOfLevels() {
        return levelByteEnd.size();
    }

    public int getNumberOfLevelsToDownload() {
        return numberOfLevelsToDownload;
    }

    public void setNumberOfLevelsToDownload(int numberOfLevelsToDownload) {
        this.numberOfLevelsToDownload = numberOfLevelsToDownload;
    }

    public long getCompleteFileSize() {
        return completeFileSize;
    }

    public void setCompleteFileSize(long completeFileSize) {
        this.completeFileSize = completeFileSize;
    }

    public long getMinimumStreamedBytes() {
        return minimumStreamedBytes;
    }

    public void setMinimumStreamedBytes(long minimumStreamedBytes) {
        this.minimumStreamedBytes = minimumStreamedBytes;
    }

    public long getDataHeaderSize() {
        return dataHeaderSize;
    }

    public void setDataHeaderSize(long dataHeaderSize) {
        this.dataHeaderSize = dataHeaderSize;
    }
}
