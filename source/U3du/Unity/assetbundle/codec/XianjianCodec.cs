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
 * Codec for Xianjian asset bundles where some bytes are XOR'd with a fixed key.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */
public class XianjianCodec : AssetBundleCodec {
    
    private static  byte[] KEY = DatatypeConverter.parseHexBinary("56F45921699978FC92B3");
    
        public override  string getName() {
        return "Xianjian XOR";
    }

        public override  bool isEncoded(SeekableByteChannel chan)  {
        DataReader in1 = DataReaders.forSeekableByteChannel(chan);
        
        in1.position(1 << 5);
        int b1 = in1.readByte();
        
        in1.position(1 << 6);
        int b2 = in1.readByte();
        
        return b1 == KEY[5] && b2 == KEY[6];
    }
    
    private void code(SeekableByteChannel chan)  {
        DataReader in1 = DataReaders.forSeekableByteChannel(chan);
        DataWriter out1 = DataWriters.forSeekableByteChannel(chan);
        
        for (int exp = 5; 1 << exp < in1.size(); exp++) {
            int offset = 1 << exp;
            in1.position(offset);
            int b = in1.readByte();
            b ^= KEY[exp % KEY.length];
            out1.position(offset);
            out1.writeUnsignedByte(b);
        }
    }

        public override  void encode(SeekableByteChannel chan)  {
        code(chan);
    }

        public override  void decode(SeekableByteChannel chan)  {
        code(chan);
    }
    
}
