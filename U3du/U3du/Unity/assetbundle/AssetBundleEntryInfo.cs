/*
 ** 2014 September 25
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
public class AssetBundleEntryInfo : Struct {
    
    private string name;
    private long offset;
    private long size;

    public string getName() {
        return name;
    }

    public void setName(string name) {
        this.name = name;
    }

    public long getOffset() {
        return offset;
    }

    public void setOffset(long offset) {
        this.offset = offset;
    }

    public long getSize() {
        return size;
    }

    public void setSize(long size) {
        this.size = size;
    }
    
        public override  void read(DataReader in1)  {
        name = in1.readstringNull();
        offset = in1.readUnsignedInt();
        size = in1.readUnsignedInt();
    }

        public override  void write(DataWriter out1)  {
        out1.writestringNull(name);
        out1.writeUnsignedInt(offset);
        out1.writeUnsignedInt(size);
    }

        public override  string ToString() {
        return getName();
    }
}
