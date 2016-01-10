/*
 ** 2014 September 20
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

public class ObjectData {
    
    private  int id;
    private  VersionInfo versionInfo;
    private ObjectSerializer serializer;
    private ObjectInfo info;
    private ByteBuffer buffer;
    private TypeNode typeTree;
    private FieldNode instance;
    
    public ObjectData(int id, VersionInfo versionInfo) {
        this.id = id;
        this.versionInfo = versionInfo;
    }
    
    public int getID() {
        return id;
    }
    
    public VersionInfo getVersionInfo() {
        return versionInfo;
    }
    
    public ObjectSerializer getSerializer() {
        return serializer;
    }
    
    public void setSerializer(ObjectSerializer serializer) {
        this.serializer = serializer;
    }
    
    public ObjectInfo getInfo() {
        return info;
    }
    
    public void setInfo(ObjectInfo info) {
        this.info = info;
    }

    public ByteBuffer getBuffer() {
        if (buffer == null && instance != null && typeTree != null) {
            try {
                serializer.serialize(this);
            } catch (IOException ex) {
                throw new RuntimeTypeException(ex);
            }
        }
        return buffer;
    }
    
    public void setBuffer(ByteBuffer buffer) {
        this.buffer = buffer;
    }
    
    public TypeNode getTypeTree() {
        return typeTree;
    }

    public void setTypeTree(TypeNode typeTree) {
        this.typeTree = typeTree;
    }

    public FieldNode getInstance() {
        if (instance == null && buffer != null && typeTree != null) {
            try {
                serializer.deserialize(this);
            } catch (IOException ex) {
                throw new RuntimeTypeException(ex);
            }
        }
        return instance;
    }

    public void setInstance(FieldNode instance) {
        this.instance = instance;
    }
    
    public string getName() {
        string name = getInstance().getstring("m_Name");

        if (name == null || name.isEmpty()) {
            name = string.Format("object %d", id);
        }
        
        return name;
    }

    public override string ToString() {
        return "object " + getID() + " " + getInfo().ToString();
    }
}
