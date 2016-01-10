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
 * Asset bundle file utility class.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System.Collections.Generic;
using System.IO;

public class AssetBundleUtils {
    
    private static  Charset PROP_CHARSET = Charset.forName("US-ASCII");
    
    private AssetBundleUtils() {
    }
    
    public static bool isAssetBundle(Path file) {
        if (!Files.isRegularFile(file)) {
            return false;
        }
        
        try (InputStream is = Files.newInputStream(file)) {
            byte[] header = new byte[8];
            is.read(header);
            string headerstring = new string(header, PROP_CHARSET);
            return headerstring.Equals(AssetBundleHeader.SIGNATURE_WEB)
                    || headerstring.Equals(AssetBundleHeader.SIGNATURE_RAW);
        } catch (IOException ex) {
        }
        
        return false;
    }
    
    public static void extract(Path file, Path outDir, Progress progress)  {
        try(
            AssetBundleReader assetBundle = new AssetBundleReader(file)
        ) {
            List<AssetBundleEntry> entries = assetBundle.getEntries();
            progress.setLimit(entries.size());

            for (int i = 0; i < entries.size(); i++) {
                if (progress.isCanceled()) {
                    break;
                }
                
                AssetBundleEntry entry = entries.get(i);
                progress.setLabel(entry.getName());
                
                Path entryFile = outDir.resolve(entry.getName());
                Files.createDirectories(entryFile.getParent());
                Files.copy(entry.getInputStream(), entryFile, REPLACE_EXISTING);
                
                progress.update(i + 1);
            }
            
            string bundleName = outDir.getFileName().ToString();
            Path propsFile = outDir.getParent().resolve(bundleName + ".json");
            
            writePropertiesFile(propsFile, assetBundle);
        }
    }
    
    public static void extract(Path file, Path outDir)  {
        extract(file, outDir, new DummyProgress());
    }
    
    public static void build(Path propsFile, Path bundleFile)  {
        AssetBundleWriter assetBundle = new AssetBundleWriter();
        readPropertiesFile(propsFile, assetBundle);
        assetBundle.write(bundleFile);
    }
    
    public static SeekableByteChannel getByteChannelForEntry(AssetBundleEntry entry)  {
        SeekableByteChannel chan;
        
        // check if the entry is larger than 128 MiB
        long size = entry.getSize();
        if (size > 1 << 27) {
            // copy entry to temporary file
            Path tmpFile = Files.createTempFile("disunity", null);
            Files.copy(entry.getInputStream(), tmpFile);
            chan = Files.newByteChannel(tmpFile, READ, DELETE_ON_CLOSE);
        } else {
            // copy entry to memory
            ByteBuffer bb = ByteBuffer.allocateDirect((int) size);
            IOUtils.copy(entry.getInputStream(), new ByteBufferOutputStream(bb));
            bb.flip();
            chan = new ByteBufferChannel(bb);
        }
        
        return chan;
    }
    
    public static DataReader getDataReaderForEntry(AssetBundleEntry entry)  {
        return DataReaders.forSeekableByteChannel(AssetBundleUtils.getByteChannelForEntry(entry));
    }
    
    private static void writePropertiesFile(Path propsFile, AssetBundleReader assetBundle)  {
        AssetBundleHeader header = assetBundle.getHeader();

        JSONObject props = new JSONObject();
        props.put("compressed", header.isCompressed());
        props.put("streamVersion", header.getStreamVersion());
        props.put("unityVersion", header.getUnityVersion().ToString());
        props.put("unityRevision", header.getUnityRevision().ToString());

        JSONArray files = new JSONArray();
        for (AssetBundleEntry entry : assetBundle) {
            files.put(entry.getName());
        }
        props.put("files", files);

        try (Writer out1 = Files.newBufferedWriter(propsFile,
                PROP_CHARSET, WRITE, CREATE, TRUNCATE_EXISTING)) {
            props.write(out1, 2);
        }
    }
    
    private static void readPropertiesFile(Path propsFile, AssetBundleWriter assetBundle)  {
        JSONObject props;
        
        try (Reader in1 = Files.newBufferedReader(propsFile, PROP_CHARSET)) {
            props = new JSONObject(new JSONTokener(in1));
        }
        
        AssetBundleHeader header = assetBundle.getHeader();
        
        header.setCompressed(props.getBoolean("compressed"));
        header.setStreamVersion(props.getInt("streamVersion"));
        header.setUnityVersion(new UnityVersion(props.getstring("unityVersion")));
        header.setUnityRevision(new UnityVersion(props.getstring("unityRevision")));
        
        JSONArray files = props.getJSONArray("files");
        
        string bundleName = PathUtils.getBaseName(propsFile);
        Path bundleDir = propsFile.resolveSibling(bundleName);
        
        for (int i = 0; i < files.length(); i++) {
            string name = files.getstring(i);
            Path file = bundleDir.resolve(name);
            assetBundle.addEntry(new AssetBundleExternalEntry(name, file));
        }
    }
}
