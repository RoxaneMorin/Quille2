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
    public struct GridItemGenerationJob : IJobParallelFor
    {
        // PROPERTIES
        public NativeArray<GridItem> GridItems;
        public GridItemPositioning ItemPositioning;

        [ReadOnly] public int2 Resolution;
        [ReadOnly] public float TileSize;
        [ReadOnly] public float4x4 ParentTransformMatrix;

        public int JobLength { get { return GridItems.Length; } }



        // METHODS
        public void Execute(int index)
        {
            int x = index % Resolution.x;
            int z = index / Resolution.x;

            // Determine item position,
            float posX = (ItemPositioning == GridItemPositioning.AtCenter ? (0.5f + x) * TileSize : x * TileSize);
            float posZ = (ItemPositioning == GridItemPositioning.AtCenter ? (0.5f + z) * TileSize : z * TileSize);

            // Build local matrix
            float4x4 localMatrix = new float4x4(Unity.Mathematics.float3x3.identity, new float3(posX, 0.01f, posZ));

            // Create the item at index.
            GridItems[index] = new GridItem
            {
                Positioning = ItemPositioning,

                GridCoordinates = new int2(x, z),
                TileSize = TileSize,
                HeightOffset = 0f,

                LocalTransformMatrix = localMatrix,
                WorldTransformMatrix = math.mul(ParentTransformMatrix, localMatrix)
            };
        }

        public static JobHandle Schedule(NativeArray<GridItem> gridItems, GridItemPositioning itemPositioning, int2 resolution, float tileSize, float4x4 parentTransformMatrix, JobHandle dependency)
        {
            var job = new GridItemGenerationJob();
            job.GridItems = gridItems;
            job.ItemPositioning = itemPositioning;
            job.Resolution = resolution;
            job.TileSize = tileSize;
            job.ParentTransformMatrix = parentTransformMatrix;

            return job.Schedule(job.JobLength, 64, dependency);
        }
    }
}

