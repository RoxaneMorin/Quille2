using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    // ENUMS
    public enum Adjacencies { bl, br, tl, tr, b, t, l, r }; // Useless?


    // STRUCTS
    [System.Serializable]
    public struct CoordPair : IEnumerable<int>
    {
        // VARIABLES
        public int x;
        public int z;

        // CONSTRUCTOR
        public CoordPair(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        // ENUMERATOR
        public IEnumerator<int> GetEnumerator()
        {
            yield return x;
            yield return z;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // OVERRIDE
        public override string ToString()
        {
            return string.Format("CoordPair : ({0}, {1}.)", x, z);
        }
    }
}