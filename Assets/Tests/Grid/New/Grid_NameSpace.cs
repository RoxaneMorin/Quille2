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

        // OVERRIDES
        public override string ToString()
        {
            return string.Format("({0}, {1})", x, z);
        }
    }


    [System.Serializable]
    public class TransformMatrix
    {
        // VARIABLES
        protected Matrix4x4 matrix;


        // PROPERTIES
        public Matrix4x4 Matrix { get { return matrix; } set { matrix = value; } }
        public Vector3 Position { get { return new Vector3(matrix.m03, matrix.m13, matrix.m23); } }


        // CONSTRUCTOR
        public TransformMatrix(Matrix4x4 sourceMatrix)
        {
            matrix = sourceMatrix;
        }
    }
}