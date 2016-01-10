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
 * Class that contains the runtime type of a single field.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 * @unity TypeTree
 */
public class FieldType  {
    
    public static int FLAG_FORCE_ALIGN = 0x4000;
    
    // field type string
    private string type;
    
    // field name string
    private string name;
    
    // size of the field value in1 bytes or -1 if the field contains sub-fields only
    private int size;
    
    // field index for the associated parent field
    private int index;
    
    // set to 1 if "type" is "Array" or "TypelessData"
    private int isArray;
    
    // type version, starts with 1 and is incremented when the type
    // information is updated in1 a new Unity release
    //
    // equal to serializedVersion in1 YAML Format files
    private int version;
    
    // field flags
    // observed values:
    // 0x1
    // 0x10
    // 0x800
    // 0x4000
    // 0x8000
    // 0x200000
    // 0x400000
    private int metaFlag;
    
    public bool isForceAlign() {
        return (metaFlag & FLAG_FORCE_ALIGN) != 0;
    }
    
    public void setForceAlign(bool forceAlign) {
        if (forceAlign) {
            metaFlag |= FLAG_FORCE_ALIGN;
        } else {
            metaFlag &= ~FLAG_FORCE_ALIGN;
        }
    }

    public string getTypeName() {
        return type;
    }

    public void setTypeName(string type) {
        this.type = type;
    }

    public string getFieldName() {
        return name;
    }

    public void setFieldName(string name) {
        this.name = name;
    }

    public int getSize() {
        return size;
    }

    public void setSize(int size) {
        this.size = size;
    }

    public int getIndex() {
        return index;
    }

    public void setIndex(int index) {
        this.index = index;
    }

    public bool getIsArray() {
        return isArray == 1;
    }

    public void setIsArray(bool isArray) {
        this.isArray = isArray ? 1 : 0;
    }

    public int getVersion() {
        return version;
    }

    public void setVersion(int flags1) {
        this.version = flags1;
    }

    public int getMetaFlag() {
        return metaFlag;
    }

    public void setMetaFlag(int flags2) {
        this.metaFlag = flags2;
    }
    
 
    public string ToString() {
        return type + ":" + name;
    }
     
    public void read(DataReader in1)   {
        type = in1.readstringNull(256);
        name = in1.readstringNull(256);
        size = in1.readInt();
        index = in1.readInt();
        isArray = in1.readInt();
        version = in1.readInt();
        metaFlag = in1.readInt();
    }

 
    public void write(DataWriter out1)  {
        out1.writestringNull(type);
        out1.writestringNull(name);
        out1.writeInt(size);
        out1.writeInt(index);
        out1.writeInt(isArray);
        out1.writeInt(version);
        out1.writeInt(metaFlag);
    }
    
 
    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (GetType() != obj.GetType()) {
            return false;
        }
         FieldType other = (FieldType) obj;
        if (!Objects.Equals(this.type, other.type)) {
            return false;
        }
        if (!Objects.Equals(this.name, other.name)) {
            return false;
        }
        if (this.size != other.size) {
            return false;
        }
        if (this.index != other.index) {
            return false;
        }
        if (this.isArray != other.isArray) {
            return false;
        }
        return true;
    }


    public override int GetHashCode() {
        int hash = 7;
        hash = 31 * hash + Objects.GetHashCode(this.type);
        hash = 31 * hash + Objects.GetHashCode(this.name);
        hash = 31 * hash + this.size;
        hash = 31 * hash + this.index;
        hash = 31 * hash + this.isArray;
        return hash;
    }
}