using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using MeshGeneration;

namespace Building
{
    //[System.Serializable]
    public class WallSegment : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private WallAnchor anchorA;
        [SerializeField] private WallAnchor anchorB;
        // These should never be null.

        // TODO: also include the WallConnections?

        [SerializeField] private float thickness;
        // Height at both anchors?
        // Should those switch if we switch up the anchors?
        // Some info on both 'sides'?
        // Should it depend on the wall direction?

        [Header("Resources")]
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;



        // PROPERTIES AND OTHER ACCESSORS
        public WallAnchor AnchorA
        {
            get { return anchorA; }
            set
            {
                if (value && value != anchorB)
                {
                    anchorA = value;
                }
            }
        }
        public WallAnchor AnchorB
        {
            get { return anchorB; }
            set
            {
                if (value && value != anchorA)
                {
                    anchorB = value;
                }
            }
        }
        public WallAnchor OtherAnchor(WallAnchor sourceAnchor)
        {
            if (sourceAnchor == anchorA)
            {
                return anchorB;
            }
            else if (sourceAnchor == anchorB)
            {
                return anchorA;
            }
            else
            {
                Debug.LogError(string.Format("{0} is not one of this wall's anchors.", sourceAnchor));
                return null;
            }
        }

        public WallConnection ConnectionA { get { return anchorA.GetConnectionTo(anchorA); } }
        public WallConnection ConnectionB { get { return anchorB.GetConnectionTo(anchorB); } }

        public Vector3 AnchorAGroundPosition { get { return AnchorA.GroundPosition; } }
        public Vector3 AnchorATopPosition { get { return AnchorA.TopPosition; } }
        public Vector3 AnchorBGroundPosition { get { return AnchorB.GroundPosition; } }
        public Vector3 AnchorBTopPosition { get { return AnchorB.TopPosition; } }

        public float3 AnchorAGroundPositionF { get { return AnchorA.GroundPositionF; } }
        public float3 AnchorATopPositionF { get { return AnchorA.TopPositionF; } }
        public float3 AnchorBGroundPositionF { get { return AnchorB.GroundPositionF; } }
        public float3 AnchorBTopPositionF { get { return AnchorB.TopPositionF; } }

        public float Thickness
        {
            get { return thickness; }
            set
            {
                if (value < 0)
                {
                    thickness = 0;
                }
                else
                {
                    thickness = value;
                }

                GenerateWallMesh();
            }
        }

        public Vector3 Normal
        {
            get
            {
                return MathHelpers.CalculateFaceNormal(AnchorAGroundPosition, AnchorBGroundPosition, AnchorBTopPosition);
            }
        }

        public Vector3 CenterPos
        {
            get
            {
                Vector3 centerPosA = (AnchorAGroundPosition + AnchorATopPosition) / 2f;
                Vector3 centerPosB = (AnchorBGroundPosition + AnchorBTopPosition) / 2f;
                return Vector3.Lerp(centerPosA, centerPosB, 0.5f);
            }
        }





        // METHODS

