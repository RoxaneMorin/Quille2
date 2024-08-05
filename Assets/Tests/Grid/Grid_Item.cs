using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    public class Grid_Item : MonoBehaviour
    {
        // VARIABLES
        // Components
        protected MeshRenderer myMeshRenderer;
        protected Material myMaterial;
        protected Color myDefaultColour;


        // Visual & Movement Information
        [Header("OnMouse and Selection Behaviour")]
        // Colours
        [SerializeField] protected Color mySelectedColour;
        [SerializeField] protected Color adjacentSelectedColour;

        [SerializeField] protected float cursorMotionNoticeDistance = 0.01f;
        [SerializeField] protected float fadeDistanceFromCursor = 100f;

        protected Vector3 previousMousePos;
        protected float previousMousePosDelta;

        protected bool mouseHoveringOnMe;
        protected bool mouseClickingOnMe;
        protected bool mouseHoveringOnRelative;
        protected bool mouseClickingOnRelative;
        public bool MouseHoveringOnMe { get { return mouseHoveringOnMe; } set { mouseHoveringOnMe = value; } }
        public bool MouseClickingOnMe { get { return mouseClickingOnMe; } set { mouseClickingOnMe = value; } }
        public bool MouseHoveringOnRelative { get { return mouseHoveringOnRelative; } set { mouseHoveringOnRelative = value; } }
        public bool MouseClickingOnRelative { get { return mouseClickingOnRelative; } set { mouseClickingOnRelative = value; } }


        // Grid information
        protected Grid_Base myParentGrid;
        [SerializeField] protected CoordPair myGridCoordinates;



        // METHODS

        // INIT
        protected void FetchComponents()
        {
            myMeshRenderer = GetComponent<MeshRenderer>();
            myMaterial = myMeshRenderer.material;
            myDefaultColour = myMaterial.color;
        }
        public void SetVariables(Grid_Base myParentGrid, CoordPair myGridCoordinates)
        {
            this.myParentGrid = myParentGrid;
            this.myGridCoordinates = myGridCoordinates;
        }


        // UTILITY

        // MOVEMENT
        public void MoveMe(Vector3 positionDelta)
        {
            transform.position += positionDelta;
        }
        virtual public void MoveMeAndRelatives(Vector3 mouseMouvementDelta) { }

        // COLOUR
        public void SetColour(Color newColour)
        {
            myMaterial.color = newColour;
        }
        public void ResetColour()
        {
            myMaterial.color = myDefaultColour;
        }

        // FADE
        protected void DistanceFade()
        {
            Vector3 myScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            myScreenPos.z = 0;
            float myDistanceFromCursor = Vector3.Distance(Input.mousePosition, myScreenPos);

            if (myDistanceFromCursor >= fadeDistanceFromCursor)
            {
                if (myMeshRenderer.enabled)
                {
                    myMeshRenderer.enabled = false;
                }
            }
            else
            {
                if (!myMeshRenderer.enabled)
                {
                    myMeshRenderer.enabled = true;
                }

                float blendValue = myDistanceFromCursor / fadeDistanceFromCursor;
                float lerpedAlpha = Mathf.Lerp(0.5f, 0f, blendValue);
                myDefaultColour = new Color(myDefaultColour.r, myDefaultColour.g, myDefaultColour.b, lerpedAlpha);
                myMaterial.color = myDefaultColour;
            }
        }
        public void DistanceFadeIfUnclicked()
        {
            if (!mouseClickingOnMe && !mouseClickingOnRelative)
            {
                DistanceFade();
            }
        }
        public void DistanceFadeIfUnhovered()
        {
            if (!MouseHoveringOnMe && ! MouseHoveringOnRelative)
            {
                DistanceFade();
            }
        }
        public void DistanceFadeIfUnclickedAndUnhovered()
        {
            if (!mouseClickingOnMe && !mouseClickingOnRelative && !MouseHoveringOnMe && !MouseHoveringOnRelative)
            {
                DistanceFade();
            }
        }

        // CHILDREN
        virtual protected void HoverChildren () {}
        virtual protected void UnhoverChildren () {}
        virtual protected void ClickChildren() { }
        virtual protected void UnclickChildren() { }
        virtual protected void ColourAndHoverChildren() { }
        virtual protected void UncolourAndUnhoverChildren() { }


        // BUILT IN
        void Start()
        {
            FetchComponents();
        }
        void FixedUpdate()
        {
            previousMousePosDelta = Vector3.Distance(previousMousePos, Input.mousePosition);

            if (previousMousePosDelta > cursorMotionNoticeDistance)
            {
                DistanceFadeIfUnclickedAndUnhovered();
            }

            previousMousePos = Input.mousePosition;
        }

        // MOUSE DRAG
        protected void OnMouseDrag()
        {
            if (previousMousePosDelta >= cursorMotionNoticeDistance)
            {
                Vector3 mouseMovementDelta = Input.mousePosition - previousMousePos;
                MoveMeAndRelatives(mouseMovementDelta);
            }
        }

        // MOUSE CLICK
        protected void OnMouseDown()
        {
            mouseClickingOnMe = true;
            ClickChildren();

            myParentGrid.CursorBusy = true;
        }
        protected void OnMouseUp()
        {
            mouseClickingOnMe = false;
            UnclickChildren();

            myParentGrid.CursorBusy = false;
        }

        // MOUSE HOVER
        private void OnMouseEnter()
        {
            if (!myParentGrid.CursorBusy)
            {
                mouseHoveringOnMe = true;

                SetColour(mySelectedColour);
                ColourAndHoverChildren();
            }
        }
        private void OnMouseExit()
        {
            mouseHoveringOnMe = false;
            UnhoverChildren();
        }
    }
}

