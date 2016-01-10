/*
 ** 2014 September 25
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */


/**
 * Streaming reader for Unity asset bundles.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System.Collections.Generic;

public class AssetBundleReader : Closeable, Iterable<AssetBundleEntry> {
    
    private  AssetBundleHeader header = new AssetBundleHeader();
    private  List<AssetBundleEntry> entries = new ArrayList<>();
    private  List<AssetBundleEntryInfo> entryInfos = new ArrayList<>();
    
    private  DataReader in1;
    private CountingInputStream lzma;

    public AssetBundleReader(Path file) throws AssetBundleException, IOException {
        in1 = DataReaders.forFile(file, READ);
        header.read(in1);

        // check signature
        if (!header.hasValidSignature()) {
            throw new AssetBundleException("Invalid signature");
        }
        
        long dataHeaderSize = header.getDataHeaderSize();
        if (dataHeaderSize == 0) {
            // old stream versions don't store the data header size, so use a large
            // fixed number instead
            dataHeaderSize = 4096;
        }
        
        InputStream is = getDataInputStream(0, dataHeaderSize);
        DataReader inData = DataReaders.forInputStream(is);
        int files = inData.readInt();

        for (int i = 0; i < files; i++) {
            AssetBundleEntryInfo entryInfo = new AssetBundleEntryInfo();
            entryInfo.read(inData);
            entryInfos.add(entryInfo);
        }
        
        // sort entries by offset so that they're in1 the order in1 which they
        // appear in1 the file, which is convenient for compressed bundles
        Collections.sort(entryInfos, new EntryComparator());
        
        for (AssetBundleEntryInfo entryInfo : entryInfos) {
            entries.add(new AssetBundleInternalEntry(this, entryInfo));
        }
    }
    
    private InputStream getDataInputStream(long offset, long size)  {
        InputStream is;
        
        // use LZMA stream if the bundle is compressed
        if (header.isCompressed()) {
            // create initial input stream if required
            if (lzma == null) {
                lzma = getLZMAInputStream();
            }
            
            // recreate stream if the offset is behind
            long lzmaOffset = lzma.getByteCount();
            if (lzmaOffset > offset) {
                lzma.close();
                lzma = getLZMAInputStream();
            }
            
            // skip forward if required
            if (lzmaOffset < offset) {
                lzma.skip(offset - lzmaOffset);
            }
            
            is = lzma;
        } else {
            in1.position(header.getHeaderSize() + offset);
            is = in1.stream();
        }
        
        return new BoundedInputStream(is, size);
    }
    
    private CountingInputStream getLZMAInputStream()  {
        in1.position(header.getHeaderSize());
        return new CountingInputStream(new LzmaInputStream(in1.stream()));
    }
    
    InputStream getInputStreamForEntry(AssetBundleEntryInfo info)  {
        return getDataInputStream(info.getOffset(), info.getSize());
    }

    public AssetBundleHeader getHeader() {
        return header;
    }
    
    public List<AssetBundleEntryInfo> getEntryInfos() {
        return Collections.unmodifiableList(entryInfos);
    }
    
    public List<AssetBundleEntry> getEntries() {
        return Collections.unmodifiableList(entries);
    }

        public override  Iterator<AssetBundleEntry> iterator() {
        return entries.iterator();
    }
    
        public override  void close()  {
        if (lzma != null) {
            lzma.close();
        }
        in1.close();
    }
    
    private class EntryComparator : Comparator<AssetBundleEntryInfo> {

        @Override
        public int compare(AssetBundleEntryInfo o1, AssetBundleEntryInfo o2) {
            long ofs1 = o1.getOffset();
            long ofs2 = o2.getOffset();

            if (ofs1 > ofs2) {
                return 1;
            } else if (ofs1 < ofs2) {
                return -1;
            } else {
                return 0;
            }
        }
    }
}
