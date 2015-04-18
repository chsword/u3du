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
public class FontExtractor : AbstractAssetExtractor
{

    public override UnityClass getUnityClass()
    {
        return new UnityClass("Font");
    }

    public override void extract(ObjectData objectData)
    {
        Font font = new Font(objectData.getInstance());
        ByteBuffer fontData = font.getFontData();

        if (ByteBufferUtils.isEmpty(fontData))
        {
            // don't write log message, this seems to be quite common
            return;
        }

        writeFile(font.getName(), "ttf", fontData);
    }
}
