/*
 ** 2015 April 09
 **
 ** The author disclaims copyright to this source code. In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */
/**
 *
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System;

public class UnityGUID
{

    private Guid uuid;

    public Guid getUUID()
    {
        return uuid;
    }

    public void setUUID(Guid uuid)
    {
        this.uuid = uuid;
    }

    public override void read(DataReader in1)
    {
        // read GUID as big-endian
        ByteOrder order = in1.order();
        in1.order(ByteOrder.BIG_ENDIAN);
        long guidMost = in1.readLong();
        long guidLeast = in1.readLong();
        in1.order(order);
        uuid = new Guid(guidMost, guidLeast);
    }

    public override void write(DataWriter out1)
    {
        // write GUID as big-endian
        ByteOrder order = out1.order();
        out1.order(ByteOrder.BIG_ENDIAN);
        out1.writeLong(uuid.getMostSignificantBits());
        out1.writeLong(uuid.getLeastSignificantBits());
        out1.order(order);
    }

    public override string ToString()
    {
        return getUUID().ToString();
    }
}
