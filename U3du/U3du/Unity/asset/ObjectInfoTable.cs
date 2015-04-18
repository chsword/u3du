/*
 ** 2013 June 17
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

using U3du.Extract;

public class ObjectInfoTable : UnityStruct {
    
    private  Map<int, ObjectInfo> infoMap;

    public ObjectInfoTable(Map<int, ObjectInfo> infoMap, VersionInfo versionInfo)
        : base(versionInfo)
    {
        ;
        this.infoMap = infoMap;
    }
    
        public override  void read(DataReader in1)  {
        int entries = in1.readInt();
        
        if (versionInfo.getAssetVersion() > 13) {
            in1.align(4);
        }

        for (int i = 0; i < entries; i++) {
            int pathID = in1.readInt();
            ObjectInfo info = new ObjectInfo(versionInfo);
            
            info.read(in1);
            infoMap.put(pathID, info);
        }
    }

        public override  void write(DataWriter out1)  {
        int entries = infoMap.size();
        out1.writeInt(entries);
        
        if (versionInfo.getAssetVersion() > 13) {
            out1.align(4);
        }

        for (Map.Entry<int, ObjectInfo> infoEntry : infoMap.entrySet()) {
            int pathID = infoEntry.getKey();
            ObjectInfo info = infoEntry.getValue();
            
            out1.writeInt(pathID);
            info.write(out1);
        }
    }
}
