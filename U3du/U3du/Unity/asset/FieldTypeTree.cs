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
 * Class that holds the runtime type information of an asset file.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 * @unity RTTIClassHierarchyDescriptor, RTTIBaseClassDescriptor2
 */

using U3du.Extract;

public class FieldTypeTree : UnityStruct {

    private  Map<int, FieldTypeNode> typeMap;

    private int attributes;
    
    public FieldTypeTree(Map<int, FieldTypeNode> typeMap, VersionInfo versionInfo) {
        base(versionInfo);
        this.typeMap = typeMap;
    }
    
    public int getAttributes() {
        return attributes;
    }

    public void setAttributes(int version) {
        this.attributes = version;
    }

        public override  void read(DataReader in1)  {
        // revision/version for newer formats
        if (versionInfo.getAssetVersion() >= 7) {
            versionInfo.setUnityRevision(new UnityVersion(in1.readstringNull(255)));
            attributes = in1.readInt();
        }
        
        int numBaseClasses = in1.readInt();
        for (int i = 0; i < numBaseClasses; i++) {
            int classID = in1.readInt();

            FieldTypeNode node = new FieldTypeNode();
            node.read(in1);
            
            typeMap.put(classID, node);
        }
        
        // padding
        if (versionInfo.getAssetVersion() >= 7) {
            in1.readInt();
        }
    }

        public override  void write(DataWriter out1)  {
        // revision/version for newer formats
        if (versionInfo.getAssetVersion() >= 7) {
            out1.writestringNull(versionInfo.getUnityRevision().ToString());
            out1.writeInt(attributes);
        }
        
        int fields = typeMap.size();
        out1.writeInt(fields);

        for (Map.Entry<int, FieldTypeNode> entry : typeMap.entrySet()) {
            int classID = entry.getKey();
            out1.writeInt(classID);

            FieldTypeNode node = entry.getValue();
            node.write(out1);
        }
        
        // padding
        if (versionInfo.getAssetVersion() >= 7) {
            out1.writeInt(0);
        }
    }
}