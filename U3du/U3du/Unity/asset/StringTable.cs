/*
 ** 2015 April 12
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

using U3du.Extract;

public class stringTable {
    
    private static  int INTERNAL_FLAG = 1 << 31;
   
    private  Map<int, string> strings = new  Map<int, string>();
    
    public stringTable()  {
        byte[] data;
        try (InputStream is = GetType().getResourceAsStream("/resources/strings.dat")) {
            data = IOUtils.toByteArray(is);
        }
        loadstrings(data, true);
    }
    
    public stringTable(byte[] data)  {
        loadstrings(data, false);
    }

    private void loadstrings(byte[] data, bool internal) {
        for (int i = 0, n = 0; i < data.length; i++) {
            if (data[i] == 0) {
                string string = new string(data, n, i - n, StandardCharsets.US_ASCII);
                int offset = n;
                if (internal) {
                    offset |= INTERNAL_FLAG;
                }
                
                n = i + 1;
                
                strings.put(offset, string);
            }
        }
    }
    
    public void loadstrings(byte[] data) {
        loadstrings(data, false);
    }
    
    public string getstring(int offset) {
        return strings.get(offset);
    }
}
