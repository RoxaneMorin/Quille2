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

        [ReadOnly] public int2 GridResolution;
        [ReadOnly] public float TileSize;
        [ReadOnly] public float ItemSize;
        [ReadOnly] public float3 ItemOffset;
        [ReadOnly] public float4x4 ParentTransformMatrix;

        public int JobLength { get { return GridItems.Length; } }



        // METHODS
        public void Execute(int index)
        {
            int x = index % GridResolution.x;
            int z = index / GridResolution.x;

            // Determine item position,
            float posX = (ItemPositioning == GridItemPositioning.AtCenter ? (0.5f + x + ItemOffset.x) * TileSize : (x + ItemOffset.x) * TileSize);
            float posZ = (ItemPositioning == GridItemPositioning.AtCenter ? (0.5f + z + ItemOffset.z) * TileSize : (z + ItemOffset.z) * TileSize);

            // Build local matrix
            float4x4 localMatrix = new float4x4(Unity.Mathematics.float3x3.identity, new float3(posX, ItemOffset.y, posZ));

            // Create the item at index.
            GridItems[index] = new GridItem
            {
                Positioning = ItemPositioning,

                GridCoordinates = new int2(x, z),
                ItemSize = ItemSize,
                HeightOffset = ItemOffset.y,

                LocalTransformMatrix = localMatrix,
                WorldTransformMatrix = math.mul(ParentTransformMatrix, localMatrix)
            };
        }

        public static JobHandle Schedule(NativeArray<GridItem> gridItems, GridItemPositioning itemPositioning, int2 resolution, float tileSize, float itemSize, float3 itemOffset, float4x4 parentTransformMatrix, JobHandle dependency)
        {
            var job = new GridItemGenerationJob();
            job.GridItems = gridItems;
            job.ItemPositioning = itemPositioning;
            job.GridResolution = resolution;
            job.TileSize = tileSize;
            job.ItemSize = itemSize;
            job.ItemOffset = itemOffset;
            job.ParentTransformMatrix = parentTransformMatrix;

            return job.Schedule(job.JobLength, 64, dependency);
        }
    }
}

