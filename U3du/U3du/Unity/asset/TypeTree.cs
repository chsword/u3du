/*
 ** 2015 April 15
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
public class TypeTree : UnityStruct {
    
    private  List<TypeClass> classes = new ArrayList<>();
    private int attributes;
    private bool embedded;

    public TypeTree(VersionInfo versionInfo) {
        base(versionInfo);
    }
    
    public TypeClass getClassByID(int classID) {
        for (TypeClass typeClass : classes) {
            if (typeClass.getClassID() == classID) {
                return typeClass;
            }
        }
        
        return null;
    }
    
    public int getAttributes() {
        return attributes;
    }
    
    public void setAttributes(int attributes) {
        this.attributes = attributes;
    }
    
    public bool isEmbedded() {
        return embedded;
    }
    
    public void setEmbedded(bool embedded) {
        this.embedded = embedded;
    }

        public override  void read(DataReader in1)  {
        // revision/version for newer formats
        if (versionInfo.getAssetVersion() >= 7) {
            versionInfo.setUnityRevision(new UnityVersion(in1.readstringNull(255)));
            attributes = in1.readInt();
        }
        
        classes.clear();
        
        if (versionInfo.getAssetVersion() >= 14) {
            stringTable stInt = new stringTable();
            
            embedded = in1.readBoolean();
            int numBaseClasses = in1.readInt();

            for (int i = 0; i < numBaseClasses; i++) {
                int classID = in1.readInt();
                
                TypeClass typeClass = new TypeClass();
                typeClass.setClassID(classID);
                
                if (classID < 0) {
                    UnityGUID scriptGUID = new UnityGUID();
                    scriptGUID.read(in1);
                    typeClass.setScriptGUID(scriptGUID);
                }

                UnityGUID classGUID = new UnityGUID();
                classGUID.read(in1);
                typeClass.setClassGUID(classGUID);
                
                if (embedded) {
                    TypeNode node = new TypeNode();
                    readFieldTypeNodeNew(in1, node, stInt);
                    typeClass.setTypeTree(node);
                }
                
                classes.add(typeClass);
            }
        } else {
            int numBaseClasses = in1.readInt();
            for (int i = 0; i < numBaseClasses; i++) {
                int classID = in1.readInt();

                TypeClass typeClass = new TypeClass();
                typeClass.setClassID(classID);
                
                TypeNode typeNode = new TypeNode();
                readFieldTypeNodeOld(in1, typeNode, 0);
                typeClass.setTypeTree(typeNode);
                
                classes.add(typeClass);
            }
            
            embedded = numBaseClasses > 0;

            // padding
            if (versionInfo.getAssetVersion() >= 7) {
                in1.readInt();
            }
        }
    }
    
    private void readFieldTypeNodeOld(DataReader in1, TypeNode node, int level)  {
        Type type = new Type(versionInfo);
        type.read(in1);
        type.setTreeLevel(level);
        
        node.setType(type);
        
        int numChildren = in1.readInt();
        for (int i = 0; i < numChildren; i++) {
            TypeNode childNode = new TypeNode();
            readFieldTypeNodeOld(in1, childNode, level + 1);
            node.add(childNode);
        }        
    }
    
    private void readFieldTypeNodeNew(DataReader in1, TypeNode node, stringTable stInt)  {
        int numFields = in1.readInt();
        int stringTableLen = in1.readInt();

        // read types
        List<Type> types = new ArrayList<>(numFields);
        for (int j = 0; j < numFields; j++) {
            Type type = new Type(versionInfo);
            type.read(in1);
            types.add(type);
        }

        // read string table
        byte[] stringTable = new byte[stringTableLen];
        in1.readBytes(stringTable);

        // assign strings
        stringTable stExt = new stringTable();
        stExt.loadstrings(stringTable);
        for (Type field : types) {
            int nameOffset = field.getNameOffset();
            string name = stExt.getstring(nameOffset);
            if (name == null) {
                name = stInt.getstring(nameOffset);
            }
            field.setFieldName(name);
            
            int typeOffset = field.getTypeOffset();
            string type = stExt.getstring(typeOffset);
            if (type == null) {
                type = stInt.getstring(typeOffset);
            }
            field.setTypeName(type);
        }
        
        // convert list to tree structure
        TypeNode currentNode = null;
        for (Type type : types) {
            if (currentNode == null) {
                node.setType(type);
                currentNode = node;
                continue;
            }
            
            int treeLevel = type.getTreeLevel();
            int currentTreeLevel = currentNode.getType().getTreeLevel();
            
//            System.out1.println(treeLevel + ": " + stringUtils.repeat("  ", treeLevel) + " " + type);
            
            TypeNode childNode = new TypeNode();
            childNode.setType(type);
            
            currentNode.add(childNode);
            
            if (treeLevel > currentTreeLevel) {
                // move one level up
                currentNode = childNode;
            } else if (treeLevel < currentTreeLevel) {
                // move levels down
                for (; treeLevel < currentTreeLevel; currentTreeLevel--) {
                    currentNode = currentNode.getParent();
                }
            }
        }
    }

        public override  void write(DataWriter out1)  {
        // revision/version for newer formats
        if (versionInfo.getAssetVersion() >= 7) {
            out1.writestringNull(versionInfo.getUnityRevision().ToString());
            out1.writeInt(attributes);
        }
        
        if (versionInfo.getAssetVersion() >= 14) {
            // TODO
            throw new UnsupportedOperationException();
        } else {
            int numBaseClasses = classes.size();
            out1.writeInt(numBaseClasses);

            for (TypeClass bc : classes) {
                int classID = bc.getClassID();
                out1.writeInt(classID);

                TypeNode node = bc.getTypeTree();
                writeFieldTypeNodeOld(out1, node);
            }

            // padding
            if (versionInfo.getAssetVersion() >= 7) {
                out1.writeInt(0);
            }
        }
    }
    
    private void writeFieldTypeNodeOld(DataWriter out1, TypeNode node)  {
        Type type = node.getType();
        type.write(out1);
        
        int numChildren = node.size();
        out1.writeInt(numChildren);
        for (TypeNode childNode : node) {
            writeFieldTypeNodeOld(out1, childNode);
        }
    }
}
