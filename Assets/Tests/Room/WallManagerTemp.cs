using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace Building
{
    public class WallManagerTemp : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [Header("Data and References")]
        [SerializeField] private List<WallAnchor> areaWallAnchors;
        [SerializeField] private List<WallSegment> areaWallSegments;

        [SerializeField] private WallAnchor selectedAnchor;


        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;



        // EVENTS
        public event WallAnchorSelected OnWallAnchorSelected;



        // TODO:
        // Upon the creation of a wall segment, use a job to check for intersections with all existing walls.
        // Upon detection, break the wall in two, create a new anchor.



        // METHODS

        // INIT
        public void Init()
        {
            areaWallAnchors = new List<WallAnchor>();
            areaWallSegments = new List<WallSegment>();
        }


         // EVENT LISTENERS
         private void OnWallAnchorClicked(WallAnchor targetAnchor, PointerEventData.InputButton clickType)
        {
            if (clickType == PointerEventData.InputButton.Left)
            {
                if (targetAnchor != selectedAnchor)
                {
                    SelectWallAnchor(targetAnchor);
                }
                else
                {
                    SelectWallAnchor(null);
                }
            }
            else if (clickType == PointerEventData.InputButton.Right)
            {
                if (targetAnchor != selectedAnchor && selectedAnchor != null)
                {
                    CreateWallSegment(targetAnchor, selectedAnchor);
                }
            }
        }


        // UTILITY
        private void SelectWallAnchor(WallAnchor targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);
            selectedAnchor = targetAnchor;
        }

        private void CreateWallAnchor(Vector3 location)
        {
            WallAnchor newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor>();
            newAnchor.Init();

            newAnchor.OnWallAnchorClicked += this.OnWallAnchorClicked;
            this.OnWallAnchorSelected += newAnchor.OnWallAnchorSelected;
            areaWallAnchors.Add(newAnchor);

            if (selectedAnchor != null)
            {
                CreateWallSegment(selectedAnchor, newAnchor);
            }
        }

        private void CreateWallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            WallSegment newSegment = new WallSegment(anchorA, anchorB);
            
            anchorA.AddConnection(newSegment);
            anchorB.AddConnection(newSegment);

            areaWallSegments.Add(newSegment);
        }


        // BUILT IN
        private void Start()
        {
            Init();
        }


        private void OnMouseDown()
        {
            // TODO: use OnPointerDown instead?

            RaycastHit cursorHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out cursorHit))
            {
                CreateWallAnchor(cursorHit.point);
            }
        }


#if DEBUG
        private void OnDrawGizmos()
        {
            foreach (WallSegment segment in areaWallSegments)
            {
                if (segment.AnchorA && segment.AnchorB)
                {
                    // Draw a line representing this wall.
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(segment.AnchorA.transform.position, segment.AnchorB.transform.position);


                    // Draw captions for anchors A and B.
                    Vector3 dirAtoB = (segment.AnchorB.transform.position - segment.AnchorA.transform.position).normalized;
                    Vector3 dirBtoA = (segment.AnchorA.transform.position - segment.AnchorB.transform.position).normalized;

                    Vector3 shiftedPosA = segment.AnchorA.transform.position + dirAtoB * 0.3f;
                    Vector3 shiftedPosB = segment.AnchorB.transform.position + dirBtoA * 0.3f;

                    Handles.Label(shiftedPosA, "A");
                    Handles.Label(shiftedPosB, "B");
                }
            }
        }
#endif
    }

}
