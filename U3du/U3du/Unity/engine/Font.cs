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
public class Font : UnityObject {

    public Font(FieldNode obj) :
        base(obj){
    }
    
    public ByteBuffer getFontData() {
        return object.getChildArrayData("m_FontData", ByteBuffer.class);
    }
}
