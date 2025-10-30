using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using MeshGeneration;
using System.Collections.Specialized;

namespace Building
{
    public partial class WallSegment : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private WallAnchor anchorA;
        [SerializeField] private WallAnchor anchorB;
        // These should never be null.

        //private WallConnection connectionA;
        //private WallConnection connectionB;

        // TODO: create all the setters and accessors for the wall connections, update existing functions, etc?
        // Though they are annoying with references and serialization :/ 

        [SerializeField] private float thickness;
        // Some info on both 'sides'?
        // Should it depend on the wall direction?

        // Consequences of the thickness
        private float3 anchorAGroundPosMin;
        private float3 anchorATopPosMin;
        private float3 anchorBGroundPosMin;
        private float3 anchorBTopPosMin;

        private float3 anchorAGroundPosPlus;
        private float3 anchorATopPosPlus;
        private float3 anchorBGroundPosPlus;
        private float3 anchorBTopPosPlus;


        [Header("Resources")]
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;

        // TODO: track materials, so they can be adjusted when the wall is thick vs thicknessless?

        
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

        public float3 AnchorAGroundPosMin { get { return thickness > 0 ? anchorAGroundPosMin : AnchorAGroundPositionF; } }
        public float3 AnchorATopPosMin { get { return thickness > 0 ? anchorATopPosMin : AnchorATopPositionF; } }
        public float3 AnchorBGroundPosMin { get { return thickness > 0 ? anchorBGroundPosMin : AnchorBGroundPositionF; } }
        public float3 AnchorBTopPosMin { get { return thickness > 0 ? anchorBTopPosMin : AnchorBTopPositionF; } }

        public float3 AnchorAGroundPosPlus { get { return thickness > 0 ? anchorAGroundPosPlus : AnchorAGroundPositionF; } }
        public float3 AnchorATopPosPlus { get { return thickness > 0 ? anchorATopPosPlus : AnchorATopPositionF; } }
        public float3 AnchorBGroundPosPlus { get { return thickness > 0 ? anchorBGroundPosPlus : AnchorBGroundPositionF; } }
        public float3 AnchorBTopPosPlus { get { return thickness > 0 ? anchorBTopPosPlus : AnchorBTopPositionF; } }

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

                OnParameterUpdate();
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
        public Vector3 CenterMinPos
        {
            get
            {
                Vector3 centerPosA = (AnchorAGroundPosMin + AnchorATopPosMin) / 2f;
                Vector3 centerPosB = (AnchorBGroundPosMin + AnchorBTopPosMin) / 2f;
                return Vector3.Lerp(centerPosA, centerPosB, 0.5f);
            }
        }
        public Vector3 CenterPlusPos
        {
            get
            {
                Vector3 centerPosA = (AnchorAGroundPosPlus + AnchorATopPosPlus) / 2f;
                Vector3 centerPosB = (AnchorBGroundPosPlus + AnchorBTopPosPlus) / 2f;
                return Vector3.Lerp(centerPosA, centerPosB, 0.5f);
            }
        }



        // METHODS

        // INIT
        public void Init(WallAnchor anchorA, WallAnchor anchorB, float thickness = 0.1f)
        {
            // Name the game object.
            gameObject.name = string.Format("WallSegment ({0} <-> {1})", anchorA.ID, anchorB.ID);

            // Set the anchor references.
            this.anchorA = anchorA;
            this.anchorB = anchorB;

            // Adjust other parameters.
            this.thickness = thickness;

            // Fetch the mesh components and generate the mesh.
            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshCollider = gameObject.GetComponent<MeshCollider>();

            //GenerateWallMesh();
        }


        // UTILITY
        public void OnParameterUpdate()
        {
            GenerateWallMesh();
        }

        //public void InverseWallDirection()
        //{
        //    // TODO: update the connection also

        //    WallAnchor tempAnchor = anchorA;
        //    anchorA = anchorB;
        //    anchorB = tempAnchor;

        //    OnParameterUpdate();
        //}


        // MESH GENERATION

        /*
         * TODO:
         * - Account for intersecting walls
         */

