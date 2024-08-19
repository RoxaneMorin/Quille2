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

        //[Header("State")]
        //[SerializeField] protected Vector3 myPreviousPosition;
        //[SerializeField] protected Quaternion myPreviousRotation;
        //[SerializeField] protected Vector3 myPreviousScale;



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
            // Prepare arrays and rendering parameters.
            myGridItems = new Grid_ItemPure[gridLengthX + 1, gridLengthZ + 1];
            myInstancedRenderParams = new RenderParams(useThisMaterial);
            myInstancedMeshData = new Matrix4x4[(gridLengthX + 1) * (gridLengthZ + 1)];
            int i = 0;

            // Make transform matrices from this manager and the parent grid.
            Matrix4x4 managerTransformMatrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
            Matrix4x4 gridTransformMatrix = Matrix4x4.TRS(parentGrid.transform.position, parentGrid.transform.rotation, parentGrid.transform.lossyScale);

            for (int x = 0; x <= gridLengthX; x++)
            {
                for (int z = 0; z <= gridLengthZ; z++)
                {
                    // Make transform matrix from parameters.
                    Vector3 itemPosition = new Vector3(x * relativeSize, 0, z * relativeSize) + offset;
                    itemPosition.y *= relativeSize;
                    Quaternion itemRotation = Quaternion.Euler(initialItemRotation);
                    Matrix4x4 itemTransformMatrix = Matrix4x4.TRS(itemPosition, itemRotation, initialItemScale);

                    // Apply ancestor matrices.
                    itemTransformMatrix = gridTransformMatrix * managerTransformMatrix * itemTransformMatrix;

                    myGridItems[x, z] = new Grid_ItemPure(myParentGrid, this, new CoordPair(x, z), itemTransformMatrix, relativeSize);
                    myInstancedMeshData[i] = itemTransformMatrix;
                    i++;
                }
            }

            // TODO: Create collider.
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
        protected Matrix4x4 GenerateGridItemMatrix(Grid_ItemPure gridItem, Matrix4x4 managerTransformMatrix, Matrix4x4 gridTransformMatrix)
        {
            // Make transform matrix from parameters.
            Vector3 itemPosition = new Vector3(gridItem.MyGridCoordinates.x * myRelativeSize, 0, gridItem.MyGridCoordinates.z * myRelativeSize) + myItemOffset;
            itemPosition.y *= myRelativeSize;
            Quaternion itemRotation = Quaternion.Euler(initialItemRotation);
            Matrix4x4 itemTransformMatrix = Matrix4x4.TRS(itemPosition, itemRotation, initialItemScale);

            // Apply ancestor matrices and return.
            return gridTransformMatrix * managerTransformMatrix * itemTransformMatrix;
        }
        protected void RegenerateItemTransforms()
        {
            // Make transform matrices from this manager and the parent grid.
            Matrix4x4 managerTransformMatrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
            Matrix4x4 gridTransformMatrix = Matrix4x4.TRS(myParentGrid.transform.position, myParentGrid.transform.rotation, myParentGrid.transform.lossyScale);

            int i = 0;
            foreach (Grid_ItemPure item in myGridItems)
            {
                item.MyTransformMatrix = GenerateGridItemMatrix(item, managerTransformMatrix, gridTransformMatrix);
                myInstancedMeshData[i] = item.MyTransformMatrix;
                i++;
            }
        }



        // TODO: grid search for the closest item to the clicked location
        // TODO: ensure that it works at different scales and rotations.
        

        // Entry function searching for the closest of our grid items to the clicked location.
        protected void SearchForClickedItem(Vector3 cursorPosition)
        {
            // TODO: work in world location instead, raycast if the cursor can't do that.
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
            }


            // bottomLeft, bottomRight, topLeft, topRight
            (int, int)[] itemsIndices = { (0, 0), (myLengthX, 0), (0, myLengthZ), (myLengthX, myLengthZ) };

            Grid_ItemPure currentGridItem;
            float distanceFromCursor;

            (float, Corners, Grid_ItemPure) minDistance = (float.PositiveInfinity, Corners.none, null);

            for (int i = 0; i < 4; i++)
            {
                currentGridItem = myGridItems[itemsIndices[i].Item1, itemsIndices[i].Item2];
                distanceFromCursor = Vector2.Distance(cursorPosition, currentGridItem.MyScreenPosition);

                if (distanceFromCursor < minDistance.Item1)
                {
                    minDistance = (distanceFromCursor, (Corners)i, currentGridItem);
                }
            }

            Debug.Log(string.Format("Closest point: {0} {1}, distance of {2}.", minDistance.Item2, minDistance.Item3.MyScreenPosition, minDistance.Item1));
        }

        // TODO: the recursive search. Probably won't do actual recursion.


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
            //myPreviousPosition = transform.position;
            //myPreviousRotation = transform.rotation;
            //myPreviousScale = transform.lossyScale;
        }

        protected void Update()
        {
            // Have we moved?
            if (transform.hasChanged)
            {
                RegenerateItemTransforms();
                PopulateBoundsGizmoPoints();

                transform.hasChanged = false;
            }

            if (useVisuals && useThisMesh != null)
            {
                Graphics.RenderMeshInstanced(myInstancedRenderParams, useThisMesh, 0, myInstancedMeshData);
            }
        }

        protected void OnMouseDown()
        {
            Debug.Log(string.Format("Mouse down on {0} at {1}.", gameObject, Input.mousePosition));

            SearchForClickedItem(Input.mousePosition);
        }
    }

}