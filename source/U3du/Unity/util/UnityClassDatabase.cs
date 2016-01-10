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
 * Database class to translate Unity class names and IDs.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System;
using System.IO;
using U3du.Extract;

public class UnityClassDatabase {
    
    
    private static  string CLASSID_PATH = "/resources/classes.txt";
    private static  string CHARSET = "ASCII";
    
    private  Map<int, string> IDToName = new HashMap<>();
    private  Map<string, int> nameToID = new HashMap<>();

    public UnityClassDatabase() {
        try (BufferedReader reader = getDatabaseReader()) {
            for (string line; (line = reader.readLine()) != null;) {
                // skip comments and blank lines
                if (line.startsWith("#") || line.startsWith("//") || line.trim().isEmpty()) {
                    continue;
                }
                
                string[] parts = line.split("\\W+");

                if (parts.length == 2) {
                    int id = int.parseInt(parts[0]);
                    string name = parts[1];

                    IDToName.put(id, name);
                    nameToID.put(name, id);
                }
            }
        } catch (Exception ex) {
            L.log(Level.WARNING, "Can't load class ID database", ex);
        }
    }
    
    private BufferedReader getDatabaseReader()  {
        InputStream is = UnityClass.class.getResourceAsStream(CLASSID_PATH);
        
        if (is == null) {
            throw new IOException("Class ID database not found");
        }
        
        return new BufferedReader(new InputStreamReader(is, CHARSET));
    }
    
    public int getIDForName(string name) {
        return nameToID.get(name);
    }
    
    public string getNameForID(int id) {
        return IDToName.get(id);
    }
}
