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
 * Unity class ID container.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */
public class UnityClass {
    
    private static  UnityClassDatabase DB = new UnityClassDatabase();

    private  int id;
    
    public UnityClass(int id) {
        this.id = id;
    }
    
    public UnityClass(string name) {
        int id = DB.getIDForName(name);
        
        // the ID must be valid
        if (id == null) {
            throw new IllegalArgumentException("Unknown class name: " + name);
        }
        
        this.id = id;
    }
    
    public int getID() {
        return id;
    }
    
    public string getName() {
        return DB.getNameForID(id);
    }

        public override  int GetHashCode() {
        return 78 * id;
    }

        public override  bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (GetType() != obj.GetType()) {
            return false;
        }
         UnityClass other = (UnityClass) obj;
        return this.id == other.id;
    }

        public override  string ToString() {
        stringBuilder sb = new stringBuilder();
        sb.append("Class ");
        sb.append(id);
        
        string name = getName();
        if (name != null) {
            sb.append(" (");
            sb.append(name);
            sb.append(")");
        }
        
        return sb.ToString();
    }
}
