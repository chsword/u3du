/*
 ** 2013 June 15
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */



/**
 * Reader for Unity asset files.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System.Collections.Generic;
using U3du.Extract;

public class AssetFile : FileHandler {
    
    
    
    private static  int METADATA_PADDING = 4096;
    
    // collection fields
    private  Map<int, ObjectInfo> objectInfoMap = new LinkedHashMap<>();
    private  List<FileIdentifier> externals = new ArrayList<>();
    private  List<ObjectData> objectList = new ArrayList<>();
    private  List<ObjectData> objectListBroken= new ArrayList<>();
    
    // struct fields
    private  VersionInfo versionInfo = new VersionInfo();
    private  AssetHeader header = new AssetHeader(versionInfo);
    private  ObjectInfoTable objectInfoStruct = new ObjectInfoTable(objectInfoMap, versionInfo);
    private  TypeTree typeTreeStruct = new TypeTree(versionInfo);
    private  FileIdentifierTable externalsStruct = new FileIdentifierTable(externals, versionInfo);
    
    // data block fields
    private  DataBlock headerBlock = new DataBlock();
    private  DataBlock objectInfoBlock = new DataBlock();
    private  DataBlock objectDataBlock = new DataBlock();
    private  DataBlock typeTreeBlock = new DataBlock();
    private  DataBlock externalsBlock = new DataBlock();
    
    // misc fields
    private ByteBuffer audioBuffer;
    
        public override  void load(Path file)  {
        load(file, null);
    }

    private void load(Path file, Map<Path, AssetFile> childAssets)  {
        sourceFile = file;
        
        if (childAssets == null) {
            childAssets = new HashMap<>();
        }
        childAssets.put(file, this);
        
        string fileName = file.getFileName().ToString();
        string fileExt = FilenameUtils.getExtension(fileName);
        
        DataReader reader;
        
        // join split asset files before loading
        if (fileExt.startsWith("split")) {
            L.fine("Found split asset file");
            
            fileName = FilenameUtils.removeExtension(fileName);
            List<Path> parts = new ArrayList<>();
            int splitIndex = 0;

            // collect all files with .split0 to .splitN extension
            while (true) {
                string splitName = string.Format("%s.split%d", fileName, splitIndex);
                Path part = file.resolveSibling(splitName);
                if (Files.notExists(part)) {
                    break;
                }
                
                L.log(Level.FINE, "Adding splinter {0}", part.getFileName());
                
                splitIndex++;
                parts.add(part);
            }
            
            // load all parts to one byte buffer
            reader = DataReaders.forByteBuffer(ByteBufferUtils.load(parts));
        } else {
            reader = DataReaders.forFile(file, READ);
        }
        
        // load audio buffer if existing        
        loadResourceStream(file.resolveSibling(fileName + ".streamingResourceImage"));
        loadResourceStream(file.resolveSibling(fileName + ".resS"));
        
        load(reader);
        
        for (FileIdentifier external : externals) {
            string filePath = external.getFilePath();
            
            if (filePath == null || filePath.isEmpty()) {
                continue;
            }
            
            filePath = filePath.replace("library/", "resources/");
            
            Path refFile = file.resolveSibling(filePath);
            if (Files.exists(refFile)) {
                AssetFile childAsset = childAssets.get(refFile);
                
                if (childAsset == null) {
                    L.log(Level.FINE, "Loading dependency {0}", filePath);
                    childAsset = new AssetFile();
                    childAsset.load(refFile, childAssets);
                }
                
                external.setAssetFile(childAsset);
            }
        }
    }
    
    private void loadResourceStream(Path streamFile)  {
        if (Files.exists(streamFile)) {
            L.log(Level.FINE, "Found sound stream file {0}", streamFile.getFileName());
            audioBuffer = ByteBufferUtils.openReadOnly(streamFile);
        }
    }
      
        public override  void load(DataReader in1)  {
        loadHeader(in1);

        // read as little endian from now on
        in1.order(ByteOrder.LITTLE_ENDIAN);
        
        // older formats store the object data before the structure data
        if (header.getVersion() < 9) {
            in1.position(header.getFileSize() - header.getMetadataSize() + 1);
        }
        
        loadMetadata(in1);
        loadObjects(in1);
        checkBlocks();
    }
    
    private void loadHeader(DataReader in1)  {
        headerBlock.markBegin(in1);
        in1.readStruct(header);
        headerBlock.markEnd(in1);
        L.log(Level.FINER, "headerBlock: {0}", headerBlock);
    }
    
    private void loadMetadata(DataReader in1)  {
        in1.order(versionInfo.getByteOrder());
        
        // read structure data
        typeTreeBlock.markBegin(in1);
        in1.readStruct(typeTreeStruct);
        typeTreeBlock.markEnd(in1);
        L.log(Level.FINER, "typeTreeBlock: {0}", typeTreeBlock);
        
        objectInfoBlock.markBegin(in1);
        in1.readStruct(objectInfoStruct);
        objectInfoBlock.markEnd(in1);
        L.log(Level.FINER, "objectInfoBlock: {0}", objectInfoBlock);
        
        // unknown block for Unity 5
        if (header.getVersion() > 13) {
            in1.align(4);
            int num = in1.readInt();
            for (int i = 0; i < num; i++) {
                in1.readInt();
                in1.readInt();
                in1.readInt();
            }
        }

        externalsBlock.markBegin(in1);
        in1.readStruct(externalsStruct);
        externalsBlock.markEnd(in1);
        L.log(Level.FINER, "externalsBlock: {0}", externalsBlock);
    }
    
    private void loadObjects(DataReader in1)  {
        long ofsMin = Long.MAX_VALUE;
        long ofsMax = Long.MIN_VALUE;
        
        for (Map.Entry<int, ObjectInfo> infoEntry : objectInfoMap.entrySet()) {
            ObjectInfo info = infoEntry.getValue();
            int id = infoEntry.getKey();
            
            ByteBuffer buf = ByteBufferUtils.allocate((int) info.getLength());
            
            long ofs = header.getDataOffset() + info.getOffset();
            
            ofsMin = Math.min(ofsMin, ofs);
            ofsMax = Math.max(ofsMax, ofs + info.getLength());
            
            in1.position(ofs);
            in1.readBuffer(buf);
            
            TypeNode typeNode = null;
            
            TypeClass typeClass = typeTreeStruct.getClassByID(info.getTypeID());
            if (typeClass != null) {
                typeNode = typeClass.getTypeTree();
            }
            
            // get type from database if the embedded one is missing
//            if (typeNode == null) {
//                typeNode = TypeTreeUtils.getNode(info.getUnityClass(),
//                        versionInfo.getUnityRevision(), false);
//            }
                       
            ObjectData data = new ObjectData(id, versionInfo);
            data.setInfo(info);
            data.setBuffer(buf);
            data.setTypeTree(typeNode);
            
            ObjectSerializer serializer = new ObjectSerializer();
            serializer.setSoundData(audioBuffer);
            data.setSerializer(serializer);
            
            // Add typeless objects to an internal list. They can't be
            // (de)serialized, but can still be written to the file.
            if (typeNode == null) {
                // log warning if it's not a MonoBehaviour
                if (info.getClassID() != 114) {
                    L.log(Level.WARNING, "{0} has no type information!", data.ToString());
                }
                objectListBroken.add(data);
            } else {
                objectList.add(data);
            }
        }
        
        objectDataBlock.setOffset(ofsMin);
        objectDataBlock.setEndOffset(ofsMax);
        L.log(Level.FINER, "objectDataBlock: {0}", objectDataBlock);
    }
    
        public override  void save(DataWriter out1)  {
        saveHeader(out1);
        
        // write as little endian from now on
        out1.order(ByteOrder.LITTLE_ENDIAN);
        
        // older formats store the object data before the structure data
        if (header.getVersion() < 9) {
            header.setDataOffset(0);
            
            saveObjects(out1);
            out1.writeUnsignedByte(0);
            
            saveMetadata(out1);
            out1.writeUnsignedByte(0);
        } else {
            saveMetadata(out1);
            
            // original files have a minimum padding of 4096 bytes after the
            // metadata
            if (out1.position() < METADATA_PADDING) {
                out1.align(METADATA_PADDING);
            }
            
            out1.align(16);
            header.setDataOffset(out1.position());
            
            saveObjects(out1);
            
            // write updated path table
            out1.position(objectInfoBlock.getOffset());
            out1.writeStruct(objectInfoStruct);
        }
        
        // update header
        header.setFileSize(out1.size());
        
        // FIXME: the metadata size is slightly off in1 comparison to original files
        int metadataOffset = header.getVersion() < 9 ? 2 : 1;
        
        header.setMetadataSize(typeTreeBlock.getLength()
                + objectInfoBlock.getLength()
                + externalsBlock.getLength()
                + metadataOffset);
        
        // write updated header
        out1.order(ByteOrder.BIG_ENDIAN);
        out1.position(headerBlock.getOffset());
        out1.writeStruct(header);
             
        checkBlocks();
    }
    
    private void saveHeader(DataWriter out1)  {
        headerBlock.markBegin(out1);
        out1.writeStruct(header);
        headerBlock.markEnd(out1);
        L.log(Level.FINER, "headerBlock: {0}", headerBlock);
    }
    
    private void saveMetadata(DataWriter out1)  {
        out1.order(versionInfo.getByteOrder());
        
        typeTreeBlock.markBegin(out1);
        out1.writeStruct(typeTreeStruct);
        typeTreeBlock.markEnd(out1);
        L.log(Level.FINER, "typeTreeBlock: {0}", typeTreeBlock);

        objectInfoBlock.markBegin(out1);
        out1.writeStruct(objectInfoStruct);
        objectInfoBlock.markEnd(out1);
        L.log(Level.FINER, "objectInfoBlock: {0}", objectInfoBlock);

        externalsBlock.markBegin(out1);
        out1.writeStruct(externalsStruct);
        externalsBlock.markEnd(out1);
        L.log(Level.FINER, "externalsBlock: {0}", externalsBlock);
    }
    
    private void saveObjects(DataWriter out1)  {
        long ofsMin = Long.MAX_VALUE;
        long ofsMax = Long.MIN_VALUE;
        
        // merge object lists
        objectList.addAll(objectListBroken);
        
        for (ObjectData data : objectList) {
            ByteBuffer bb = data.getBuffer();
            bb.rewind();
            
            out1.align(8);
            
            ofsMin = Math.min(ofsMin, out1.position());
            ofsMax = Math.max(ofsMax, out1.position() + bb.remaining());
            
            ObjectInfo info = data.getInfo();            
            info.setOffset(out1.position() - header.getDataOffset());
            info.setLength(bb.remaining());

            out1.writeBuffer(bb);
        }
        
        // separate object lists
        objectList.removeAll(objectListBroken);
        
        objectDataBlock.setOffset(ofsMin);
        objectDataBlock.setEndOffset(ofsMax);
        L.log(Level.FINER, "objectDataBlock: {0}", objectDataBlock);
    }
    
    private void checkBlocks() {
        // sanity check for the data blocks
        assert !headerBlock.isIntersecting(typeTreeBlock);
        assert !headerBlock.isIntersecting(objectInfoBlock);
        assert !headerBlock.isIntersecting(externalsBlock);
        assert !headerBlock.isIntersecting(objectDataBlock);
        
        assert !typeTreeBlock.isIntersecting(objectInfoBlock);
        assert !typeTreeBlock.isIntersecting(externalsBlock);
        assert !typeTreeBlock.isIntersecting(objectDataBlock);
        
        assert !objectInfoBlock.isIntersecting(externalsBlock);
        assert !objectInfoBlock.isIntersecting(objectDataBlock);
        
        assert !objectDataBlock.isIntersecting(externalsBlock);
    }

    public VersionInfo getVersionInfo() {
        return versionInfo;
    }

    public AssetHeader getHeader() {
        return header;
    }
    
    public int getTypeTreeAttributes() {
        return typeTreeStruct.getAttributes();
    }
    
    public Map<int, FieldTypeNode> getTypeTree() {
        return new HashMap();
    }

    public Map<int, ObjectInfo> getObjectInfoMap() {
        return objectInfoMap;
    }
    
    public List<ObjectData> getObjects() {
        return objectList;
    }
    
    public List<FileIdentifier> getExternals() {
        return externals;
    }

    public bool isStandalone() {
        return typeTreeStruct.isEmbedded();
    }
    
    public void setStandalone() {
        typeTreeStruct.setEmbedded(false);
    }
}