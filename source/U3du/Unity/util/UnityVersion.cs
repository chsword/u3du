/*
 ** 2014 February 03
 **
 ** The author disclaims copyright to this source code.  In place of
 ** a legal notice, here is a blessing:
 **    May you do good and not evil.
 **    May you find forgiveness for yourself and forgive others.
 **    May you share freely, never taking more than you give.
 */
 
/**
 * Unity engine version string container.
 * 
 * @author Nico Bergemann <barracuda415 at yahoo.de>
 */

using System;

public class UnityVersion : IComparable<UnityVersion> {
    
    private byte major;
    private byte minor;
    private byte patch;
    private string build;
    private string raw;
    
    public UnityVersion(string version) {
        try {
            major = partFromstring(version.substring(0, 1));
            minor = partFromstring(version.substring(2, 3));
            patch = partFromstring(version.substring(4, 5));
            build = version.substring(5);
        } catch (NumberFormatException | IndexOutOfBoundsException ex) {
            // invalid Format, save raw string
            raw = version;
        }
    }

    private SByte partFromstring(string part)
    {
        if (part.Equals("x")) {
            return -1;
        } else {
            return Byte.valueOf(part);
        }
    }

    private string partTostring(SByte part)
    {
         
        if (part == -1) {
            return "x";
        } else {
            return string.valueOf(part);
        }
    }
    
    public bool isValid() {
        return raw == null;
    }

    public byte getMajor() {
        return major;
    }

    public void setMajor(byte major) {
        this.major = major;
    }

    public byte getMinor() {
        return minor;
    }

    public void setMinor(byte minor) {
        this.minor = minor;
    }

    public byte getPatch() {
        return patch;
    }

    public void setPatch(byte patch) {
        this.patch = patch;
    }

    public string getBuild() {
        return build;
    }

    public void setBuild(string build) {
        this.build = build;
    }
    
        public override  string ToString() {
        if (raw != null) {
            return raw;
        } else {
            return string.Format("%s.%s.%s%s", partTostring(major),
                    partTostring(minor), partTostring(patch), build);
        }
    }
    
        public override  int GetHashCode() {
        if (raw != null) {
            return raw.GetHashCode();
        } else {
            int hash = 5;
            hash = 97 * hash + this.major;
            hash = 97 * hash + this.minor;
            hash = 97 * hash + this.patch;
            hash = 97 * hash + Objects.GetHashCode(this.build);
            return hash;
        }
    }

        public override  bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (GetType() != obj.GetType()) {
            return false;
        }
         UnityVersion other = (UnityVersion) obj;
        if (raw != null) {
            if (!Objects.Equals(this.raw, other.raw)) {
                return false;
            }
        } else {
            if (this.major != other.major) {
                return false;
            }
            if (this.minor != other.minor) {
                return false;
            }
            if (this.patch != other.patch) {
                return false;
            }
            if (!Objects.Equals(this.build, other.build)) {
                return false;
            }
            if (!Objects.Equals(this.raw, other.raw)) {
                return false;
            }
        }
        return true;
    }
   
        public    int CompareTo(UnityVersion that) {
        if (!this.isValid() && !that.isValid()) {
            return this.raw.CompareTo(that.raw);
        }

        if (this.major < that.major) {
            return 1;
        } else if (this.major > that.major) {
            return -1;
        } else {
            if (this.minor < that.minor) {
                return 1;
            } else if (this.minor > that.minor) {
                return -1;
            } else {
                if (this.patch < that.patch) {
                    return 1;
                } else if (this.patch > that.patch) {
                    return -1;
                } else {
                    return this.build.CompareTo(that.build);
                }
            }
        }
    }

    public bool lesserThan(UnityVersion that) {
        return this.CompareTo(that) == 1;
    }
    
    public bool greaterThan(UnityVersion that) {
        return this.CompareTo(that) == -1;
    }
}
