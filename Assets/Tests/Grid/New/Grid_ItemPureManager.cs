using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_ItemPureManager : Grid_ItemManager
    {
        // VARIABLES 

        // Configurations
        [Header("Config - Source Elements")]
        [SerializeField] protected bool useVisuals = true;
        [SerializeField] protected Mesh useThisMesh;
        [SerializeField] protected Material useThisMaterial;
        [SerializeField] protected Vector3 initialItemRotation;
        [SerializeField] protected Vector3 initialItemScale;

        [Header("Config - Gizmos")]
        [SerializeField] protected Color itemGizmoColour;
        [SerializeField] protected float itemGizmoRadius = 0.05f;

        // References
        [Header("References")]
        [SerializeField] protected Grid_ItemPure[,] myGridItems;
        [SerializeField] protected Grid_ItemPure myActiveItem;

        [Header("Components")]
        [SerializeField] protected RenderParams myInstancedRenderParams;
        [SerializeField] [SerializeReference] protected Matrix4x4[] myInstancedMeshData; // Is SerializeReference actually working?

        [Header("State")]
        [SerializeField] protected Vector3 myPreviousPosition;
        [SerializeField] protected Quaternion myPreviousRotation;
        [SerializeField] protected Vector3 myPreviousScale;


        // METHODS

        // SET UP
        public override void Init(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset)
        {
            base.Init(parentGrid, gridLengthX, gridLengthZ, relativeSize, offset);

            // Create internal items.
            CreateInternalItems(parentGrid, gridLengthX, gridLengthZ, relativeSize, offset);

            // Populate boundsGizmoPoints.
            boundsGizmoVertexPos = new Vector3[4];
            PopulateBoundsGizmoPoints();
        }

        // Separated submethods for ease of overriding.
        protected override void CreateInternalItems(Grid_Base parentGrid, int gridLengthX, int gridLengthZ, float relativeSize, Vector3 offset)
        {
            myGridItems = new Grid_ItemPure[gridLengthX + 1, gridLengthZ + 1];

            // Separate into their own function?
            myInstancedRenderParams = new RenderParams(useThisMaterial);
            myInstancedMeshData = new Matrix4x4[(gridLengthX + 1) * (gridLengthZ + 1)];
            int i = 0;

            for (int x = 0; x <= gridLengthX; x++)
            {
                for (int z = 0; z <= gridLengthZ; z++)
                {
                    // Transform matrix from parameters and parent position.
                    Vector3 position = new Vector3(x * relativeSize, 0, z * relativeSize) + offset;
                    position.y *= relativeSize;
                    position += parentGrid.transform.position;

                    Quaternion rotation = Quaternion.Euler(initialItemRotation) * parentGrid.transform.rotation;
                    Vector3 scale = Vector3.Scale(initialItemScale, parentGrid.transform.localScale);

                    Matrix4x4 transformMatrix = Matrix4x4.Translate(position) * Matrix4x4.Rotate(rotation) * Matrix4x4.Scale(initialItemScale);

                    myGridItems[x, z] = new Grid_ItemPure(myParentGrid, this, new CoordPair(x, z), transformMatrix, relativeSize);
                    myInstancedMeshData[i] = transformMatrix;
                    i++;
                }
            }
        }
        protected void PopulateInstancedRenderDataFromItems()
        {
            int i = 0;
            foreach (Grid_ItemPure item in myGridItems)
            {
                if (i < myInstancedMeshData.Length)
                {
                    myInstancedMeshData[i] = item.MyTransformMatrix;
                    i++;
                }
            }
        }
        protected override void PopulateBoundsGizmoPoints()
        {
            boundsGizmoVertexPos[0] = myGridItems[0, 0].MyPostion + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[1] = myGridItems[myLengthX, 0].MyPostion + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[2] = myGridItems[myLengthX, myLengthZ].MyPostion + new Vector3(0, boundsGizmoHeight, 0);
            boundsGizmoVertexPos[3] = myGridItems[0, myLengthZ].MyPostion + new Vector3(0, boundsGizmoHeight, 0);
        }

        public void CreateHandles(Grid_ItemPure centerItem, int doBeyond)
        {
        }


        // UTILITY
        protected Matrix4x4 GenerateGridItemMatrix(Grid_ItemPure gridItem)
        {
            Vector3 position = new Vector3(gridItem.MyGridCoordinates.x * myRelativeSize, 0, gridItem.MyGridCoordinates.z * myRelativeSize) + myItemOffset;
            position.y *= myRelativeSize;
            position += transform.position;

            Quaternion rotation = myParentGrid.transform.rotation * Quaternion.Euler(initialItemRotation);
            Vector3 scale = Vector3.Scale(initialItemScale, myParentGrid.transform.localScale);

            return Matrix4x4.Translate(position) * Matrix4x4.Rotate(rotation) * Matrix4x4.Scale(scale);
        }
        protected void RegenerateItemTransforms()
        {
            int i = 0;
            foreach (Grid_ItemPure item in myGridItems)
            {
                item.MyTransformMatrix = GenerateGridItemMatrix(item);
                myInstancedMeshData[i] = item.MyTransformMatrix;
                i++;
            }
        }


        // BUILT IN
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // Draw the component items.
            if (myGridItems != null)
            {
                Gizmos.color = itemGizmoColour;
                foreach (Grid_ItemPure item in myGridItems)
                {
                    Gizmos.DrawSphere(item.MyPostion, itemGizmoRadius * myRelativeSize);
                }
            }
        }

        protected void FixedUpdate()
        {
            myPreviousPosition = transform.position;
            myPreviousRotation = transform.rotation;
            myPreviousScale = transform.lossyScale;
        }

        protected void Update()
        {
            // Have we moved?
            if (transform.position != myPreviousPosition || transform.rotation != myPreviousRotation || transform.lossyScale != myPreviousScale)
            {
                RegenerateItemTransforms();
                PopulateBoundsGizmoPoints();
            }

            if (useVisuals && useThisMesh != null)
            {
                Graphics.RenderMeshInstanced(myInstancedRenderParams, useThisMesh, 0, myInstancedMeshData);
            }
        }
    }

}