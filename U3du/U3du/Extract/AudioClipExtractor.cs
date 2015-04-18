using U3du.Extract;
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
public class AudioClipExtractor : AbstractAssetExtractor {
    
    
    
    private static  Map<AudioType, string> AUDIO_EXT;

    static AudioClipExtractor(){
        Map<AudioType, string> extMap = new  Map<AudioType, string>
        extMap.put(AudioType.ACC, "aif");
        extMap.put(AudioType.AIFF, "aif");
        extMap.put(AudioType.AUDIOQUEUE, "caf");
        extMap.put(AudioType.GCADPCM, "adp");
        extMap.put(AudioType.MOD, "mod");
        extMap.put(AudioType.MPEG, "mp3");
        extMap.put(AudioType.OGGVORBIS, "ogg");
        extMap.put(AudioType.S3M, "s3m");
        extMap.put(AudioType.WAV, "wav");
        extMap.put(AudioType.XM, "xm");
        extMap.put(AudioType.XMA, "xma");
        AUDIO_EXT = Collections.unmodifiableMap(extMap);
    }

        public override  UnityClass getUnityClass() {
        return new UnityClass("AudioClip");
    }
    
        public override  void extract(ObjectData objectData)  {
        AudioClip audio = new AudioClip(objectData.getInstance());
        string name = audio.getName();
        ByteBuffer audioData = audio.getAudioData();
        
        // skip empty buffers
        if (ByteBufferUtils.isEmpty(audioData)) {
            L.log(Level.WARNING, "Audio clip {0} is empty", name);
            return;
        }
        
        AudioType type = audio.getType();
        string ext = null;
        if (type != null) {
            ext = AUDIO_EXT.get(type);
        }

        // use .bin if the file extension cannot be determined
        if (ext == null) {
            L.log(Level.WARNING, "Audio clip {0} uses unknown audio type {1}",
                    new object[]{name, type});
            ext = "bin";
        }
        
        writeFile(name, ext, audioData);
    }
}
