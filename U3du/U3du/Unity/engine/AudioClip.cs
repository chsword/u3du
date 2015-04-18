/*
 ** 2014 December 26
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
public class AudioClip : UnityObject {

    public AudioClip(FieldNode obj) :
        base(obj){
    }
    public int getFormat() {
        return object.getSInt32("m_Format");
    }
    
    public AudioType getType() {
        return AudioType.fromOrdinal(object.getSInt32("m_Type"));
    }

    public int getStream() {
        return object.getSInt32("m_Stream");
    }
    
    public ByteBuffer getAudioData() {
        string fieldName;
        
        if (object.getType().getVersion() > 2) {
            // vector m_AudioData
            fieldName = "m_AudioData";
        } else {
            // TypelessData audio data
            fieldName = "audio data";
        }
        
        return object.getChildArrayData(fieldName, ByteBuffer.class);
    }
    
}
