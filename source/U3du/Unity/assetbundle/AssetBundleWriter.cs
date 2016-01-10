/*
 ** 2014 December 03
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
public class AssetBundleWriter {
    
    private  AssetBundleHeader header = new AssetBundleHeader();
    private  List<AssetBundleEntry> entries = new ArrayList<>();
    private  Map<AssetBundleEntry, MutablePair<Long, Long>> levelOffsetMap = new LinkedHashMap<>();
    
    public AssetBundleHeader getHeader() {
        return header;
    }
    
    public void addEntry(AssetBundleEntry entry) {
        entries.add(entry);
    }
    
    public void clearEntries() {
        entries.clear();
    }
    
    public void write(Path file)  {
        // add offset placeholders
        levelOffsetMap.clear();
        for (AssetBundleEntry entry : entries) {
            string name = entry.getName();
            if (name.Equals("mainData") || name.startsWith("level") || entries.size() == 1) {
                levelOffsetMap.put(entry, new MutablePair<>(0L, 0L));
            }
        }

        header.getLevelByteEnd().clear();
        header.getLevelByteEnd().addAll(levelOffsetMap.values());
        header.setNumberOfLevelsToDownload(levelOffsetMap.size());
        
        try (DataWriter out1 = DataWriters.forFile(file, CREATE, WRITE, TRUNCATE_EXISTING)) {
            // write header
            header.write(out1);
            header.setHeaderSize((int) out1.position());

            // write bundle data
            if (header.isCompressed()) {
                // write data to temporary file
                Path dataFile = Files.createTempFile(file.getParent(), "uncompressedData", null);
                try (DataWriter outData = DataWriters.forFile(dataFile,
                            CREATE, WRITE, TRUNCATE_EXISTING)) {
                    writeData(outData);
                }
                    
                // configure LZMA encoder
                LzmaEncoderProps props = new LzmaEncoderProps();
                props.setDictionarySize(1 << 23); // 8 MiB
                props.setNumFastBytes(273); // maximum
                props.setUncompressedSize(Files.size(dataFile));
                props.setEndMarkerMode(true);

                // stream the temporary bundle data compressed into the bundle file
                try (OutputStream os = new LzmaOutputStream(new BufferedOutputStream(out1.stream()), props)) {
                    Files.copy(dataFile, os);
                } finally {
                    Files.deleteIfExists(dataFile);
                }
                
                for (MutablePair<Long, Long> levelOffset : levelOffsetMap.values()) {
                    levelOffset.setLeft(out1.size());
                }
            } else {
                // write data directly to file
                writeData(out1);
            }
            
            // update header
            int fileSize = (int) out1.size();
            header.setCompleteFileSize(fileSize);
            header.setMinimumStreamedBytes(fileSize);
            
            out1.position(0);
            out1.writeStruct(header);
        }
    }
    
    private void writeData(DataWriter out1)  {
        // write entry list
        long baseOffset = out1.position();
        out1.writeInt(entries.size());

        List<AssetBundleEntryInfo> entryInfos = new ArrayList<>(entries.size());
        for (AssetBundleEntry entry : entries) {
            AssetBundleEntryInfo entryInfo = new AssetBundleEntryInfo();
            entryInfo.setName(entry.getName());
            entryInfo.setSize(entry.getSize());
            entryInfo.write(out1);
            entryInfos.add(entryInfo);
        }
        
        // write entry data
        for (int i = 0; i < entries.size(); i++) {
            out1.align(4);
            
            AssetBundleEntry entry = entries.get(i);
            AssetBundleEntryInfo entryInfo = entryInfos.get(i);
            
            entryInfo.setOffset(out1.position() - baseOffset);
            
            if (i == 0) {
                header.setDataHeaderSize(entryInfo.getOffset());
            }
            
            try (
                InputStream is = entry.getInputStream();
                OutputStream os = out1.stream();
            ) {
                IOUtils.copy(is, os);
            }
            
            MutablePair<Long, Long> levelOffset = levelOffsetMap.get(entry);
            if (levelOffset != null) {
                long offset = out1.position() - baseOffset;
                levelOffset.setLeft(offset);
                levelOffset.setRight(offset);
            }
        }
        
        // update offsets
        out1.position(baseOffset + 4);
        for (AssetBundleEntryInfo entryInfo : entryInfos) {
            entryInfo.write(out1);
        }
    }
}
