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

    // TODO: determine how a target or controller may inform us it's going inactive

    public class ControlArrow : MonoBehaviour, IPointerDragAndHoverHandler
    {
        // VARIABLES
        [Header("References")]
        [SerializeField] protected MeshCollider myCollider;
        [SerializeField] protected MeshFilter myMeshFilter;
        [SerializeField] protected MeshRenderer myMeshRenderer;
        [SerializeField] protected Material myMaterial;
        
        [Header("Parameters")]
        [SerializeField] protected Mesh originalArrowMesh;
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
        public void Init(Vector3 additionalDeltaFromTarget, ControlArrowOrientation arrowOrientation = ControlArrowOrientation.YPlus)
        {
            // Fetch components.
            myCollider = gameObject.GetComponent<MeshCollider>();
            myMeshFilter = gameObject.GetComponent<MeshFilter>();
            myMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            myMaterial = myMeshRenderer.material;

            // Set parameters
            this.additionalDeltaFromTarget = additionalDeltaFromTarget;
            this.arrowOrientation = arrowOrientation;

            // Handle mesh stuff
            if (originalArrowMesh != myMeshFilter.mesh)
            {
                // TODO:make sure it's getting copied properly
                this.originalArrowMesh = Instantiate(myMeshFilter.mesh);
            }
            if (arrowOrientation != ControlArrowOrientation.YPlus) // don't rotate the mesh for the default orientation of YPlus.
            {
                RotateArrowMeshFromCAO();
            }
            
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

                // If the target has a mesh, offset our position by its bounds
                MeshRenderer targetsMeshRenderer = targetGameObject.GetComponent<MeshRenderer>();
                if (targetsMeshRenderer != null && arrowOrientation != ControlArrowOrientation.YMin) // unless the arrow orientation is YMin.
                {
                    Vector3 targetSizeOnAxis = IsolatedVecComponentFromCAO(targetsMeshRenderer.bounds.size);
                    // Half X and Z bound values, assuming the mesh was centered.
                    if (arrowOrientation != ControlArrowOrientation.YPlus)
                    {
                        targetSizeOnAxis /= 2f;
                    }
                    newPosition += targetSizeOnAxis;
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
        protected void RotateArrowMeshFromCAO()
        {
            List<Vector3> referenceMeshVertices = new List<Vector3>();
            originalArrowMesh.GetVertices(referenceMeshVertices);

            Quaternion rotation = MeshRotationFromCAO();
            for (int i = 0; i < referenceMeshVertices.Count; i++)
            {
                Vector3 vertex = referenceMeshVertices[i];
                referenceMeshVertices[i] = rotation * vertex;
            }

            Mesh myMesh = myMeshFilter.mesh;
            myMesh.SetVertices(referenceMeshVertices);
            myMesh.RecalculateBounds();

            myCollider.sharedMesh = myMesh;
        }
        protected Quaternion MeshRotationFromCAO()
        {
            return arrowOrientation switch
            {
                ControlArrowOrientation.XPlus => Quaternion.AngleAxis(90, Vector3.back),
                ControlArrowOrientation.XMin => Quaternion.AngleAxis(-90, Vector3.back),
                ControlArrowOrientation.YPlus => Quaternion.identity,
                ControlArrowOrientation.YMin => Quaternion.AngleAxis(180, Vector3.back),
                ControlArrowOrientation.ZPlus => Quaternion.AngleAxis(90, Vector3.right),
                ControlArrowOrientation.ZMin => Quaternion.AngleAxis(-90, Vector3.right),
                _ => throw new ArgumentOutOfRangeException(string.Format("Somehow, the inputed ControlArrowOrientation '{0}' is not valid.", nameof(arrowOrientation))),
            };
        }

        protected Vector3 IsolatedVecComponentFromCAO(Vector3 sourceVec)
        {
            return arrowOrientation switch
            {
                ControlArrowOrientation.XPlus => new Vector3(sourceVec.x, 0, 0),
                ControlArrowOrientation.XMin => new Vector3(-sourceVec.x, 0, 0),
                ControlArrowOrientation.YPlus => new Vector3(0, sourceVec.y, 0),
                ControlArrowOrientation.YMin => new Vector3(0, -sourceVec.y, 0),
                ControlArrowOrientation.ZPlus => new Vector3(0, 0, sourceVec.z),
                ControlArrowOrientation.ZMin => new Vector3(0, 0, -sourceVec.z),
                _ => throw new ArgumentOutOfRangeException(string.Format("Somehow, the inputed ControlArrowOrientation '{0}' is not valid.", nameof(arrowOrientation))),
            };
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