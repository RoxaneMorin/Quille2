using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class Grid_Handle : MonoBehaviour
    {
        // VARIABLES

        // Components
        //[Header("Components")]
        protected MeshCollider myMeshCollider;
        protected MeshRenderer myMeshRenderer;

        // Config
        [Header("Config")]
        [SerializeField] protected float cursorMovementNoticeDistance = 1f;
        [SerializeField] protected float cursorFadeDistance = 100f;

        // References
        [Header("References")]
        [SerializeField] protected Grid_Base myParentGrid;
        [SerializeField] protected Grid_ItemManager myParentGridItems;
        [SerializeField] [SerializeReference] protected Grid_Item myCurrentItem;


        // Cursor State
        protected Vector3 previousCursorPosition;
        protected float previousCursorPositionDelta;

        protected Vector3 myScreenPos;
        protected float myCurrentDistanceFromCursor;

        protected bool mouseHoveringOnMe;
        protected bool mouseClickingOnMe;
        protected bool mouseHoveringSibling;
        protected bool mouseClickingSibling;
        // TODO: find a better name for these.



        // PROPERTIES
        public Grid_Base MyParentGrid { get { return myParentGrid; } set { myParentGrid = value; } }
        public Grid_ItemManager MyParentGridItems { get { return myParentGridItems; } set { myParentGridItems = value; } }
        public Grid_Item MyCurrentItem { get { return myCurrentItem; } set { myCurrentItem = value; } }

        public CoordPair MyItemsGridCoordinates { get { return myCurrentItem.MyGridCoordinates; } }
        public Vector3 MyItemsPosition { get { return myCurrentItem.transform.position; } }
        


        // METHODS

        // CONSTRUCTION
        protected void Init()
        {
            myMeshCollider = GetComponent<MeshCollider>();
            myMeshRenderer = GetComponent<MeshRenderer>();

            myMeshRenderer.material.SetFloat("_CursorFadeDistance", cursorFadeDistance);
        }

        public void SetReferencesAndPosition(Grid_Base parentGrid, Grid_ItemManager parentGridItems, Grid_Item gridItem)
        {
            SetReferences(parentGrid, parentGridItems);
            AssignGridItem(gridItem);
        }
        public void SetReferences(Grid_Base parentGrid, Grid_ItemManager parentGridItems)
        {
            myParentGrid = parentGrid;
            myParentGridItems = parentGridItems;
        }
        public void AssignGridItem(Grid_Item gridItem)
        {
            myCurrentItem = gridItem;
            transform.position = myCurrentItem.transform.position;
        }
        public void ClearAndDeactivate()
        {
            myCurrentItem = null;

            mouseHoveringOnMe = false;
            mouseClickingOnMe = false;
            mouseHoveringSibling = false;
            mouseClickingSibling = false;

            transform.localPosition = Vector3.zero;

            gameObject.SetActive(false);
        }
        



        // UTILITY
        protected void UpdateMaterialDistanceParameter()
        {
            // Update my material's cursor distance param.
            myScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            myScreenPos.z = 0;

            myCurrentDistanceFromCursor = Vector3.Distance(Input.mousePosition, myScreenPos);
            myMeshRenderer.material.SetFloat("_CurrentDistanceFromCursor", myCurrentDistanceFromCursor);
        }

        // Alter both material parameters.
        public void AlterMaterial(bool newDoCursorDistanceFade, Color newColour)
        {
            myMeshRenderer.material.SetColor("_Color", newColour);
            myMeshRenderer.material.SetFloat("_DoCursorDistanceFade", (newDoCursorDistanceFade ? 1f : 0f));
        }
        public void AlterMaterial(bool newDoCursorDistanceFade, Color newColour, float newAlpha)
        {
            Color combinedColour = new Color(newColour.r, newColour.g, newColour.b, newAlpha);
            myMeshRenderer.material.SetColor("_Color", combinedColour);
            myMeshRenderer.material.SetFloat("_DoCursorDistanceFade", (newDoCursorDistanceFade ? 1f : 0f));
        }
        // Alter distance fade only.
        public void AlterMaterial(bool newDoCursorDistanceFade)
        {
            myMeshRenderer.material.SetFloat("_DoCursorDistanceFade", (newDoCursorDistanceFade ? 1f : 0f));
        }
        // Alter colour only.
        public void AlterMaterial(Color newColour)
        {
            myMeshRenderer.material.SetColor("_Color", newColour);
        }
        public void AlterMaterial(Color newColour, float newAlpha)
        {
            Color combinedColour = new Color(newColour.r, newColour.g, newColour.b, newAlpha);
            myMeshRenderer.material.SetColor("_Color", combinedColour);
        }
        




        // BUILT IN
        protected void Awake()
        {
            Init();
        }

        protected void FixedUpdate()
        {
            previousCursorPositionDelta = Vector3.Distance(previousCursorPosition, Input.mousePosition);

            if (previousCursorPositionDelta >= cursorMovementNoticeDistance)
            {
                UpdateMaterialDistanceParameter();
            }

            previousCursorPosition = Input.mousePosition;
        }

        // MOUSE HOVER
        protected void OnMouseEnter()
        {
            mouseHoveringOnMe = true;

            Debug.Log(string.Format("Mouse entered {0}.", gameObject));
        }
        protected void OnMouseExit()
        {
            mouseHoveringOnMe = false;

            Debug.Log(string.Format("Mouse exited {0}.", gameObject));
        }

        // MOUSE CLICK
        protected void OnMouseDown()
        {
            mouseClickingOnMe = true;
            Debug.Log(string.Format("Mouse down on {0}.", gameObject));
        }
        protected void OnMouseUp()
        {
            mouseClickingOnMe = false;
            Debug.Log(string.Format("Mouse up on {0}.", gameObject));
        }
    }
}
