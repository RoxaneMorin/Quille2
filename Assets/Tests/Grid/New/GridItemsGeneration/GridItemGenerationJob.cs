using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace proceduralGrid
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct GridCenterItemGenerationJob : IJobParallelFor
    {
        // PROPERTIES
        public NativeArray<GridItem> GridItems;

        [ReadOnly] public int2 Resolution;
        [ReadOnly] public float TileSize;
        [ReadOnly] public float4x4 ParentTransformMatrix;

        public GridItemPositioning ItemPositioning { get { return GridItemPositioning.AtCenter; } }



        // METHODS
        public void Execute(int index)
        {
            int x = index % Resolution.x;
            int z = index / Resolution.x;

            // Determine item position,
            float posX = (0.5f + x) * TileSize;
            float posZ = (0.5f + z) * TileSize;

            // Build local matrix
            float4x4 localMatrix = new float4x4(Unity.Mathematics.float3x3.identity, new float3(posX, 0.01f, posZ));

            // Build world matrix
            float4x4 worldMatrix = math.mul(ParentTransformMatrix, localMatrix);

            // Create the item at index.
            GridItems[index] = new GridItem
            {
                Positioning = ItemPositioning,

                GridCoordinates = new int2(x, z),
                TileSize = TileSize,
                HeightOffset = 0f,

                LocalTransformMatrix = localMatrix,
                WorldTransformMatrix = worldMatrix
            };
        }
    }


    // Also have a job to edit the item's transform matrix & other info.
}

