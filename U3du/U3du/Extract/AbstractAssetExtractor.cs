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
public abstract class AbstractAssetExtractor : AssetExtractor {
    
     
    private string outputDirectory;
    
    public abstract UnityClass getUnityClass();

   public abstract void extract(ObjectData objectData);
    public bool isEligible(ObjectData objectData) {
        return objectData.getInfo().getUnityClass().Equals(getUnityClass());
    }

 


    public string getOutputDirectory() {
        return outputDirectory;
    }


    public void setOutputDirectory(string dir)
    {
        outputDirectory = dir;
    }
    
    protected void writeFile(string name, string ext, object data)  {
        string outFile = getOutputDirectory()+(name + "." + ext);
      //  ByteBufferUtils.save(outFile, data);
        
    //    L.log(Level.INFO, "Extracted {0}", outFile.getFileName());
    }
}
