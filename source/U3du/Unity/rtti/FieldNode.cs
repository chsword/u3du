/*
 ** 2014 September 22
 **
 ** The author disclaims copyright to this source code. In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */


/**
 *
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */
public class FieldNode : Node<FieldNode> {
    
    private Type type;
    private object value;    

    public Type getType() {
        return type;
    }

    public void setType(Type field) {
        this.type = field;
    }

    public object getValue() {
        return value;
    }

    public void setValue(object value) {
        this.value = value;
    }
    
    public FieldNode getChild(string name) {
        foreach (FieldNode child in this) {
            if (child.getType().getFieldName().Equals(name)) {
                return child;
            }
        }
        return null;
    }

    public <T> T getChildValue(string name, Class<T> type) {
        FieldNode child = getChild(name);
        if (child == null) {
            return null;
        }
        
        object v = child.getValue();
        if (!type.isInstance(v)) {
            throw new RuntimeTypeException(string.Format("Wrong type for %s: expected %s, but got %s", name, type, v.GetType()));
        }
        
        return (T) v;
    }

    public void setChildValue(string name, object value) {
        FieldNode child = getChild(name);
        if (child == null) {
            throw new RuntimeTypeException("Field " + name + " doesn't exist");
        }
        
        child.setValue(value);
    }
    
    public <T> T getArrayData(Class<T> type) {
        FieldNode arrayField = getChild("Array");
        if (arrayField == null) {
            throw new RuntimeTypeException("Field is not an array");
        }
        
        return arrayField.getChildValue("data", type);
    }

    public void setArrayData(object value) {
        FieldNode arrayField = getChild("Array");
        if (arrayField == null) {
            throw new RuntimeTypeException("Field is not an array");
        }
        
        arrayField.setChildValue("data", value);
    }
    
    public <T> T getChildArrayData(string name, Class<T> type) {
        return getChild(name).getArrayData(type);
    }

    public void setChildArrayData(string name, object value) {
        getChild(name).setArrayData(value);
    }

    public byte getSInt8(string name) {
        return getChildValue(name, Number.class).byteValue();
    }
    
    public void setSInt8(string name, byte v) {
        setChildValue(name, v);
    }

    public short getUInt8(string name) {
        return (short) (getChildValue(name, Number.class).byteValue() & 0xff);
    }
    
    public void setUInt8(string name, byte v) {
        setChildValue(name, v);
    }

    public short getSInt16(string name) {
        return getChildValue(name, Number.class).shortValue();
    }
    
    public void setSInt16(string name, short v) {
        setChildValue(name, v);
    }

    public int getUInt16(string name) {
        return getChildValue(name, Number.class).shortValue() & 0xffff;
    }
    
    public void setUInt16(string name, int v) {
        setChildValue(name, v & 0xffff);
    }

    public int getSInt32(string name) {
        return getChildValue(name, Number.class).intValue();
    }
    
    public void setSInt32(string name, int v) {
        setChildValue(name, v);
    }

    public long getUInt32(string name) {
        return getChildValue(name, Number.class).longValue() & 0xffffffffL;
    }
    
    public void getUInt32(string name, long v) {
        setChildValue(name, v & 0xffffffffL);
    }

    public long getSInt64(string name) {
        return getChildValue(name, Number.class).longValue();
    }
    
    public void setSInt64(string name, long v) {
        setChildValue(name, v);
    }

    public BigInteger getUInt64(string name) {
        return getChildValue(name, BigInteger.class);
    }
    
    public void setUInt64(string name, BigInteger v) {
        setChildValue(name, v);
    }

    public float getFloat(string name) {
        return getChildValue(name, Number.class).floatValue();
    }
    
    public void setFloat(string name, float v) {
        setChildValue(name, v);
    }

    public double getDouble(string name) {
        return getChildValue(name, Number.class).doubleValue();
    }
    
    public void setDouble(string name, double v) {
        setChildValue(name, v);
    }

    public bool getBoolean(string name) {
        return getChildValue(name, Boolean.class);
    }
    
    public void setBoolean(string name, bool v) {
        setChildValue(name, v);
    }

    public string getstring(string name) {
        return getChildValue(name, string.class);
    }
    
    public void setstring(string name, string v) {
        setChildValue(name, v);
    }
    
    public FieldNode getObject(string name) {
        return getChildValue(name, FieldNode.class);
    }
    
    public void setObject(string name, FieldNode v) {
        setChildValue(name, v);
    }

        public override  int GetHashCode() {
        int hash = base.GetHashCode();
        hash = 97 * hash + Objects.GetHashCode(this.type);
        hash = 97 * hash + Objects.GetHashCode(this.value);
        return hash;
    }

        public override  bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (GetType() != obj.GetType()) {
            return false;
        }
         FieldNode other = (FieldNode) obj;
        if (!Objects.Equals(this.type, other.type)) {
            return false;
        }
        if (!Objects.Equals(this.value, other.value)) {
            return false;
        }
        return true;
    }
}
