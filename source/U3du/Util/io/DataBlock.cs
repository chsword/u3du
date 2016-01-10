/*
 ** 2014 October 28
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

using System;

public class DataBlock {
    
    private long offset;
    private long length;

    public long getOffset() {
        return offset;
    }

    public void setOffset(long offset) {
        this.offset = offset;
    }

    public long getLength() {
        return length;
    }

    public void setLength(long length) {
        this.length = length;
    }
    
    public void setEndOffset(long endOffset) {
        this.length = endOffset - offset;
    }
    
    public long getEndOffset() {
        return offset + length;
    }
    
    public bool isIntersecting(DataBlock that) {
        return this.getEndOffset() > that.getOffset() && that.getEndOffset() > this.getOffset();
    }
    
    public bool isInside(DataBlock that) {
        return this.getOffset() >= that.getOffset() && this.getEndOffset() <= that.getEndOffset();
    }
    
    public void markBegin(Positionable p)  {
        setOffset(p.position());
    }
    
    public void markEnd(Positionable p)  {
        setEndOffset(p.position());
    }
    
 
    public override string ToString() {
        return getOffset() + " - " + getEndOffset() + " (" + getLength() + ")";
    }
}
