using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGridOld
{
    // ENUMS
    public enum Adjacencies { bl, br, tl, tr, b, t, l, r };


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

    [System.Serializable]
    public struct FourAdjacentSquares : IEnumerable<Grid_Square>
    {
        // VARIABLES
        [InspectorName("Bottom Left")] public Grid_Square bl;
        [InspectorName("Bottom Right")] public Grid_Square br;
        [InspectorName("Top Left")] public Grid_Square tl;
        [InspectorName("Top Right")] public Grid_Square tr;

        // CONSTRUCTOR
        public FourAdjacentSquares(Grid_Square bl, Grid_Square br, Grid_Square tl, Grid_Square tr)
        {
            this.bl = bl;
            this.br = br;
            this.tl = tl;
            this.tr = tr;
        }

        // ENUMERATOR
        public IEnumerator<Grid_Square> GetEnumerator()
        {
            yield return bl;
            yield return br;
            yield return tl;
            yield return tr;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // OVERRIDES
        public override string ToString()
        {
            return string.Format("AdjacentGridSquares : bottom left: {0}, bottom right: {1}, top left: {2}, top right: {3}.", bl, br, tl, tr);
        }
    }

    [System.Serializable]
    public struct EightAdjacentSquares : IEnumerable<Grid_Square>
    {
        // VARIABLES
        [InspectorName("Bottom Left")] public Grid_Square bl;
        [InspectorName("Bottom")] public Grid_Square b;
        [InspectorName("Bottom Right")] public Grid_Square br;
        [InspectorName("Top Left")] public Grid_Square tl;
        [InspectorName("Top")] public Grid_Square t;
        [InspectorName("Top Right")] public Grid_Square tr;
        [InspectorName("Left")] public Grid_Square l;
        [InspectorName("Right")] public Grid_Square r;

        // CONSTRUCTOR
        public EightAdjacentSquares(Grid_Square bl, Grid_Square b, Grid_Square br, Grid_Square tl, Grid_Square t, Grid_Square tr, Grid_Square l, Grid_Square r)
        {
            this.bl = bl;
            this.b = b;
            this.br = br;
            this.tl = tl;
            this.t = t;
            this.tr = tr;
            this.l = l;
            this.r = r;
        }

        // ENUMERATOR
        public IEnumerator<Grid_Square> GetEnumerator()
        {
            yield return bl;
            yield return b;
            yield return br;
            yield return tl;
            yield return t;
            yield return tr;
            yield return l;
            yield return r;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // OVERRIDES
        public override string ToString()
        {
            return string.Format("AdjacentGridSquares : bottom left: {}, bottom: {}, bottom right: {}, left: {}, right: {}, top left: {}, top: {}, top right: {}.", bl, b, br, l, r, tl, t, tr);
        }
    }
}