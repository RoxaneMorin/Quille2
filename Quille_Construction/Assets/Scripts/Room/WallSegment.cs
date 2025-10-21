using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
         * - General clean up / transfer to the other mesh API?
         * - Keep track of the wall 'sides' locations
         * - Account for intersecting walls
         * - Editor stuff with tools to change thickness and the like.
         */


        public void GenerateWallMesh(Vector3 anchorAPos, float anchorAHeight, Vector3 anchorBPos, float anchorBHeight)
        {
            // // Temp: random thickness and heights instead:
            //anchorAHeight = Random.Range(0.5f, 2f);
            //anchorBHeight = Random.Range(0.5f, 2f);
            //thickness = Random.Range(0.05f, 1f);

             
            // Calculate the upper vertices' locations.
            Vector3 anchorATopPos = anchorAPos + new Vector3(0, anchorAHeight, 0);
            Vector3 anchorBTopPos = anchorBPos + new Vector3(0, anchorBHeight, 0);

            // Calculate the initial face normal.
            Vector3 faceNormal = MathHelpers.CalculateFaceNormal(anchorAPos, anchorBPos, anchorBTopPos);

            // Calculate displacement based on wall thickness.
            Vector3 halfNormal = faceNormal * (thickness / 2);

            // Calculate the displaced points
            Vector3 pointZeroZeroFront = anchorAPos - halfNormal;
            Vector3 pointOneZeroFront = anchorBPos - halfNormal;
            Vector3 pointZeroOneFront = anchorATopPos - halfNormal;
            Vector3 pointOneOneFront = anchorBTopPos - halfNormal;

            Vector3 pointZeroZeroBack = anchorAPos + halfNormal;
            Vector3 pointOneZeroBack = anchorBPos + halfNormal;
            Vector3 pointZeroOneBack = anchorATopPos + halfNormal;
            Vector3 pointOneOneBack = anchorBTopPos + halfNormal;

            // Calculate the UV distances.
            float uvDistanceHorizontal = Vector3.Distance(anchorAPos, anchorBPos);
            float uvDistanceVerticalA = Vector3.Distance(anchorAPos, anchorATopPos);
            float uvDistanceVerticalB = Vector3.Distance(anchorBPos, anchorBTopPos);


            // Create the arrays.
            Vector3[] wallVertices = new Vector3[]{pointZeroZeroFront, pointOneZeroFront, pointZeroOneFront, pointOneOneFront, // Main Clockwise
                                                   pointZeroZeroBack, pointOneZeroBack, pointZeroOneBack, pointOneOneBack, // Main Counterclockwise

                                                   pointZeroOneFront, pointOneOneFront, pointZeroOneBack, pointOneOneBack, // Top
                                                   pointZeroZeroFront, pointOneZeroFront, pointZeroZeroBack, pointOneZeroBack, // Bottom

                                                   pointZeroZeroBack, pointZeroZeroFront, pointZeroOneBack, pointZeroOneFront, // AnchorA
                                                   pointOneZeroBack, pointOneZeroFront, pointOneOneBack, pointOneOneFront // AnchorB
            };

            Vector2[] wallUVs = new Vector2[] { new Vector2(0, 0), new Vector2(uvDistanceHorizontal, 0), new Vector2(0, uvDistanceVerticalA), new Vector2(uvDistanceHorizontal, uvDistanceVerticalB),
                                                new Vector2(0, 0), new Vector2(-uvDistanceHorizontal, 0), new Vector2(0, uvDistanceVerticalA), new Vector2(-uvDistanceHorizontal, uvDistanceVerticalB),

                                                new Vector2(0, 0), new Vector2(uvDistanceHorizontal, 0), new Vector2(0, thickness), new Vector2(uvDistanceHorizontal, thickness),
                                                new Vector2(0, 0), new Vector2(-uvDistanceHorizontal, 0), new Vector2(0, thickness), new Vector2(-uvDistanceHorizontal, thickness),

                                                new Vector2(0, 0), new Vector2(thickness, 0), new Vector2(0, uvDistanceVerticalA), new Vector2(thickness, uvDistanceVerticalA),
                                                new Vector2(0, 0), new Vector2(-thickness, 0), new Vector2(0, uvDistanceVerticalB), new Vector2(-thickness, uvDistanceVerticalB)
            };

            int[] wallTrianglesSideA = new int[] { 0, 3, 1, 0, 2, 3 };
            int[] wallTrianglesSideB = new int[] { 4, 5, 7, 4, 7, 6 };
            int[] wallTrianglesOthers = new int[] { 8, 11, 9, 8, 10, 11,
                                                    12, 13, 15, 12, 15, 14,
                                                    16, 19, 17, 16, 18, 19,
                                                    20, 21, 23, 20, 23, 22
            };


            // Create the mesh proper.
            Mesh wallMesh = new Mesh
            {
                name = "WallMesh",
                subMeshCount = 3,

                vertices = wallVertices,
                uv = wallUVs
            };

           wallMesh.SetTriangles(wallTrianglesSideA, 0);
           wallMesh.SetTriangles(wallTrianglesSideB, 1);
           wallMesh.SetTriangles(wallTrianglesOthers, 2);

            // Recalculate the necessary stuff.
            wallMesh.RecalculateNormals();
            wallMesh.RecalculateTangents();
            wallMesh.RecalculateBounds();

            // Set!
            meshFilter.mesh = wallMesh;
            meshCollider.sharedMesh = wallMesh;
        }
    }
}
