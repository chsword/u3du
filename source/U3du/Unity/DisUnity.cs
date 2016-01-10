
using System;

public class DisUnity {

    public static string getName() {
        return "DisUnity";
    }
    
    public static string getProgramName() {
        return "disunity";
    }
    
    public static string getVersion() {
        return "0.4.0";
    }
    
    public static string getSignature() {
        return string.Format("%s v%s", getName(), getVersion());
    }

    private DisUnity() {
    }
}
