/*
 ** 2014 October 06
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
public class TextAsset : UnityObject {

    public TextAsset(FieldNode obj) {
        base(obj);
    }
    
    public string getPathName() {
        return object.getstring("m_PathName");
    }
    
    public string getScript() {
        return object.getstring("m_Script");
    }
    
    public ByteBuffer getScriptRaw() {
        return object.getChildArrayData("m_Script", ByteBuffer.class);
    }
}
