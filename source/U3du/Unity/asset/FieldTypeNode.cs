/*
 ** 2014 September 22
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
public class FieldTypeNode : Node<FieldTypeNode>  {
    
    private FieldType type = new FieldType();

    public FieldType getType() {
        return type;
    }

    public void setType(FieldType field) {
        this.type = field;
    }

        public override  void read(DataReader in1)  {
        type.read(in1);
        
        int numChildren = in1.readInt();
        for (int i = 0; i < numChildren; i++) {
            FieldTypeNode child = new FieldTypeNode();
            child.read(in1);
            add(child);
        }
    }

        public override  void write(DataWriter out1)  {
        type.write(out1);
        
        int numChildren = size();
        out1.writeInt(numChildren);
        for (FieldTypeNode child : this) {
            child.write(out1);
        }
    }

        public override  int GetHashCode() {
        int hash = base.GetHashCode();
        hash = 97 * hash + Objects.GetHashCode(this.type);
        return hash;
    }

        public override  bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (GetType() != obj.GetType()) {
            return false;
        }
        if (!base.Equals(obj)) {
            return false;
        }
         FieldTypeNode other = (FieldTypeNode) obj;
        if (!Objects.Equals(this.type, other.type)) {
            return false;
        }
        return true;
    }

}
