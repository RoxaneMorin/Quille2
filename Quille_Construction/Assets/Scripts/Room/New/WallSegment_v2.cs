using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Building
{
    public class WallSegment_v2 : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [Header("References")]
        [SerializeField] protected MeshFilter myMeshFilter;
        [SerializeField] protected MeshRenderer myMeshRenderer;
        [SerializeField] protected MeshCollider myMeshCollider;

        [SerializeField] protected LineRenderer myLineRenderer;
       
        [Header("Data")]
        [SerializeField] protected int id;
        [SerializeField] protected WallAnchor_v2 anchorA;
        [SerializeField] protected WallAnchor_v2 anchorB;
        [SerializeField] protected float thickness;


        // PROPERTIES
        public int ID { get { return id; } }

        public WallAnchor_v2 AnchorA
        {
            get { return anchorA; }
        }
        public WallAnchor_v2 AnchorB
        {
            get { return anchorB; }
        }

        public float Thickness
        {
            get { return thickness; }
            set
            {
                if (value < Constants_Building.MIX_WALL_SEGMENT_THICKNESS)
                {
                    thickness = Constants_Building.MIX_WALL_SEGMENT_THICKNESS;
                }
                else if (value > Constants_Building.MAX_WALL_SEGMENT_THICKNESS)
                {
                    thickness = Constants_Building.MAX_WALL_SEGMENT_THICKNESS;
                }
                else
                {
                    thickness = value;
                }
            }
        }

        //
        public Vector3 PosAtPointA
        {
            get { return AnchorA.transform.position; }
        }
        public Vector3 PosAtPointB
        {
            get { return AnchorB.transform.position; }
        }
        public Vector3 LocalPosAtPointB
        {
            get { return AnchorB.transform.position - AnchorA.transform.position; }
        }
        public Vector3 PosAtMidpoint
        {
            get { return Vector3.Lerp(PosAtPointA, PosAtPointB, 0.5f); }
        }

        public float DistanceBetweenPoints
        {
            get { return Vector3.Distance(PosAtPointA, PosAtPointB); }
        }

        public float HeightAtPointA
        {
            get { return anchorA.Height; }
        }
        public float HeightAtPointB
        {
            get { return anchorB.Height; }
        }



        // METHODS

        // INIT
        public void Init(int id, WallAnchor_v2 anchorA, WallAnchor_v2 anchorB, float thickness = 0.1f)
        {
            // Fetch mesh components.
            myMeshFilter = gameObject.GetComponent<MeshFilter>();
            myMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            myMeshCollider = gameObject.GetComponent<MeshCollider>();

            // Ensure that the given wall anchors are non-null.
            if (anchorA != null && anchorB != null)
            {
                // Always start from the lowest ID anchor.
                ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

                // Name the game object.
                gameObject.name = string.Format("WallSegment {0} ({1} <-> {2})", id, anchorA.ID, anchorB.ID);

                // Set the anchor references.
                this.anchorA = anchorA;
                this.anchorB = anchorB;
                WallAnchor_v2.Connect(anchorA, anchorB);

                // Adjust other parameters.
                this.id = id;
                this.thickness = thickness;

                // Temp line renderer
                myLineRenderer = gameObject.GetComponent<LineRenderer>();
                myLineRenderer.SetPosition(0, anchorA.transform.position);
                myLineRenderer.SetPosition(1, anchorB.transform.position);

                // Test mesh
                GenerateMesh();
            }
            else // log error and commit sudoku
            {
                Debug.LogError("Invalid attempt at initializing a WallSegment with a null WallAnchor parameter.");
                Destroy(this);
            } 
        }




        // Generate flat mesh
        protected void GenerateMesh()
        {
            // TODO: simpler to modify a base/existing mesh instead?

            Mesh generatedMesh = new Mesh();
            generatedMesh.name = "WallMesh";

            float halfThickness = Thickness / 2f;

            var vertices = new NativeArray<Vector3>(8, Allocator.Temp);
            vertices[0] = new Vector3(0, 0, halfThickness);
            vertices[1] = new Vector3(DistanceBetweenPoints, 0, halfThickness);
            vertices[2] = new Vector3(0, HeightAtPointA, halfThickness);
            vertices[3] = new Vector3(DistanceBetweenPoints, HeightAtPointB, halfThickness);

            vertices[4] = new Vector3(0, 0, -halfThickness);
            vertices[5] = new Vector3(DistanceBetweenPoints, 0, -halfThickness);
            vertices[6] = new Vector3(0, HeightAtPointA, -halfThickness);
            vertices[7] = new Vector3(DistanceBetweenPoints, HeightAtPointB, -halfThickness);


            // Rotate the vector
            float angle = MathHelpers.GetNormalizedAngleBetween(PosAtPointA, PosAtPointB);
            Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.down);
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                vertices[i] = rotation * vertex;
            }

            var UVs = new NativeArray<Vector2>(8, Allocator.Temp);
            UVs[0] = Vector2.zero;
            UVs[1] = new Vector2(DistanceBetweenPoints, 0);
            UVs[2] = new Vector2(0, HeightAtPointA);
            UVs[3] = new Vector2(DistanceBetweenPoints, HeightAtPointB);
            UVs[4] = Vector2.zero;
            UVs[5] = new Vector2(DistanceBetweenPoints, 0);
            UVs[6] = new Vector2(0, HeightAtPointA);
            UVs[7] = new Vector2(DistanceBetweenPoints, HeightAtPointB);

            var triangles = new NativeArray<int>(12, Allocator.Temp);
            triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
            triangles[3] = 1; triangles[4] = 3; triangles[5] = 2;
            triangles[6] = 4; triangles[7] = 6; triangles[8] = 5;
            triangles[9] = 5; triangles[10] = 6; triangles[11] = 7;

            generatedMesh.SetVertices(vertices);
            generatedMesh.SetUVs(0, UVs);
            generatedMesh.SetIndices(triangles, MeshTopology.Triangles, 0);

            vertices.Dispose();
            triangles.Dispose();

            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateBounds();

            myMeshFilter.mesh = generatedMesh;
        }






#if DEBUG
        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            // ID
            Vector3 idLabelPos = PosAtMidpoint;
            idLabelPos.y -= 0.05f;
            Handles.Label(idLabelPos, string.Format("Segment #{0}", ID));
        }
#endif
    }
}