        public void GenerateWallMesh()
        {
            float3 anchorAGroundPos = AnchorAGroundPositionF;
            float3 anchorBGroundPos = AnchorBGroundPositionF;
            float3 anchorATopPos = AnchorATopPositionF;
            float3 anchorBTopPos = AnchorBTopPositionF;

            // Generate the mesh & corresponding collider.
            Mesh wallMesh;
            Mesh colliderMesh;

            if (thickness > 0f)
            {
                // Calculate displacement based on wall thickness and normal.
                float3 thicknessDisplacement = Normal * (thickness / 2);

                // Calculate the displaced points
                anchorAGroundPosMin = anchorAGroundPos - thicknessDisplacement;
                anchorBGroundPosMin = anchorBGroundPos - thicknessDisplacement;
                anchorATopPosMin = anchorATopPos - thicknessDisplacement;
                anchorBTopPosMin = anchorBTopPos - thicknessDisplacement;

                anchorAGroundPosPlus = anchorAGroundPos + thicknessDisplacement;
                anchorBGroundPosPlus = anchorBGroundPos + thicknessDisplacement;
                anchorATopPosPlus = anchorATopPos + thicknessDisplacement;
                anchorBTopPosPlus = anchorBTopPos + thicknessDisplacement;

                // TODO: check adjacent walls if we need to clip.

                WallSegment? aPrev = anchorA.GetSegmentPreceding(this);
                WallSegment? aNext = anchorA.GetSegmentFollowing(this);
                WallSegment? bPrev = anchorB.GetSegmentPreceding(this);
                WallSegment? bNext = anchorB.GetSegmentFollowing(this);

                Debug.Log($"Walls adjacent to {this}:\nAPrev: {aPrev}\nANext: {aNext}\nBPrev: {bPrev}\nBNext: {bNext}");

                // If the two walls share the same AnchorA, min/min or plus/plus face each other
                // Else, we have min/plus or plus/min

                // Check if min or plus is determined by prev vs next.

                if (aPrev != null)
                {
                    bool sharedAnchor = this.AnchorA == aPrev.AnchorA;
                    Debug.Log($"Do {this} and {aPrev} (aPrev) share the same anchor A? {sharedAnchor}");
                }
                if (aNext != null)
                {
                    bool sharedAnchor = this.AnchorA == aNext.AnchorA;
                    Debug.Log($"Do {this} and {aNext} (aNext) share the same anchor A? {sharedAnchor}");
                }
                if (bPrev != null)
                {
                    bool sharedAnchor = this.AnchorB == bPrev.AnchorB;
                    Debug.Log($"Do {this} and {bPrev} (bPrev) share the same anchor B? {sharedAnchor}");
                }
                if (bNext != null)
                {
                    bool sharedAnchor = this.AnchorB == bNext.AnchorB;
                    Debug.Log($"Do {this} and {bNext} (bNext) share the same anchor B? {sharedAnchor}");
                }


                // TODO: check for intersections in 3D

                //if (aPrev != null)
                //{
                //    bool minWithAMin = MathHelpers.DoSegmentsIntersectXZ(anchorAGroundPosMin, anchorBGroundPosMin, aPrev.AnchorAGroundPosMin, aPrev.anchorBGroundPosMin);
                //    bool minWithAPlus = MathHelpers.DoSegmentsIntersectXZ(anchorAGroundPosMin, anchorBGroundPosMin, aPrev.AnchorAGroundPosPlus, aPrev.anchorBGroundPosPlus);

                //    bool plusWithAMin = MathHelpers.DoSegmentsIntersectXZ(anchorAGroundPosPlus, anchorBGroundPosPlus, aPrev.AnchorAGroundPosMin, aPrev.anchorBGroundPosMin);
                //    bool plusWithAPlus = MathHelpers.DoSegmentsIntersectXZ(anchorAGroundPosPlus, anchorBGroundPosPlus, aPrev.AnchorAGroundPosPlus, aPrev.anchorBGroundPosPlus);

                //    Debug.Log($"Min side intersects with:\naPrev min: {minWithAMin}\naPrev plus: {minWithAPlus}\nPlus side intersects with:\naPrev min: {plusWithAMin}\naPrev plus: {plusWithAPlus}");

                //    // Need to check them all as two min sides may be "facing" each other.
                //    // Is there a way to check for this so we can avoid useless calculations?

                //    Vector3? test = MathHelpers.CalculatePotentialIntersectionPointXZ(anchorAGroundPosMin, anchorBGroundPosMin, aPrev.AnchorAGroundPosMin, aPrev.anchorBGroundPosMin);

                //    Debug.Log(test);
                //}

                // Generate the actual meshes.
                wallMesh = GenerateThickWallMesh(anchorAGroundPosMin, anchorBGroundPosMin, anchorATopPosMin, anchorBTopPosMin, anchorAGroundPosPlus, anchorBGroundPosPlus, anchorATopPosPlus, anchorBTopPosPlus);
                colliderMesh = GenerateWallColliderMesh(anchorAGroundPos, anchorATopPos, anchorBGroundPos, anchorBTopPos);
            }
            else
            {
                // Generate the one flat mesh.
                wallMesh = GenerateFlatWallMesh(anchorAGroundPos, anchorATopPos, anchorBGroundPos, anchorBTopPos);
                colliderMesh = wallMesh;
            }

            // Generate bounds.
            Bounds wallBounds = GenerateWallMeshBounds(anchorAGroundPos, anchorATopPos, anchorBGroundPos, anchorBTopPos);

            // Set!
            wallMesh.bounds = wallBounds;

            meshFilter.mesh = wallMesh;
            meshCollider.sharedMesh = colliderMesh;
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
