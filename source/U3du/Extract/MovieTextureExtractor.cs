/*
 ** 2014 December 27
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
public class MovieTextureExtractor : AbstractAssetExtractor {
    
 
    public override UnityClass getUnityClass() {
        return new UnityClass("MovieTexture");
    }


    public override void extract(ObjectData objectData)
    {
        MovieTexture mtex = new MovieTexture(objectData.getInstance());
        string name = mtex.getName();
        ByteBuffer movieData = mtex.getMovieData();
        
        // skip empty buffers
        if (ByteBufferUtils.isEmpty(movieData)) {
            L.log(Level.WARNING, "Movie texture clip {0} is empty", name);
            return;
        }
        
        string fourCC;
        byte[] fourCCRaw = new byte[4];
        string ext;
        
        movieData.rewind();
        movieData.get(fourCCRaw);
        
        fourCC = new string(fourCCRaw, "ASCII");
        
        switch (fourCC) {
            case "OggS":
                ext = "ogv";
                break;
                
            default:
                ext = "mov";
                L.log(Level.WARNING, "Unrecognized movie fourCC \"{0}\"", fourCC);
        }
        
        writeFile(name, ext, movieData);
    }
}
