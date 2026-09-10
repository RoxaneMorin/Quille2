using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Building
{
    public enum ControlArrowOrientation
    {
        XPlus,
        XMin,
        YPlus,
        YMin,
        ZPlus,
        ZMin
    }

    // TODO: determine how our target may inform us it's going inactive.
    // TODO: should it instead be unaware of its target, and throw up an event?
    // TODO: rotate the mesh depending on the arrowOrientation?

    public class ControlArrow : MonoBehaviour, IPointerDragAndHoverHandler
    {
        // VARIABLES
        [Header("References")]
        [SerializeField] protected MeshFilter myMeshFilter;
        [SerializeField] protected MeshRenderer myMeshRenderer;
        [SerializeField] protected Material myMaterial;

        [Header("Parameters")]
        [SerializeField] protected ControlArrowOrientation arrowOrientation = ControlArrowOrientation.YPlus;
        [SerializeField] protected Vector3 additionalDeltaFromTarget;

        [SerializeField] protected Color colourDefault = Color.blue;
        [SerializeField] protected Color colourHovered = Color.blue;
        [SerializeField] protected Color colourInUse = Color.cyan;

        // Runtime
        [SerializeField] protected IArrowControllable arrowTarget;

        [Header("Runtime")]
        [SerializeField] protected bool arrowAvailable;
        [SerializeField] protected bool arrowInUse;

        [SerializeField] protected Vector3 prevCursorPos;
        [SerializeField] protected Vector2 cursorPosDelta;


        // PROPERTIES
        public bool ArrowAvailable
        {
            get { return arrowAvailable; }
            set
            {
                arrowAvailable = value;
                gameObject.SetActive(arrowAvailable);
            }
        }
        public bool ArrowInUse
        {
            get { return arrowInUse; } 
            set
            {
                arrowInUse = value;
                myMaterial.color = value ? colourInUse : colourDefault;
            }
        }
        public IArrowControllable ArrowTarget
        {
            get { return arrowTarget; }
            set
            {
                arrowTarget = value;
                ArrowAvailable = arrowTarget != null;

                if (ArrowAvailable)
                {
                    SetPositionFromTarget();
                }
            }
        }


        // METHODS
        public void Init(ControlArrowOrientation arrowOrientation = ControlArrowOrientation.XPlus)
        {
            // Fetch components.
            myMeshFilter = gameObject.GetComponent<MeshFilter>();
            myMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            myMaterial = myMeshRenderer.material;

            // Set parameters
            this.arrowOrientation = arrowOrientation;

            // Set the arrow's availability based on its target.
            ArrowAvailable = ArrowTarget != null;
        }


        // BEHAVIOUR
        protected void BeDragged()
        {
            cursorPosDelta = prevCursorPos - Input.mousePosition;
            prevCursorPos = Input.mousePosition;

            arrowTarget.OnControlArrowAdjustment(this, cursorPosDelta);
        }

        public void SetPosition(Vector3 newPosition, bool addDeltaFromTarget = true)
        {
            if ( addDeltaFromTarget)
            {
                newPosition += additionalDeltaFromTarget;
            }

            transform.position = newPosition;
        }
        public void SetPositionFromTarget(bool addDeltaFromTarget = true)
        {
            if (ArrowTarget != null && ArrowTarget is Component component)
            {
                GameObject targetGameObject = component.gameObject;

                Transform targetsTransform = targetGameObject.transform;
                Vector3 newPosition = targetsTransform.position;

                MeshRenderer targetsMeshRenderer = targetGameObject.GetComponent<MeshRenderer>();
                if (targetsMeshRenderer != null)
                {
                    newPosition.y += SignedVecComponentFromCAO(targetsMeshRenderer.bounds.size);
                }

                SetPosition(newPosition, addDeltaFromTarget);
            }
        }
        public void NudgePosition(Vector3 positionDelta, bool addDeltaFromTarget = true)
        {
            Vector3 newPosition = transform.position;
            newPosition += positionDelta;

            SetPosition(newPosition, addDeltaFromTarget);
        }


        // UTILITY
        protected float SignedVecComponentFromCAO(Vector3 sourceVec)
        {
            return arrowOrientation switch
            {
                ControlArrowOrientation.XPlus => sourceVec.x,
                ControlArrowOrientation.XMin => -sourceVec.x,
                ControlArrowOrientation.YPlus => sourceVec.y,
                ControlArrowOrientation.YMin => -sourceVec.y,
                ControlArrowOrientation.ZPlus => sourceVec.z,
                ControlArrowOrientation.ZMin => -sourceVec.z,
                _ => throw new ArgumentOutOfRangeException(string.Format("Somehow, the inputed ControlArrowOrientation '{0}' is not valid.", nameof(arrowOrientation))),
            };
        }


        protected void RotateArrowMeshFromCAO()
        {
            Mesh myMesh = myMeshFilter.mesh;
            List<Vector3> meshVertices = new List<Vector3>();
            myMesh.GetVertices(meshVertices);

            // TODO: finish this
            // TODO: take into account the mesh's previous orientation

            Quaternion rotation = Quaternion.AngleAxis(90, Vector3.left);

            for (int i = 0; i < meshVertices.Count; i++)
            {
                Vector3 vertex = meshVertices[i];
                meshVertices[i] = rotation * vertex;
            }

            myMesh.SetVertices(meshVertices);
            myMesh.RecalculateBounds();

            // Update collider

        }


        // INTERFACES

        // ->POINTER
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!arrowInUse)
            {
                myMaterial.color = colourHovered;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!arrowInUse)
            {
                myMaterial.color = colourDefault;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (arrowTarget != null && arrowAvailable)
            {
                ArrowInUse = true;
                prevCursorPos = Input.mousePosition;
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            ArrowInUse = false;
        }
        public void OnDrag(PointerEventData data)
        {
            if (arrowTarget != null && arrowInUse)
            {
                BeDragged();
            }
        }
    }
}