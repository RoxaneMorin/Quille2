using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using MeshGeneration;

namespace ProceduralGrid
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct GridGenerationJob<G, S> : IJobFor
        where G : struct, IGridMeshGenerator
        where S : struct, IMeshStreams
    {
        G generator;
        [WriteOnly] S streams;

        public void Execute(int i) => generator.Execute(i, streams);

        public static JobHandle ScheduleParallel(Mesh mesh, Mesh.MeshData meshData, int2 resolution, float tileSize, JobHandle dependency)
        {
            var job = new GridGenerationJob<G, S>();
            job.generator.Resolution = resolution;
            job.generator.TileSize = tileSize;

            job.streams.Setup(meshData, mesh.bounds = job.generator.Bounds, job.generator.VertexCount, job.generator.IndexCount);

            return job.ScheduleParallel(job.generator.JobLength, 1, dependency);
        }
    }

    public delegate JobHandle GridMeshJobScheduleDelegate(Mesh mesh, Mesh.MeshData meshData, int2 resolution, float tileSize, JobHandle dependency);
} 

