using AYellowpaper;
using Building;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.FilePathAttribute;

namespace Building
{
    public class WallManagerTempV2 : MonoBehaviour, IPointerDownHandler
    {
        // VARIABLES/PARAMETERS
        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;
        [SerializeField] protected GameObject wallSegmentPrefab;
        [SerializeField] protected GameObject previewObjectPrefab;

        [Header("Data and References")]
        [SerializeField] private int highestAnchorID = -1;
        [SerializeField] private int highestSegmentID = -1;

        [SerializeField] private PreviewObject previewObject;

        [SerializeField] private List<WallAnchor> areaWallAnchors;
        [SerializeField] private List<WallSegment> areaWallSegments;
        // TODO: add list with all connections

        [SerializeField] private WallAnchor selectedAnchor;
        // TODO: Any selectable from the ISelectable interface.


        // PROPERTIES
        private int NextAnchorID
        {
            get
            {
                highestAnchorID++;
                return highestAnchorID;
            }
        }
        private int NextSegmentID
        {
            get
            {
                highestSegmentID++;
                return highestSegmentID;
            }
        }


        // EVENTS
        public event WallAnchorSelected OnWallAnchorSelected;



        // METHODS

        // INIT
        public void Init()
        {
            previewObject = Instantiate(previewObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<PreviewObject>();

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
                    CreateIndividualWallSegment(targetAnchor, selectedAnchor);
                }
            }
        }


        // UTILITY
        private void SelectWallAnchor(WallAnchor targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);
            selectedAnchor = targetAnchor;
        }


        // -> ANCHOR AND SEGMENT CREATION
        private WallAnchor CreateWallAnchor(Vector3 location, bool doSegment = true)
        {
            WallAnchor newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor>();
            newAnchor.Init(NextAnchorID);

            newAnchor.OnWallAnchorClicked += this.OnWallAnchorClicked;
            this.OnWallAnchorSelected += newAnchor.OnWallAnchorSelected;

            areaWallAnchors.Add(newAnchor);

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
                    // Create the new wall segment and its destination anchor.
                    WallAnchor newAnchor = CreateWallAnchor(intersection.Item2, false);
                    CreateIndividualWallSegment(anchorA, newAnchor);

                    // Split the existing intersected segment.
                    SplitWallSegment(intersection.Item1, newAnchor);

                    anchorA = newAnchor;
                }
            }
            // Else, just create the one segment.
            CreateIndividualWallSegment(anchorA, anchorB);
        }

        private void CreateIndividualWallSegment(WallAnchor sourceAnchor, WallAnchor destAnchor)
        {
            Vector3 position = (sourceAnchor.GroundPosition + destAnchor.GroundPosition) / 2f;

            WallSegment newSegment = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegment.Init(NextSegmentID, sourceAnchor, destAnchor);

            areaWallSegments.Add(newSegment);
        }

        private void SplitWallSegment(WallSegment targetSegment, WallAnchor centralAnchor)
        {
            WallAnchor anchorB = targetSegment.AnchorB;

            targetSegment.AnchorB = centralAnchor;
            CreateIndividualWallSegment(anchorB, centralAnchor);
        }

        // TODO: Clean up. Do raycast at middle and top also.
        private List<(WallSegment, Vector3, float)> ListIntersectingWallSegments(WallAnchor anchorA, WallAnchor anchorB)
        {
            List<(WallSegment, Vector3, float)> intersectingSegments = new List<(WallSegment, Vector3, float)>();

            Vector3 basePosA = anchorA.transform.position;
            Vector3 dirAtoBatBase = anchorB.transform.position - anchorA.transform.position;
            float distAtoBatBase = dirAtoBatBase.magnitude;

            //Vector3 topPosA = anchorA.TopPosition;
            //Vector3 dirAtoBatTop = anchorB.TopPosition - anchorA.TopPosition;
            //float distAtoBatTop = dirAtoBatTop.magnitude;

            RaycastHit[] wallHits;
            wallHits = Physics.RaycastAll(basePosA, dirAtoBatBase, distAtoBatBase, (1 << 13));

            foreach (RaycastHit hit in wallHits)
            {
                Vector3 intersectionPoint = hit.point;

                Collider hitCollider = hit.collider;
                GameObject hitGameObject = hitCollider.gameObject;
                WallSegment hitWallSegment = hitGameObject.GetComponent<WallSegment>();

                if (hitWallSegment != null)
                {
                    float distanceFromAnchorA = Vector3.Distance(basePosA, intersectionPoint);
                    intersectingSegments.SortedInsert((hitWallSegment, intersectionPoint, distanceFromAnchorA), (existingIntersection, newIntersection) => existingIntersection.Item3 > newIntersection.Item3);
                }
            }

            return intersectingSegments;
        }






        // BUILT IN
        private void Start()
        {
            Init();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            SelectWallAnchor(CreateWallAnchor(eventData.pointerPressRaycast.worldPosition));
        }

        private void OnMouseOver()
        {
            RaycastHit cursorHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out cursorHit))
            {
                previewObject.transform.position = cursorHit.point;
            }
        }
    }
}

