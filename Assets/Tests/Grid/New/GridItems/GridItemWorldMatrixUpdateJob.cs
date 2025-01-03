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
    public struct GridItemWorldMatrixUpdateJob : IJobParallelFor
    {
        // PROPERTIES
        public NativeArray<GridItem> GridItems;
        [ReadOnly] public float4x4 ParentTransformMatrix;

        public int JobLength { get { return GridItems.Length; } }



        // METHODS
        public void Execute(int index)
        {
            GridItems[index].UpdateWorldTransformMatrix(ParentTransformMatrix);
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