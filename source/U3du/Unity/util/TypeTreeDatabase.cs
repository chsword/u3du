/*
 ** 2014 December 22
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
public class TypeTreeDatabase {
    
    
    
    public static  int VERSION = 1;
    public static  string FILENAME = "types.dat";
    
    private  Map<Pair<UnityClass, UnityVersion>, FieldTypeNode> nodeMap = new HashMap<>();
    private Path dbFile;
    
    public TypeTreeDatabase() {
        // get database path based on the path to the current .jar file
        dbFile = PathUtils.getCodeSourceLocation(GetType());
        
        if (dbFile != null) {
            dbFile = dbFile.resolveSibling(FILENAME);
        }
    }
    
    private InputStream getDatabaseInputStream()  {
        // read database file, external or internal otherwise
        InputStream is;
        
        if (dbFile != null && Files.exists(dbFile)) {
            is = Files.newInputStream(dbFile);
        } else {
            is = GetType().getResourceAsStream("/resources/" + FILENAME);
        }
        
        if (is == null) {
            throw new IOException("Type database file not found");
        }
        
        return is;
    }
    
    public void load() {
        L.fine("Loading type database");
        
        try (InputStream is = getDatabaseInputStream()) {
            load(is);
        } catch (IOException ex) {
            L.log(Level.SEVERE, "Can't open type database", ex);
        }
    }
    
    public void load(InputStream is) {
        try (DataReader in1 = DataReaders.forInputStream(is)) {
            // read header
            int dbVersion = in1.readInt();

            if (dbVersion != VERSION) {
                throw new RuntimeException("Wrong database version");
            }

            // read field node table
            int fieldNodeSize = in1.readInt();
            List<FieldTypeNode> fieldNodes = new ArrayList<>(fieldNodeSize);

            for (int i = 0; i < fieldNodeSize; i++) {
                FieldTypeNode fieldNode = new FieldTypeNode();
                fieldNode.read(in1);
                fieldNodes.add(fieldNode);
            }

            // read version string table
            int versionSize = in1.readInt();
            List<UnityVersion> versions = new ArrayList<>(versionSize);

            for (int i = 0; i < versionSize; i++) {
                versions.add(new UnityVersion(in1.readstringNull()));
            }

            // read mapping data
            int fieldNodeKeySize = in1.readInt();

            for (int i = 0; i < fieldNodeKeySize; i++) {
                int index = in1.readInt();
                int classID = in1.readInt();
                int versionIndex = in1.readInt();
                
                UnityVersion version = versions.get(versionIndex);
                UnityClass uclass = new UnityClass(classID);
                FieldTypeNode fieldNode = fieldNodes.get(index);

                nodeMap.put(new ImmutablePair<>(uclass, version), fieldNode);
            }
        } catch (IOException ex) {
            L.log(Level.SEVERE, "Can't read type database", ex);
        }
    }
    
    public void save() {
        L.fine("Saving type database");
        
        try (
            OutputStream os = Files.newOutputStream(dbFile, WRITE, CREATE, TRUNCATE_EXISTING)
        ) {
            save(os);
        } catch (IOException ex) {
            L.log(Level.SEVERE, "Can't open type database", ex);
        }
    }
    
    public void save(OutputStream os) {
        // write database file
        try (DataWriter out1 = DataWriters.forOutputStream(new BufferedOutputStream(os))) {
            // write header
            out1.writeInt(VERSION);

            // write field node table
            Set<FieldTypeNode> fieldNodes = new HashSet<>(nodeMap.values());
            Map<FieldTypeNode, int> fieldNodeMap = new HashMap<>();

            out1.writeInt(fieldNodes.size());

            int index = 0;
            for (FieldTypeNode fieldNode : fieldNodes) {
                fieldNodeMap.put(fieldNode, index++);
                fieldNode.write(out1);
            }

            // write version string table
            Set<UnityVersion> versions = new HashSet<>();
            Map<UnityVersion, int> versionMap = new HashMap<>();

            for (Map.Entry<Pair<UnityClass, UnityVersion>, FieldTypeNode> entry : nodeMap.entrySet()) {
                versions.add(entry.getKey().getRight());
            }

            out1.writeInt(versions.size());

            index = 0;
            for (UnityVersion version : versions) {
                versionMap.put(version, index++);
                out1.writestringNull(version.ToString());
            }

            // write mapping data
            out1.writeInt(nodeMap.entrySet().size());

            for (Map.Entry<Pair<UnityClass, UnityVersion>, FieldTypeNode> entry : nodeMap.entrySet()) {
                index = fieldNodeMap.get(entry.getValue());
                Pair<UnityClass, UnityVersion> fieldNodeKey = entry.getKey();

                int classID = fieldNodeKey.getLeft().getID();
                UnityVersion version = fieldNodeKey.getRight();

                out1.writeInt(index);
                out1.writeInt(classID);
                out1.writeInt(versionMap.get(version));
            }
        } catch (IOException ex) {
            L.log(Level.SEVERE, "Can't write type database", ex);
        }
    }
    
    public Map<Pair<UnityClass, UnityVersion>, FieldTypeNode> getTypeMap() {
        return nodeMap;
    }
}
