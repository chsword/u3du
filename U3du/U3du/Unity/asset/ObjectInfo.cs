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
 * @unity SerializedFile::ObjectInfo
 */
public class ObjectInfo : UnityStruct {
    
    // object data offset
    private long offset;
    
    // object data size
    private long length;
    
    // Type ID, equal to classID if it's not a MonoBehaviour
    private int typeID;
    
    // Class ID, probably something else in1 asset Format <=5
    private int classID;
    
    // set to 1 if destroyed object instances are stored?
    private short isDestroyed;
    
    private int unknown;
    
    public ObjectInfo(VersionInfo versionInfo) : base(versionInfo){
    }

        public override  void read(DataReader in1)  {
        if (versionInfo.getAssetVersion() > 13) {
            unknown = in1.readInt();
        }
        offset = in1.readUnsignedInt();
        length = in1.readUnsignedInt();
        typeID = in1.readInt();
        classID = in1.readShort();
        isDestroyed = in1.readShort();

        assert typeID == classID || (classID == 114 && typeID < 0);
    }

        public override  void write(DataWriter out1)  {
        if (versionInfo.getAssetVersion() > 13) {
            out1.writeInt(unknown);
        }
        
        out1.writeUnsignedInt(offset);
        out1.writeUnsignedInt(length);
        out1.writeInt(typeID);
        out1.writeShort((short) classID);
        out1.writeShort(isDestroyed);
    }

    public long getOffset() {
        return offset;
    }

    public void setOffset(long offset) {
        this.offset = offset;
    }

    public long getLength() {
        return length;
    }

    public void setLength(long length) {
        this.length = length;
    }
    
    public bool isScript() {
        return typeID < 0;
    }

    public int getTypeID() {
        return typeID;
    }

    public void setTypeID(int typeID) {
        this.typeID = typeID;
    }
    
    public int getClassID() {
        return classID;
    }

    public void setClassID(int classID) {
        this.classID = classID;
    }
    
    public UnityClass getUnityClass() {
        return new UnityClass(classID);
    }
    
        public override  string ToString() {
        return getUnityClass().ToString();
    }
}