        // CONSTRUCTOR
        public WallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            this.anchorA = anchorA;
            this.anchorB = anchorB;
        }

        // INIT
        public void Init(WallAnchor anchorA, WallAnchor anchorB)
        {
            // Name the game object.
            gameObject.name = string.Format("WallSegment ({0} <-> {1})", anchorA, anchorB);

            // Set the anchor references.
            this.anchorA = anchorA;
            this.anchorB = anchorB;

            // Fetch the mesh components and generate the mesh.
            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshCollider = gameObject.GetComponent<MeshCollider>();

            GenerateWallMesh();
        }


        // UTILITY
        public void InverseWallDirection()
        {
            // TODO: should we change smt inside the wall anchors too?

            WallAnchor tempAnchor = anchorA;
            anchorA = anchorB;
            anchorB = tempAnchor;
        }




        // MESH GENERATION

        /*
         * TODO:
         * - Keep track of the wall 'sides' locations
         * 
         * - Account for intersecting walls
         * 
         * - Editor stuff with tools to change thickness and the like.
         */

        public void GenerateWallMesh()
        {
            float3 anchorAGroundPos = AnchorAGroundPositionF;
            float3 anchorBGroundPos = AnchorBGroundPositionF;
            float3 anchorATopPos = AnchorATopPositionF;
            float3 anchorBTopPos = AnchorBTopPositionF;

            // Generate the main mesh.
            Mesh wallMesh;
            if (thickness > 0f)
            {
                wallMesh = GenerateThickWallMesh(anchorAGroundPos, anchorATopPos, anchorBGroundPos, anchorBTopPos, Thickness, Normal);
            }
            else
            {
                wallMesh = GenerateFlatWallMesh(anchorAGroundPos, anchorATopPos, anchorBGroundPos, anchorBTopPos);
                // TODO: test if we can reuse this mesh for the collider.
            }

            // Generate the collider mesh.
            Mesh colliderMesh = GenerateWallColliderMesh(anchorAGroundPos, anchorATopPos, anchorBGroundPos, anchorBTopPos);

            // Generate bounds.
            Bounds wallBounds = GenerateWallMeshBounds(anchorAGroundPos, anchorATopPos, anchorBGroundPos, anchorBTopPos);

            // Set!
            wallMesh.bounds = wallBounds;

            meshFilter.mesh = wallMesh;
            meshCollider.sharedMesh = colliderMesh;
        }




        // STATIC METHODS

        private static Mesh GenerateThickWallMesh(float3 anchorAPos, float3 anchorATopPos, float3 anchorBPos, float3 anchorBTopPos, float thickness, float3 normal)
        {
            // Calculate displacement based on wall thickness and normal.
            float3 halfDistance = normal * (thickness / 2);

            // Calculate the displaced points
            float3 anchorAPosMin = anchorAPos - halfDistance;
            float3 anchorBPosMin = anchorBPos - halfDistance;
            float3 anchorATopPosMin = anchorATopPos - halfDistance;
            float3 anchorBTopPosMin = anchorBTopPos - halfDistance;

            float3 anchorAPosPlus = anchorAPos + halfDistance;
            float3 anchorBPosPlus = anchorBPos + halfDistance;
            float3 anchorATopPosPlus = anchorATopPos + halfDistance;
            float3 anchorBTopPosPlus = anchorBTopPos + halfDistance;

            // Bounds
            //Bounds wallBounds = GenerateWallMeshBounds(anchorAPos, anchorATopPos, anchorAHeight, anchorBPos, anchorBTopPos, anchorBHeight);


            // Counts
            int submeshCount = 3;

            NativeArray<int> vertexCounts = new NativeArray<int>(3, Allocator.Temp);
            vertexCounts[0] = 4;
            vertexCounts[1] = 4;
            vertexCounts[2] = 16;

            NativeArray<int> triangleCounts = new NativeArray<int>(3, Allocator.Temp);
            triangleCounts[0] = 2;
            triangleCounts[1] = 2;
            triangleCounts[2] = 8;

            // Set up the mesh and stream.
            Mesh wallMesh = new Mesh { name = "WallMesh" };
            wallMesh.Clear();

            Mesh.MeshDataArray wallMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData wallMeshData = wallMeshDataArray[0];

            var stream = new MultimeshStreamUInt16();
            stream.Setup(wallMeshData, new Bounds(), submeshCount, vertexCounts, triangleCounts);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = CreateFlatFaceVertices(anchorAPosMin, anchorBPosMin, anchorATopPosMin, anchorBTopPosMin);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = CreateFlatFaceVertices(anchorBPosPlus, anchorAPosPlus, anchorBTopPosPlus, anchorATopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(1, i, faceVertices[i]); }
            // Top
            faceVertices = CreateFlatFaceVertices(anchorATopPosMin, anchorBTopPosMin, anchorATopPosPlus, anchorBTopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i, faceVertices[i]); }
            // Bottom
            faceVertices = CreateFlatFaceVertices(anchorBPosMin, anchorAPosMin, anchorBPosPlus, anchorAPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 4, faceVertices[i]); }
            // AnchorA
            faceVertices = CreateFlatFaceVertices(anchorAPosPlus, anchorAPosMin, anchorATopPosPlus, anchorATopPosMin);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 8, faceVertices[i]); }
            // AnchorB
            faceVertices = CreateFlatFaceVertices(anchorBPosMin, anchorBPosPlus, anchorBTopPosMin, anchorBTopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(2, i + 12, faceVertices[i]); }

            // Create the triangles
            for (int i = 0; i < 2; i++)
            {
                stream.SetTriangle(i, 0, new int3(0, 3, 1));
                stream.SetTriangle(i, 1, new int3(0, 2, 3));
            }
            for (int t = 0, v = 0; t < triangleCounts[2]; t += 2, v += 4)
            {
                stream.SetTriangle(2, t, new int3(v, v + 3, v + 1));
                stream.SetTriangle(2, t + 1, new int3(v, v + 2, v + 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(wallMeshDataArray, wallMesh);

            return wallMesh;
        }

        private static Mesh GenerateFlatWallMesh(float3 anchorAPos, float3 anchorATopPos, float3 anchorBPos, float3 anchorBTopPos)
        {
            // Counts
            int submeshCount = 2;

            NativeArray<int> vertexCounts = new NativeArray<int>(2, Allocator.Temp);
            vertexCounts[0] = 4;
            vertexCounts[1] = 4;

            NativeArray<int> triangleCounts = new NativeArray<int>(2, Allocator.Temp);
            triangleCounts[0] = 2;
            triangleCounts[1] = 2;

            // Set up the mesh and stream.
            Mesh wallMesh = new Mesh { name = "WallMesh" };
            wallMesh.Clear();

            Mesh.MeshDataArray wallMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData wallMeshData = wallMeshDataArray[0];

            var stream = new MultimeshStreamUInt16();
            stream.Setup(wallMeshData, new Bounds(), submeshCount, vertexCounts, triangleCounts);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = CreateFlatFaceVertices(anchorAPos, anchorBPos, anchorATopPos, anchorBTopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = CreateFlatFaceVertices(anchorBPos, anchorAPos, anchorBTopPos, anchorATopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(1, i, faceVertices[i]); }

            // Create the triangles
            for (int i = 0; i < submeshCount; i++)
            {
                stream.SetTriangle(i, 0, new int3(0, 3, 1));
                stream.SetTriangle(i, 1, new int3(0, 2, 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(wallMeshDataArray, wallMesh);

            return wallMesh;
        }

        private static Mesh GenerateWallColliderMesh(float3 anchorAPos, float3 anchorATopPos, float3 anchorBPos, float3 anchorBTopPos)
        {
            // Counts
            int vertexCount = 8;
            int triangleCount = 4;
            int indexCount = 4 * 3;

            // Set up the mesh and stream.
            Mesh colliderMesh = new Mesh { name = "WallColliderMesh" };
            colliderMesh.Clear();

            Mesh.MeshDataArray colliderMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData colliderMeshData = colliderMeshDataArray[0];

            var stream = new MeshStreamUInt16();
            stream.Setup(colliderMeshData, new Bounds(), vertexCount, indexCount);

            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = CreateFlatFaceVertices(anchorAPos, anchorBPos, anchorATopPos, anchorBTopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = CreateFlatFaceVertices(anchorBPos, anchorAPos, anchorBTopPos, anchorATopPos);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(i+4, faceVertices[i]); }

            // Create the triangles
            for (int t = 0, v = 0; t < triangleCount; t += 2, v += 4)
            {
                stream.SetTriangle(t, new int3(v, v + 3, v + 1));
                stream.SetTriangle(t + 1, new int3(v, v + 2, v + 3));
            }

            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(colliderMeshDataArray, colliderMesh);

            return colliderMesh;
        }

        private static Bounds GenerateWallMeshBounds(Vector3 anchorAPos, Vector3 anchorATopPos, Vector3 anchorBPos, Vector3 anchorBTopPos)
        {
            // Calculate the bounds' necessary data.
            Vector3 anchorAMidPos = (anchorAPos + anchorATopPos) / 2f;
            Vector3 anchorBMidPos = (anchorBPos + anchorBTopPos) / 2f;

            Vector3 boundsCenter = Vector3.Lerp(anchorAMidPos, anchorBMidPos, 0.5f);
            float boundsSizeX = math.abs(anchorAPos.x - anchorBPos.x);
            float boundsSizeY = math.abs(math.max(anchorATopPos.y, anchorBTopPos.y) - math.min(anchorAPos.y, anchorBPos.y));
            float boundsSizeZ = math.abs(anchorAPos.z - anchorBPos.z);
            Vector3 boundsSize = new Vector3(boundsSizeX, boundsSizeY, boundsSizeZ);

            return new Bounds(boundsCenter, boundsSize);
        }

        // TODO: decide whether to move this for reuse elsewhere.
        private static Vertex[] CreateFlatFaceVertices(float3 posZeroZero, float3 posOneZero, float3 posZeroOne, float3 posOneOne)
        {
            // Calculate normal and tangent.
            (float3, half4) normalAndTangent = MathHelpers.CalculateTrisNormalAndTangent(posZeroZero, posOneZero, posOneOne);
            //Vector3 faceTangent =  

            // Calculate UV composants.
            half uvDistanceHorizontal = (half)math.distance(posZeroZero, posOneZero);
            half uvDistanceVerticalZero = (half)math.distance(posZeroZero, posZeroOne);
            half uvDistanceVerticalOne = (half)math.distance(posOneZero, posOneOne);

            // Create the vertices.
            Vertex[] vertices = new Vertex[4];

            vertices[0] = new Vertex
            {
                position = posZeroZero,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(half.zero, half.zero)
            };
            vertices[1] = new Vertex
            {
                position = posOneZero,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(uvDistanceHorizontal, half.zero)
            };
            vertices[2] = new Vertex
            {
                position = posZeroOne,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(half.zero, uvDistanceVerticalZero)
            };
            vertices[3] = new Vertex
            {
                position = posOneOne,
                normal = normalAndTangent.Item1,
                tangent = normalAndTangent.Item2,
                texCoord0 = new half2(uvDistanceHorizontal, uvDistanceVerticalOne)
            };

            return vertices;
        }



#if UNITY_EDITOR
        // VISUALIZATION
        //private void OnDrawGizmos()
        //{
        //    if (meshFilter.mesh != null)
        //    {
        //        Bounds meshBounds = meshFilter.mesh.bounds;
        //        Gizmos.DrawWireCube(meshBounds.center, meshBounds.size);
        //    }
        //}
#endif
    }
}
