using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public class ControlArrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // VARIABLES
        [Header("References")]
        [SerializeField] protected Material myMaterial;

        [Header("Parameters")]
        [SerializeField] protected Color colourDefault = Color.blue;
        [SerializeField] protected Color colourInUse = Color.cyan;
        [SerializeField] protected ControlArrowOrientation arrowOrientation = ControlArrowOrientation.XPlus;
        [SerializeField] protected Vector3 distanceDeltaFromTarget;

        [Header("Runtime")]
        [SerializeField] protected IArrowControllable arrowTarget;

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
            MeshRenderer myMeshRenderer = gameObject.GetComponent<MeshRenderer>();
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
                newPosition += distanceDeltaFromTarget;
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


        // BUILT IN
        protected void Start()
        {
            //Init();
        }

        protected void Update()
        {
            if (arrowTarget != null && arrowInUse)
            {
                BeDragged();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (arrowTarget !=null && arrowAvailable)
            {
                ArrowInUse = true;
                prevCursorPos = Input.mousePosition;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            ArrowInUse = false;
        }
    }
}