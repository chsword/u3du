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
 *
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 * @unity SerializedFileHeader
 */
public class AssetHeader : UnityStruct {
    
    // size of the structure data
    private long metadataSize;
    
    // size of the whole asset file
    private long fileSize;
    
    // offset to the serialized data
    private long dataOffset;
    
    // byte order of the serialized data?
    private byte endianness;
    
    // unused
    private  byte[] reserved = new byte[3];
    
    public AssetHeader(VersionInfo versionInfo) {
        base(versionInfo);
    }

        public override  void read(DataReader in1)  {
        metadataSize = in1.readInt();
        fileSize = in1.readUnsignedInt();
        versionInfo.setAssetVersion(in1.readInt());
        dataOffset = in1.readUnsignedInt();
        if (versionInfo.getAssetVersion() >= 9) {
            endianness = in1.readByte();
            in1.readBytes(reserved);
        }
    }

        public override  void write(DataWriter out1)  {
        out1.writeUnsignedInt(metadataSize);
        out1.writeUnsignedInt(fileSize);
        out1.writeInt(versionInfo.getAssetVersion());
        out1.writeUnsignedInt(dataOffset);
        if (versionInfo.getAssetVersion() >= 9) {
            out1.writeByte(endianness);
            out1.writeBytes(reserved);
        }
    }

    public long getMetadataSize() {
        return metadataSize;
    }

    public void setMetadataSize(long metadataSize) {
        this.metadataSize = metadataSize;
    }

    public long getFileSize() {
        return fileSize;
    }

    public void setFileSize(long fileSize) {
        this.fileSize = fileSize;
    }

    public int getVersion() {
        return versionInfo.getAssetVersion();
    }

    public void setVersion(int version) {
        versionInfo.setAssetVersion(version);
    }

    public long getDataOffset() {
        return dataOffset;
    }

    public void setDataOffset(long dataOffset) {
        this.dataOffset = dataOffset;
    }

    public byte getEndianness() {
        return endianness;
    }

    public void setEndianness(byte endianness) {
        this.endianness = endianness;
    }
}
