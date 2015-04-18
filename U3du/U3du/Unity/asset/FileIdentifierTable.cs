/*
 ** 2013 August 16
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

using System.Collections.Generic;

public class FileIdentifierTable : UnityStruct : Iterable<FileIdentifier> {

    private  List<FileIdentifier> fileIDs;
    
    public FileIdentifierTable(List<FileIdentifier> fileIDs, VersionInfo versionInfo) {
        base(versionInfo);
        this.fileIDs = fileIDs;
    }
    
        public override  void read(DataReader in1)  {
        int entries = in1.readInt();
        for (int i = 0; i < entries; i++) {
            FileIdentifier ref = new FileIdentifier(versionInfo);
            ref.read(in1);
            fileIDs.add(ref);
        }
    }

        public override  void write(DataWriter out1)  {
        int entries = fileIDs.size();
        out1.writeInt(entries);

        for (FileIdentifier ref : fileIDs) {
            ref.write(out1);
        }
    }

        public override  Iterator<FileIdentifier> iterator() {
        return fileIDs.iterator();
    }
}
