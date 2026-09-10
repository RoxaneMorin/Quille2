using AYellowpaper.SerializedCollections;
using MeshGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        public Vector3 PosAtPointABase
        {
            get { return AnchorA.PosAtBase; }
        }
        public Vector3 PosAtPointBBase
        {
            get { return AnchorB.PosAtBase; }
        }
        public Vector3 PosAtMidpointBase
        {
            get { return Vector3.Lerp(PosAtPointABase, PosAtPointBBase, 0.5f); }
        }

        public Vector3 PosAtPointATop
        {
            get { return AnchorA.PosAtTop; }
        }
        public Vector3 PosAtPointBTop
        {
            get { return AnchorB.PosAtTop; }
        }
        public Vector3 PosAtMidpointTop
        {
            get { return Vector3.Lerp(PosAtPointATop, PosAtPointBTop, 0.5f); }
        }

        public float HeightAtPointA
        {
            get { return anchorA.Height; }
        }
        public float HeightAtPointB
        {
            get { return anchorB.Height; }
        }
        public float HeightAtMidpoint
        {
            get { return Mathf.Lerp(HeightAtPointA, HeightAtPointB, 0.5f); }
        }

        public float Length
        {
            get { return Vector3.Distance(PosAtPointABase, PosAtPointBBase); }
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

                // Subscribe to anchors' update events.
                anchorA.OnParameterUpdated += AnchorParameterUpdated;
                anchorB.OnParameterUpdated += AnchorParameterUpdated;

                // Generate wall mesh.
                GenerateWallMesh();
            }
            else // log error and commit sudoku
            {
                Debug.LogError("Invalid attempt at initializing a WallSegment with a null WallAnchor parameter.");
                Destroy(this);
            } 
        }


        // UPDATES

        public void AnchorParameterUpdated(WallAnchor_v2 updatedItem)
        {
            ParameterUpdated();
        }

        public void ParameterUpdated()
        {
            GenerateWallMesh();
        }


        // MESH GENERATION
        protected void GenerateWallMesh()
        {
            Mesh wallMesh;
            Mesh colliderMesh;

            Quaternion localRot = GetLocalRotationQuat();
            if (Thickness == 0f)
            {
                wallMesh = GenerateFlatWallMesh(localRot);
                colliderMesh = wallMesh;
            }
            else
            {
                wallMesh = GenerateThickWallMesh(localRot);
                colliderMesh = GenerateFlatWallMesh(localRot, false);
            }

            wallMesh.RecalculateTangents();
            wallMesh.RecalculateBounds();
            colliderMesh.RecalculateBounds();

            wallMesh.name = "WallMesh";
            colliderMesh.name = "WallColliderMesh";

            myMeshFilter.mesh = wallMesh;
            myMeshCollider.sharedMesh = colliderMesh;
        }

        // TODO: make these static, move to other script?
        protected Mesh GenerateFlatWallMesh(Quaternion localRot, bool TwoMats = true)
        {
            // Vertex positions
            float3 posABase = new float3(0);
            float3 posBBase = localRot * new float3(Length, 0, 0);
            float3 posATop = localRot * new float3(0, HeightAtPointA, 0);
            float3 posBTop = localRot * new float3(Length, HeightAtPointB, 0);

            // The mesh proper
            if (TwoMats)
            {
                return MeshGenerationHelpers.GenerateTwoSidedPlaneTwoMats(posABase, posBBase, posATop, posBTop);
            }
            else
            {
                return MeshGenerationHelpers.GenerateTwoSidedPlane(posABase, posBBase, posATop, posBTop);
            }
        }
        protected Mesh GenerateThickWallMesh(Quaternion localRot)
        {
            // Vertex positions
            float halfThickness = Thickness / 2f;
            float3 posABaseLeft = localRot * new float3(0, 0, -halfThickness);
            float3 posBBaseLeft = localRot * new float3(Length, 0, -halfThickness);
            float3 posATopLeft = localRot * new float3(0, HeightAtPointA, -halfThickness);
            float3 posBTopLeft = localRot * new float3(Length, HeightAtPointB, -halfThickness);
            float3 posABaseRight = localRot * new float3(0, 0, halfThickness);
            float3 posBBaseRight = localRot * new float3(Length, 0, halfThickness);
            float3 posATopRight = localRot * new float3(0, HeightAtPointA, halfThickness);
            float3 posBTopRight = localRot * new float3(Length, HeightAtPointB, halfThickness);

            // The mesh proper
            return MeshGenerationHelpers.GenerateBoxThreeMats(posABaseLeft, posBBaseLeft, posATopLeft, posBTopLeft, posABaseRight, posBBaseRight, posATopRight, posBTopRight);
        }

        protected Quaternion GetLocalRotationQuat()
        {
            float angle = MathHelpers.GetNormalizedAngleBetween(PosAtPointABase, PosAtPointBBase);
            return Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.down);
        }


        //

#if DEBUG
        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            // ID
            Vector3 idLabelPos = PosAtMidpointBase;
            idLabelPos.y -= 0.05f;
            Handles.Label(idLabelPos, string.Format("Segment #{0}", ID));
        }
#endif
    }
}

