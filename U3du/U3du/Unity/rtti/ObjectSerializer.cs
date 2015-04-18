/*
 ** 2014 November 28
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
public class ObjectSerializer {
    
    private static  bool DEBUG = false;
    private static  int ALIGNMENT = 4;
    
    private ByteBuffer soundData;
    private VersionInfo versionInfo;
    
    public ByteBuffer getSoundData() {
        return soundData;
    }
    
    public void setSoundData(ByteBuffer soundData) {
        this.soundData = soundData;
    }
    
    public void serialize(ObjectData data)  {
        throw new UnsupportedOperationException("Not supported yet.");
    }
    
    public void deserialize(ObjectData data)  {
        versionInfo = data.getVersionInfo();
        
        DataReader in1 = DataReaders.forByteBuffer(data.getBuffer());
        in1.order(versionInfo.getByteOrder());
        in1.position(0);
        
        TypeNode typeNode = data.getTypeTree();
        FieldNode instance = readObject(in1, typeNode);
        
        // check if all bytes have been read
        if (in1.hasRemaining()) {
            throw new RuntimeTypeException("Remaining bytes: " + in1.remaining());
        }
        
        data.setInstance(instance);
    }
    
    private FieldNode readObject(DataReader in1, TypeNode typeNode)  {
        Type type = typeNode.getType();
        
        FieldNode fieldNode = new FieldNode();
        fieldNode.setType(type);
        
        // if the type has no children, it has a primitve value
        if (typeNode.isEmpty() && type.getSize() > 0) {
            fieldNode.setValue(readPrimitiveValue(in1, type, -1));
        }
        
        // read object fields
        for (TypeNode childTypeNode : typeNode) {
            Type childType = childTypeNode.getType();
            
            // Check if the current node is an array and if the current field is
            // "data". In that case, "data" needs to be read "size" times.
            if (type.getIsArray() && childType.getFieldName().Equals("data")) {
                int size = fieldNode.getSInt32("size");
                
                FieldNode childFieldNode = new FieldNode();
                childFieldNode.setType(childType);

                // if the child type has no children, it has a primitve array
                if (childTypeNode.isEmpty()) {
                    childFieldNode.setValue(readPrimitiveValue(in1, childType, size));
                } else {
                    // read list of object nodes
                    List<FieldNode> childFieldNodes = new ArrayList<>(size);
                    for (int i = 0; i < size; i++) {
                        childFieldNodes.add(readObject(in1, childTypeNode));
                    }
                    childFieldNode.setValue(childFieldNodes);
                }
                
                fieldNode.add(childFieldNode);
            } else {
                fieldNode.add(readObject(in1, childTypeNode));
            }
        }
        
        // convert byte buffers to string instances in1 "string" fields for convenience
        if (fieldNode.getType().getTypeName().Equals("string")) {
            // strings use "char" arrays, so it should be wrapped in1 a ByteBuffer
            ByteBuffer buf = fieldNode.getArrayData(ByteBuffer.class);
            fieldNode.setValue(new string(buf.array(), "UTF-8"));
        }
        
        return fieldNode;
    }
    
    private object readPrimitiveValue(DataReader in1, Type type, int size) , RuntimeTypeException {
        long pos = 0;
        if (DEBUG) {
            pos = in1.position();
        }
        
        object value;
        if (size == -1) {
            value = readPrimitive(in1, type);
            if (type.isForceAlign()) {
                 in1.align(ALIGNMENT);
            }
        } else {
            value = readPrimitiveArray(in1, type, size);
            if (versionInfo.getAssetVersion() > 5) {
                // arrays always need to be aligned in1 newer versions
                in1.align(ALIGNMENT);
            }
        }
        
        if (DEBUG) {
            long bytes = in1.position() - pos;
            System.out1.printf("0x%x: %s %s = %s, b: %d, v: %d, f: 0x%x, s: %d\n",
                    pos, type.getTypeName(), type.getFieldName(), value, bytes,
                    type.getVersion(), type.getMetaFlag(), size);
        }
        
        return value;
    }
    
    private object readPrimitive(DataReader in1, Type type) , RuntimeTypeException {
        switch (type.getTypeName()) {
            // 1 byte
            case "bool":
                return in1.readBoolean();

            case "SInt8":
                return in1.readByte();

            case "UInt8":
            case "char":
                return in1.readUnsignedByte();

            // 2 bytes
            case "SInt16":
            case "short":
                return in1.readShort();

            case "UInt16":
            case "unsigned short":
                return in1.readUnsignedShort();

            // 4 bytes
            case "SInt32":
            case "int":
                return in1.readInt();

            case "UInt32":
            case "unsigned int":
                return in1.readUnsignedInt();

            case "float":
                return in1.readFloat();

            // 8 bytes
            case "SInt64":
            case "long":
                return in1.readLong();

            case "UInt64":
            case "unsigned long":
                return in1.readUnsignedLong();

            case "double":
                return in1.readDouble();

            default:
                throw new RuntimeTypeException("Unknown primitive type: " + type.getTypeName());
        }
    }
    
    private object readPrimitiveArray(DataReader in1, Type type, int size) , RuntimeTypeException {
        switch (type.getTypeName()) {
            // read byte arrays natively and wrap them as ByteBuffers,
            // which is much faster and more efficient than a list of wrappped
            // Byte/int objects
            case "SInt8":
            case "UInt8":
                ByteBuffer buf;
                
                // NOTE: AudioClips "fake" the size of m_AudioData when the stream is
                // stored in1 a separate file. The array contains just an offset integer
                // in1 that case, so pay attention to the bytes remaining in1 the buffer
                // as well to avoid EOFExceptions.
                long remaining = in1.remaining();
                if (size > remaining && remaining == 4) {
                    int offset = in1.readInt();
                    // create empty sound buffer in1 case the .resS file doesn't
                    // exist
                    if (soundData != null) {
                        buf = ByteBufferUtils.getSlice(soundData, offset, size);
                    } else {
                        buf = ByteBufferUtils.allocate(size);
                    }
                } else {
                    buf = ByteBufferUtils.allocate(size);
                    in1.readBuffer(buf);
                }

                buf.clear();
                return buf;

            // always wrap char arrays so array() is available on the buffer, which
            // is required to convert them to Java strings in1 readObject()
            case "char":
                byte[] raw = new byte[size];
                in1.readBytes(raw, 0, size);
                return ByteBuffer.wrap(raw);

            // read a list of primitive objects
            default:
                List list = new ArrayList(size);
                for (int i = 0; i < size; i++) {
                    list.add(readPrimitive(in1, type));
                }
                return list;
        }
    }
}
