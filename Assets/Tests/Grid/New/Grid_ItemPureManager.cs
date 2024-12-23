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
        protected Matrix4x4[] myInstancedMeshData;

        //[Header("State")]
        //[SerializeField] protected Vector3 myPreviousPosition;
        //[SerializeField] protected Quaternion myPreviousRotation;
        //[SerializeField] protected Vector3 myPreviousScale;

        // SEARCH VARIABLES
        private readonly object lockObject = new object();
        private bool isBusy;

        float halfTileLength;
        int currentLengthX;
        int currentLengthZ;

        int searchDepth;
        int totalPointsLookedAt;

        (int, int)[] itemsIndices = new (int, int)[4];

        Grid_ItemPure currentGridItem;
        float distanceFromCursor;
        (float, Corners, Grid_ItemPure) currentMinDistance;
        (float, Corners, Grid_ItemPure) currentMinDistanceSub;


        // EVENTS
        public event GridItemPureClicked OnItemClicked;



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

            // Register to own event(s).
            this.OnItemClicked += OnMyItemClicked;
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
                    Vector3 itemScale = initialItemScale * relativeSize;
                    Matrix4x4 itemTransformMatrix = Matrix4x4.TRS(itemPosition, itemRotation, itemScale);

                    // Apply ancestor matrices.
                    itemTransformMatrix = gridTransformMatrix * managerTransformMatrix * itemTransformMatrix;

                    myGridItems[x, z] = new Grid_ItemPure(myParentGrid, this, new CoordPair(x, z), itemTransformMatrix, relativeSize);
                    myGridItems[x, z].OnItemClicked += OnMyItemClicked;

                    myInstancedMeshData[i] = itemTransformMatrix;
                    i++;
                }
            }
            // TODO: subscribe to events.
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
            Vector3 itemScale = initialItemScale * myRelativeSize;
            Matrix4x4 itemTransformMatrix = Matrix4x4.TRS(itemPosition, itemRotation, itemScale);

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
        

        // ITEM SEARCH
        // Entry function searching for the closest of our grid items to the clicked location.
        protected void SearchForClickedItem(Vector3 mousePosition)
        {
            RaycastHit cursorHit;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            // Raycast so we can work with world positions.
            if (Physics.Raycast(ray, out cursorHit))
            {
                // Initial dimensions and the like.
                halfTileLength = myRelativeSize / 2 + 0.05f;
                currentLengthX = myLengthX;
                currentLengthZ = myLengthZ;

                searchDepth = 0;
                totalPointsLookedAt = 0;

                // bottomLeft, bottomRight, topLeft, topRight
                itemsIndices[0] = (0, 0);
                itemsIndices[1] = (myLengthX, 0);
                itemsIndices[2] = (0, myLengthZ);
                itemsIndices[3] = (myLengthX, myLengthZ);

                // Holder variables.
                currentGridItem = null;
                distanceFromCursor = float.PositiveInfinity;
                currentMinDistance = (float.PositiveInfinity, Corners.none, null);

                // not sure it's the right loop condition if they have different lenght
                while (currentLengthX > 0 || currentLengthZ > 0)
                {
                    //Debug.Log(string.Format("Dimensions of the current search area: {0} x {1}", currentLengthX + 1, currentLengthZ + 1));
                    Debug.Log(string.Format("Corner points of the current search area: {0}, {1}, {2}, {3}", itemsIndices[0], itemsIndices[1], itemsIndices[2], itemsIndices[3]));

                    currentMinDistance = SearchedForClickedItemSub(cursorHit.point, currentMinDistance, ref searchDepth, ref totalPointsLookedAt);

                    // Return this closest point if it's less than half a tile away.
                    if (currentMinDistance.Item1 < halfTileLength)
                    {
                        Debug.Log(string.Format("Final closest point: {0} at {1}, distance of {2}. Looked at a total of {3} points out of {4}.", currentMinDistance.Item3, currentMinDistance.Item3.MyPostion, currentMinDistance.Item1, totalPointsLookedAt, ((myLengthX + 1) * (myLengthZ + 1))));

                        // Do Event.
                        OnItemClicked?.Invoke(currentMinDistance.Item3);

                        return;
                    }
                    else
                    {
                        currentLengthX = Mathf.Max(currentLengthX / 2, 0);
                        currentLengthZ = Mathf.Max(currentLengthZ / 2, 0);

                        switch (currentMinDistance.Item2)
                        {
                            case Corners.bottomLeft:
                                {
                                    // bottom left: we want { (0, 0), (myLengthX/2, 0), (0, myLengthZ/2), (myLengthX/2, myLengthZ/2) };
                                    itemsIndices[0] = currentMinDistance.Item3.MyGridCoordinates.AsTuple;
                                    itemsIndices[1] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((currentLengthX, 0));
                                    itemsIndices[2] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((0, currentLengthZ));
                                    itemsIndices[3] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((currentLengthX, currentLengthZ));
                                    break;
                                }

                            case Corners.bottomRight:
                                {
                                    // bottom right: we want { (myLengthX/2, 0), (myLengthX, 0), (myLengthX/2, myLengthZ/2), (myLengthX, myLengthZ/2) };
                                    itemsIndices[0] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((-currentLengthX, 0));
                                    itemsIndices[1] = currentMinDistance.Item3.MyGridCoordinates.AsTuple;
                                    itemsIndices[2] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((-currentLengthX, currentLengthZ));
                                    itemsIndices[3] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((0, currentLengthZ));
                                    break;
                                }
                            case Corners.topLeft:
                                {
                                    // top left: we want { (0, myLengthZ/2), (myLengthX/2, myLengthZ/2), (0, myLengthZ), (myLengthX/2, myLengthZ) };
                                    itemsIndices[0] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((0, -currentLengthZ));
                                    itemsIndices[1] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((currentLengthX, -currentLengthZ));
                                    itemsIndices[2] = currentMinDistance.Item3.MyGridCoordinates.AsTuple;
                                    itemsIndices[3] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((currentLengthX, 0));
                                    break;
                                }
                            case Corners.topRight:
                                {
                                    // top right: we want { (myLengthX/2, myLengthZ/2), (myLengthX, myLengthZ/2), (myLengthX/2, myLengthZ), (myLengthX, myLengthZ) };
                                    itemsIndices[0] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((-currentLengthX, -currentLengthZ));
                                    itemsIndices[1] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((0, -currentLengthZ));
                                    itemsIndices[2] = currentMinDistance.Item3.MyGridCoordinates.AsTuple.Add((-currentLengthX, 0));
                                    itemsIndices[3] = currentMinDistance.Item3.MyGridCoordinates.AsTuple;
                                    break;
                                }
                            default:
                                {
                                    Debug.LogWarning(string.Format("The ItemPureManager {0} was clicked, but the search for the closest point failed.", gameObject.name));
                                    return;
                                }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning(string.Format("The ItemPureManager {0} was clicked, but the raycast did not hit anything.", gameObject.name));
                return;
            }
        }

        protected (float, Corners, Grid_ItemPure) SearchedForClickedItemSub(Vector3 cursorPosition, (float, Corners, Grid_ItemPure) previousMinDistance, ref int searchDepth, ref int totalPointsLookedAt)
        {
            currentMinDistanceSub = (previousMinDistance.Item1, previousMinDistance.Item2, previousMinDistance.Item3);

            searchDepth++;
            for (int i = 0; i < 4; i++)
            {
                if ((Corners)i != previousMinDistance.Item2)
                {
                    totalPointsLookedAt++;

                    currentGridItem = myGridItems[itemsIndices[i].Item1, itemsIndices[i].Item2];
                    distanceFromCursor = Vector3.Distance(cursorPosition, currentGridItem.MyPostion);
                    // TODO: keep array2d of distances to avoid having to recalculate them? Are they event recalculated at times?

                    if (distanceFromCursor < currentMinDistanceSub.Item1)
                    {
                        currentMinDistanceSub = (distanceFromCursor, (Corners)i, currentGridItem);
                    }

                    //Debug.Log(string.Format("Depth {0}. Currently looking at point: {1} at {2}, {3} of the current search points, distance of {4}.", searchDepth, currentGridItem, currentGridItem.MyPostion, (Corners)i, distanceFromCursor));
                }
            }

            Debug.Log(string.Format("Depth {0}. Closest point: {1} at {2}, {3} of the current search points, distance of {4}.", searchDepth, currentMinDistanceSub.Item3, currentMinDistanceSub.Item3.MyPostion, currentMinDistanceSub.Item2, currentMinDistanceSub.Item1));
            return currentMinDistanceSub;
        }


        // EVENTS
        protected void OnMyItemClicked(Grid_ItemPure clickedItem)
        {
            Debug.Log(string.Format("{0} aknowledges that its item {1} was clicked.", gameObject.name, clickedItem));
        }

        protected void OnMyHandleClicked(Grid_Handle clickedHandle)
        {
            Debug.Log(string.Format("{0} aknowledges that its handle {1}, curently attached to {2}, was clicked.", gameObject.name, clickedHandle.name, clickedHandle.MyCurrentItem.name));
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
            lock (lockObject)
            {
                if (!isBusy)
                {
                    isBusy = true;
                    Debug.Log(string.Format("Mouse down on {0} at {1}.", gameObject, Input.mousePosition));
                    SearchForClickedItem(Input.mousePosition);
                    isBusy = false;
                }
            }
        }
    }

}