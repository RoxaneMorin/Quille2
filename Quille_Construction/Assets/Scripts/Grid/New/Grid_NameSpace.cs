using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace proceduralGrid
{
    // ENUMS
    public enum Corners { bottomLeft, bottomRight, topLeft, topRight, none };
    public enum Adjacencies { bl, br, tl, tr, b, t, l, r }; // Useless?



    // EVENTS
    public delegate void GridItemGOClicked(Grid_ItemGO theItem);
    public delegate void GridItemPureClicked(Grid_ItemPure theItem);
    public delegate void HandleClicked(Grid_Handle theHandle);



    // STRUCTS
    [System.Serializable]
    public struct CoordPair : IEnumerable<int>
    {
        // VARIABLES
        public int x;
        public int z;

        // PROPERTIES
        public (int, int) AsTuple { get { return (x, z); } }

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

        // OVERRIDES
        public override string ToString()
        {
            return string.Format("({0}, {1})", x, z);
        }
    }
}