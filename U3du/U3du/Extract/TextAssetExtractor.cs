/*
 ** 2014 December 25
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
public class TextAssetExtractor : AbstractAssetExtractor {
    
  
 
    public override UnityClass getUnityClass() {
        return new UnityClass("TextAsset");
    }

 
    public override void extract(ObjectData objectData)  {
        TextAsset shader = new TextAsset(objectData.getInstance());
        string name = shader.getName();
        ByteBuffer script = shader.getScriptRaw();
        
        string ext;
        string assetType;
        
        if (objectData.getInfo().getUnityClass().getName().Equals("Shader")) {
            assetType = "Shader";
            ext = "shader";
        } else {
            assetType = "Text asset";
            ext = "txt";
        }
        
        // skip empty buffers
        if (ByteBufferUtils.isEmpty(script)) {
            L.log(Level.WARNING, "{0} {1} is empty", new object[]{assetType, name});
            return;
        }
        
        writeFile(name, ext, script);
    }
    
}
