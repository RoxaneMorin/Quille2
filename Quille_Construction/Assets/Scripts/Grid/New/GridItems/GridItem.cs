using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace proceduralGrid
{
    [BurstCompile]
    public struct GridItem
    {
        // VARIABLES
        public GridItemPositioning Positioning;

        public int2 GridCoordinates;
        public float ItemSize;
        public float HeightOffset;

        public float4x4 LocalTransformMatrix;
        public float4x4 WorldTransformMatrix;
        public float3 LocalPosition => new float3(LocalTransformMatrix.c3.x, LocalTransformMatrix.c3.y, LocalTransformMatrix.c3.z);
        public float3 WorldPosition => new float3(WorldTransformMatrix.c3.x, WorldTransformMatrix.c3.y, WorldTransformMatrix.c3.z);

        // Anything else?



        // METHODS
    }
}

