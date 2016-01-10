/*
 ** 2014 July 01
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */

/**
 * Inteface for asset bundle codecs.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */
public interface AssetBundleCodec {

    public string getName();

    public bool isEncoded(SeekableByteChannel chan) ;

    public void encode(SeekableByteChannel chan) ;

    public void decode(SeekableByteChannel chan) ;
}
