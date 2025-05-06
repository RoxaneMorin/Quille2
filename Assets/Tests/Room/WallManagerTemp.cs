using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using UnityEditor.MemoryProfiler;
using UnityEngine.UIElements;

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
        [SerializeField] protected GameObject wallSegmentPrefab;


        // EVENTS
        public event WallAnchorSelected OnWallAnchorSelected;



        // TODO:
        // Option to split a wall segment by clicking on it? Will wall segments have physical objects anyways?
        // Room detection!


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
                if (targetAnchor != selectedAnchor && selectedAnchor != null && !targetAnchor.IsConnectedTo(selectedAnchor))
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

        private WallAnchor CreateWallAnchor(Vector3 location, bool doSegment = true)
        {
            WallAnchor newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor>();
            newAnchor.Init();

            newAnchor.OnWallAnchorClicked += this.OnWallAnchorClicked;
            this.OnWallAnchorSelected += newAnchor.OnWallAnchorSelected;
            areaWallAnchors.Add(newAnchor);

            // TODO: can we toggle whether the wall gets selected after its creation?
            // Atm, it "gets clicked" which throws the event.

            //Debug.Log(String.Format("Created a new wall anchor: {0}", newAnchor));

            if (doSegment && selectedAnchor != null)
            {
                CreateWallSegment(selectedAnchor, newAnchor);
            }

            return newAnchor;
        }

        private void CreateWallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            // Collect potential intersections with other segments.
            List<(WallSegment, Vector3, float)> intersectingSegments = ListIntersectingWallSegments(anchorA, anchorB);

            if (intersectingSegments.Count > 0)
            {
                // If there are any, split this new segment in consequence.
                foreach ((WallSegment, Vector3, float) intersection in intersectingSegments)
                {
                    WallAnchor newAnchor = CreateWallAnchor(intersection.Item2, false);
                    CreateSingleWallSegment(anchorA, newAnchor);

                    SplitWallSegment(intersection.Item1, newAnchor);

                    anchorA = newAnchor;
                }
            }
            // Else, just create the one segment.

            CreateSingleWallSegment(anchorA, anchorB);
        }

        private void CreateSingleWallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            WallSegment newSegment = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegment.Init(anchorA, anchorB);

            anchorA.AddConnection(newSegment);
            anchorB.AddConnection(newSegment);

            areaWallSegments.Add(newSegment);
        }

        private void SplitWallSegment(WallSegment targetSegment, WallAnchor centralAnchor)
        {
            WallSegment newSegmentA = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegmentA.Init(targetSegment.AnchorA, centralAnchor);
           
            WallSegment newSegmentB = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegmentB.Init(centralAnchor, targetSegment.AnchorB);

            targetSegment.AnchorA.ReplaceConnection(targetSegment, centralAnchor, newSegmentA);
            targetSegment.AnchorB.ReplaceConnection(targetSegment, centralAnchor, newSegmentB);

            centralAnchor.AddConnection(newSegmentA);
            centralAnchor.AddConnection(newSegmentB);

            areaWallSegments.Add(newSegmentA);
            areaWallSegments.Add(newSegmentB);

            areaWallSegments.Remove(targetSegment);
            Destroy(targetSegment.gameObject);
        }




        private List<(WallSegment, Vector3, float)> ListIntersectingWallSegments(WallAnchor anchorA, WallAnchor anchorB)
        {
            List<(WallSegment, Vector3, float)> intersectingSegments = new List<(WallSegment, Vector3, float)>();

            foreach (WallSegment otherSegment in areaWallSegments)
            {
                // TODO: Further skip segments we know won't intersect.

                // Skip segments connected to the same anchors.
                if (anchorA == otherSegment.AnchorA || anchorA == otherSegment.AnchorB || anchorB == otherSegment.AnchorA || anchorB == otherSegment.AnchorB)
                {
                    continue;
                }

                Vector3? intersectionPoint = Building_MathHelpers.CalculatePotentialIntersectionPointXZ(anchorA.transform.position, anchorB.transform.position, otherSegment.AnchorA.transform.position, otherSegment.AnchorB.transform.position);
                if (intersectionPoint.HasValue)
                {
                    float distanceFromAnchorA = Vector3.Distance(anchorA.transform.position, intersectionPoint.Value);
                    intersectingSegments.SortedInsert((otherSegment, intersectionPoint.Value, distanceFromAnchorA), (existingIntersection, newIntersection) => existingIntersection.Item3 > newIntersection.Item3);
                }
            }

            return intersectingSegments;
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
