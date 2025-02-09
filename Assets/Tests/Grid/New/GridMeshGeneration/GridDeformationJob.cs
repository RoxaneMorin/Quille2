using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


namespace proceduralGrid
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct GridDeformationJob<S> : IJobFor
        where S : struct, IMeshStreams
    {
        [WriteOnly] S streams;



        [BurstCompile]
        public void Execute(int index)
        {

        }



        public static JobHandle ScheduleParallel(Mesh inputMesh, JobHandle dependency)
        {
            // TODO: Move this out so the memory can be properly deallocated.

            // Acquire source mesh data.
            Mesh.MeshDataArray inputMeshDataArray = Mesh.AcquireReadOnlyMeshData(inputMesh);
            Mesh.MeshData inputMeshData = inputMeshDataArray[0];

            // Prepare writeable mesh data.
            Mesh.MeshDataArray outputMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData outputMeshData = outputMeshDataArray[0];

           

            // TODO: customisable type of stream?
            // Rebuild the vertex attribute descriptor? Pass it as a parameter?

            outputMeshData.SetVertexBufferParams(inputMeshData.vertexCount, SingleStream.GetVertexAttributeDescriptor());
            outputMeshData.SetIndexBufferParams(inputMeshData.GetSubMesh(0).indexCount, inputMeshData.indexFormat);

            NativeArray<SingleStream.SingleStreamVertex> vertexData = outputMeshData.GetVertexData<SingleStream.SingleStreamVertex>();

            // Use the streams that already exists instead?
            var job = new GridDeformationJob<S>();
            job.streams.Setup(outputMeshData, inputMesh.bounds, inputMeshData.vertexCount, inputMeshData.GetSubMesh(0).indexCount);


            return job.ScheduleParallel(1, 1, dependency);
        }
    }

    //public delegate JobHandle GridMeshJobScheduleDelegate(Mesh mesh, Mesh.MeshData meshData, int2 resolution, float tileSize, JobHandle dependency);


    //private void UpdateMesh(Mesh.MeshData meshData)
    //{
    //    // Get a reference to the index data and fill it from the input mesh data
    //    var outputIndexData = meshData.GetIndexData<ushort>();
    //    _meshDataArray[0].GetIndexData<ushort>().CopyTo(outputIndexData);
    //    // According to docs calling Mesh.AcquireReadOnlyMeshData
    //    // does not cause any memory allocations or data copies by default, as long as you dispose of the MeshDataArray before modifying the Mesh
    //    _meshDataArray.Dispose();
    //    meshData.subMeshCount = 1;
    //    meshData.SetSubMesh(0,
    //        _subMeshDescriptor,
    //        MeshUpdateFlags.DontRecalculateBounds |
    //        MeshUpdateFlags.DontValidateIndices |
    //        MeshUpdateFlags.DontResetBoneBounds |
    //        MeshUpdateFlags.DontNotifyMeshUsers);
    //    Mesh.MarkDynamic();
    //    Mesh.ApplyAndDisposeWritableMeshData(
    //        _meshDataArrayOutput,
    //        Mesh,
    //        MeshUpdateFlags.DontRecalculateBounds |
    //        MeshUpdateFlags.DontValidateIndices |
    //        MeshUpdateFlags.DontResetBoneBounds |
    //        MeshUpdateFlags.DontNotifyMeshUsers);
    //    Mesh.RecalculateNormals();
    //}
}

