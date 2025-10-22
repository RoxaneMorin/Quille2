using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace ProceduralGrid
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct GridItemWorldMatrixUpdateJob : IJobParallelFor
    {
        // PROPERTIES
        public NativeArray<GridItem> GridItems;
        [ReadOnly] public float4x4 ParentTransformMatrix;

        public int JobLength { get { return GridItems.Length; } }



        // METHODS
        public void Execute(int index)
        {
            // Is there a way to do this without recreating the item?

            GridItem previousGridItem = GridItems[index];
            GridItems[index] = new GridItem
            {
                Positioning = previousGridItem.Positioning,

                GridCoordinates = previousGridItem.GridCoordinates,
                ItemSize = previousGridItem.ItemSize,
                HeightOffset = previousGridItem.HeightOffset,

                LocalTransformMatrix = previousGridItem.LocalTransformMatrix,
                WorldTransformMatrix = math.mul(ParentTransformMatrix, previousGridItem.LocalTransformMatrix)
            };
        }

        public static JobHandle Schedule(NativeArray<GridItem> gridItems, float4x4 parentTransformMatrix, JobHandle dependency)
        {
            var job = new GridItemWorldMatrixUpdateJob();
            job.GridItems = gridItems;
            job.ParentTransformMatrix = parentTransformMatrix;

            return job.Schedule(job.JobLength, 64, dependency);
        }
    }
}