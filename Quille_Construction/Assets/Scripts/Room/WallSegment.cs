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



        // PROPERTIES
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

        public WallConnection ConnectionA
        {
            get { return anchorA.GetConnectionTo(anchorA); }
        }
        public WallConnection ConnectionB
        {
            get { return anchorB.GetConnectionTo(anchorB); }
        }

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
                // TODO: update mesh thickness.
            }
        }

        // ADDITIONAL ACCESSORS
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

            GenerateWallMesh(anchorA.Position, anchorA.Height, anchorB.Position, anchorB.Height);
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
         * - Verify whether the wall has thickness, and generate accordingly?
         * - Review collider: can we use a simpler box collider instead?
         * - Keep track of the wall 'sides' locations
         * - Account for intersecting walls
         * - Editor stuff with tools to change thickness and the like.
         */


        // TODO: consider a flat colliderMesh.

        // Idea: if the wall has no thickness, just reuse the flat mesh for it.


        public void GenerateWallMesh(Vector3 anchorAPos, float anchorAHeight, Vector3 anchorBPos, float anchorBHeight)
        {
            // Calculate the upper vertices' locations.
            float3 anchorATopPos = (float3)anchorAPos + new float3(0, anchorAHeight, 0);
            float3 anchorBTopPos = (float3)anchorBPos + new float3(0, anchorBHeight, 0);

            // Calculate the initial wall normal.
            float3 wallNormal = MathHelpers.CalculateFaceNormal(anchorAPos, anchorBPos, anchorBTopPos);

            // Calculate displacement based on wall thickness.
            float3 halfDistance = wallNormal * (thickness / 2);

            // Calculate the displaced points
            float3 anchorAPosMin = (float3)anchorAPos - halfDistance;
            float3 anchorBPosMin = (float3)anchorBPos - halfDistance;
            float3 anchorATopPosMin = anchorATopPos - halfDistance;
            float3 anchorBTopPosMin = anchorBTopPos - halfDistance;

            float3 anchorAPosPlus = (float3)anchorAPos + halfDistance;
            float3 anchorBPosPlus = (float3)anchorBPos + halfDistance;
            float3 anchorATopPosPlus = anchorATopPos + halfDistance;
            float3 anchorBTopPosPlus = anchorBTopPos + halfDistance;

            // Bounds
            Bounds wallBounds = GenerateWallMeshBounds(anchorAPos, anchorATopPos, anchorAHeight, anchorBPos, anchorBTopPos, anchorBHeight);


            // Counts
            int vertexCount = 24;
            int triangleCount = 12;
            int indexCount = triangleCount * 3;

            NativeArray<int> vertexCounts = new NativeArray<int>(1, Allocator.Temp);
            vertexCounts[0] = vertexCount;

            NativeArray<int> indexCounts = new NativeArray<int>(1, Allocator.Temp);
            indexCounts[0] = indexCount;

            // Set up the mesh and stream.
            Mesh wallMesh = new Mesh { name = "WallMesh" };
            wallMesh.Clear();

            Mesh.MeshDataArray wallMeshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData wallMeshData = wallMeshDataArray[0];

            var stream = new MultimeshStreamUInt16();
            stream.Setup(wallMeshData, wallBounds, 1, vertexCounts, indexCounts);


            // Create the vertices
            // Main clockwise
            Vertex[] faceVertices = CreateFaceVertices(anchorAPosMin, anchorBPosMin, anchorATopPosMin, anchorBTopPosMin);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i, faceVertices[i]); }
            // Main counterclockwise
            faceVertices = CreateFaceVertices(anchorBPosPlus, anchorAPosPlus, anchorBTopPosPlus, anchorATopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i+4, faceVertices[i]); }
            // Top
            faceVertices = CreateFaceVertices(anchorATopPosMin, anchorBTopPosMin, anchorATopPosPlus, anchorBTopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i+8, faceVertices[i]); }
            // Bottom
            faceVertices = CreateFaceVertices(anchorBPosMin, anchorAPosMin, anchorBPosPlus, anchorAPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i+12, faceVertices[i]); }
            // AnchorA
            faceVertices = CreateFaceVertices(anchorAPosPlus, anchorAPosMin, anchorATopPosPlus, anchorATopPosMin);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i+16, faceVertices[i]); }
            // AnchorB
            faceVertices = CreateFaceVertices(anchorBPosMin, anchorBPosPlus, anchorBTopPosMin, anchorBTopPosPlus);
            for (int i = 0; i < faceVertices.Length; i++) { stream.SetVertex(0, i+20, faceVertices[i]); }

            // Create the triangles
            for (int t = 0, v = 0; t < triangleCount; t+=2, v+=4)
            {
                stream.SetTriangle(0, t, new int3(v, v+3, v+1));
                stream.SetTriangle(0, t+1, new int3(v, v+2, v+3));
            }


            // Apply and adjust.
            Mesh.ApplyAndDisposeWritableMeshData(wallMeshDataArray, wallMesh);

            // Set!
            wallMesh.bounds = wallBounds; // TODO: verify why the bounds aren't being set earlier :/

            wallMesh.RecalculateBounds();

            meshFilter.mesh = wallMesh;
            meshCollider.sharedMesh = wallMesh;
        }




        

        private static Vertex[] CreateFaceVertices(float3 posZeroZero, float3 posOneZero, float3 posZeroOne, float3 posOneOne)
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

        private static Bounds GenerateWallMeshBounds(Vector3 anchorAPos, Vector3 anchorATopPos, float anchorAHeight, Vector3 anchorBPos, Vector3 anchorBTopPos, float anchorBHeight)
        {
            // Calculate the bounds' necessary data.
            Vector3 anchorAMidPos = anchorAPos + new Vector3(0f, anchorAHeight / 2f, 0f);
            Vector3 anchorBMidPos = anchorBPos + new Vector3(0f, anchorBHeight / 2f, 0f);

            Vector3 boundsCenter = Vector3.Lerp(anchorAMidPos, anchorBMidPos, 0.5f);
            float boundsSizeX = math.abs(anchorAPos.x - anchorBPos.x);
            float boundsSizeY = math.abs(math.max(anchorATopPos.y, anchorBTopPos.y) - math.min(anchorAPos.y, anchorBPos.y));
            float boundsSizeZ = math.abs(anchorAPos.z - anchorBPos.z);
            Vector3 boundsSize = new Vector3(boundsSizeX, boundsSizeY, boundsSizeZ);

            return new Bounds(boundsCenter, boundsSize);
        }


#if UNITY_EDITOR
        // VISUALIZATION
        private void OnDrawGizmos()
        {
            //if (meshFilter.mesh != null)
            //{
            //    Bounds meshBounds = meshFilter.mesh.bounds;
            //    Gizmos.DrawWireCube(meshBounds.center, meshBounds.size);
            //}
        }
#endif
    }
}
