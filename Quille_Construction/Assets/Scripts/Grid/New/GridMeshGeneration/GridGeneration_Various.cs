using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Mathematics;
using MeshGeneration;

namespace ProceduralGrid
{
    // ENUMS
    public enum GridMeshType
    {
        SeparateQuads,
        LinkedQuads
    };


    // INTERFACES
    public interface IGridMeshGenerator : IMeshGenerator
    {
        public int2 Resolution { get; set; }
        public float TileSize { get; set; }
    }
}