using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U3du.Extract
{
    internal class Map<T1, T2> : Dictionary<T1, T2>
    {
        public void put(T1 key, T2 value)
        {
            base.Add(key, value);
        }
    }
}
