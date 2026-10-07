using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    // TODO: detect intersections, do wall splits

    // TODO: cleanup and deletion of anchors and segments

    // TODO: separate wall creation and registering to the wallManager?

    public class WallTools : MonoBehaviour, IPointerClickAndHoverHandler
    {
        // VARIABLES/PARAMETERS
        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;
        [SerializeField] protected GameObject wallSegmentPrefab;

        [SerializeField] protected GameObject controlArrowPrefab;
        [SerializeField] protected GameObject previewObjectPrefab;

        [Header("References")]
        [SerializeField] protected WallManager_new wallManager;

        [SerializeField] protected LinePlacementVisualizer placementVisualizer;

        [SerializeField] protected WallAnchor_v2 selectedAnchor;
        // TODO: Any selectable from the ISelectable interface?

        [SerializeField] protected ControlArrow anchorControlArrow;
        // TÒDO: where should the controlArrows live??



        // EVENTS
        public event ItemSelected<WallAnchor_v2> OnWallAnchorSelected;



        // METHODS

        // INIT
        public void Init()
        {  
            // Find the WallManager
            if (wallManager is null)
            {
                wallManager = gameObject.GetComponent<WallManager_new>() ?? FindFirstObjectByType<WallManager_new>();
            }

            // To review
            if (placementVisualizer is null)
            {
                placementVisualizer = gameObject.GetComponentInChildren<LinePlacementVisualizer>() ?? FindFirstObjectByType<LinePlacementVisualizer>();
            }


            // To review
            anchorControlArrow = Instantiate(controlArrowPrefab, Vector3.zero, Quaternion.identity).GetComponent<ControlArrow>();
            anchorControlArrow.Init(new Vector3(0, 0.1f, 0));
        }


        // CLICK LISTENERS
        protected void OnWallAnchorClicked(WallAnchor_v2 targetAnchor, PointerEventData.InputButton clickType)
        {
            // On right clicks, try to connect the clicked and currently selected anchors
            if (clickType == PointerEventData.InputButton.Right && selectedAnchor != null)
            {
                CreateWallSegment(selectedAnchor, targetAnchor);
            }
            else // (un)select the clicked anchor
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
        }

        protected void OnFreeGroundClicked(Vector3 location)
        {
            WallAnchor_v2 newAnchor = CreateWallAnchor(location);

            if (selectedAnchor != null && selectedAnchor != newAnchor)
            {
                CreateWallSegment(selectedAnchor, newAnchor);
            }

            SelectWallAnchor(newAnchor);
        }



        // UTILITY
        protected void SelectWallAnchor(WallAnchor_v2 targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);

            selectedAnchor = targetAnchor;
            anchorControlArrow.ArrowTarget = selectedAnchor;

            placementVisualizer.SetLineSource(targetAnchor == null? null : targetAnchor.gameObject);
        }



        // -> ANCHOR CREATION
        protected WallAnchor_v2 CreateWallAnchor(Vector3 location)
        {
            WallAnchor_v2 newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor_v2>();
            newAnchor.Init();

            newAnchor.OnClicked += this.OnWallAnchorClicked;
            this.OnWallAnchorSelected += newAnchor.OnWallAnchorSelected;

            wallManager.RegisterWallAnchor(newAnchor);

            return newAnchor;
        }


    // -> SEGMENT CREATION
        protected WallSegment_v2 CreateWallSegment(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            // Always start from the lowest ID anchor.
            ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

            // Do not recreate existing wall segments.
            if (WouldBeNewAndValidWallSegment(anchorA, anchorB))
            {
                // Account for potential intersections with other segments.
                List<(WallSegment_v2, Vector3, float)> intersectedSegments = FindPotentialIntersectedSegments(anchorA, anchorB);
                if (intersectedSegments.Count > 0)
                {
                    WallAnchor_v2 anchorAtSplit;
                    foreach ((WallSegment_v2, Vector3, float) intersection in intersectedSegments)
                    {
                        anchorAtSplit = SplitExistingWallSegment(intersection.Item1, intersection.Item2);
                        CreateIndividualWallSegment(anchorA, anchorAtSplit, true);
                        anchorA = anchorAtSplit;
                    }
                }
                // Else and beyond, just create the one segment.
                return CreateIndividualWallSegment(anchorA, anchorB, true);
            }
            else
            {
                Debug.Log(string.Format("Could not create a wall segment between anchors '{0}' and '{1}'.", anchorA, anchorB));
                return null;
            }
        }
        protected WallAnchor_v2 SplitExistingWallSegment(WallSegment_v2 targetSegment, Vector3 splitAt)
        {
            WallAnchor_v2 anchorA = targetSegment.AnchorA;
            WallAnchor_v2 anchorB = targetSegment.AnchorB;

            DeleteWallSegment(targetSegment);

            WallAnchor_v2 anchorAtSplit = CreateWallAnchor(splitAt);
            CreateIndividualWallSegment(anchorA, anchorAtSplit, true);
            CreateIndividualWallSegment(anchorB, anchorAtSplit, true);

            return anchorAtSplit;
        }
        protected WallSegment_v2 CreateIndividualWallSegment(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB, bool bypassValidityCheck = false)
        {
            // Always start from the lowest ID anchor.
            ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

            // Do not recreate existing wall segments.
            if (bypassValidityCheck || WouldBeNewAndValidWallSegment(anchorA, anchorB))
            {
                WallSegment_v2 newSegment = Instantiate(wallSegmentPrefab, anchorA.transform.position, Quaternion.identity).GetComponent<WallSegment_v2>();
                newSegment.Init(anchorA, anchorB);

                // Event subscriptions

                wallManager.RegisterWallSegment(newSegment);

                return newSegment;
            }
            else
            {
                Debug.Log(string.Format("A wall segment already exists between anchors '{0}' and '{1}'. The CreateWallSegment function will return it instead.", anchorA, anchorB));
                return wallManager.GetSegmentForAnchorPair((anchorA, anchorB));
            }
        }
        protected void DeleteWallSegment(WallSegment_v2 targetSegment)
        {
            if (targetSegment != null && wallManager.RemoveWallSegment(targetSegment))
            {
                Destroy(targetSegment.gameObject);
            }
        }

        protected List<(WallSegment_v2, Vector3, float)> FindPotentialIntersectedSegments(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            List<(WallSegment_v2, Vector3, float)> intersectedSegments = new List<(WallSegment_v2, Vector3, float)>();

            RaycastHit[] segmentHits;
            Vector3 dirAtoB = anchorB.PosAtBase - anchorA.PosAtBase;
            segmentHits = Physics.RaycastAll(anchorA.PosAtBase, dirAtoB, dirAtoB.magnitude, 1 << 13);
            // Should we also check their middles and tops?

            foreach (RaycastHit hit in segmentHits)
            {
                Vector3 intersectionPoint = hit.point;
                if (intersectionPoint.RoughlyEquals(anchorA.PosAtBase) || intersectionPoint.RoughlyEquals(anchorB.PosAtBase))
                {
                    continue; // Discard hits to segments connected to the anchors.
                }

                Collider hitCollider = hit.collider;
                GameObject hitGameObject = hitCollider.gameObject;
                WallSegment_v2 hitSegment =  hitGameObject.GetComponent<WallSegment_v2>();
                if (hitSegment != null)
                {
                    float distanceFromAnchorA = Vector3.Distance(anchorA.PosAtBase, intersectionPoint);
                    intersectedSegments.SortedInsert((hitSegment, intersectionPoint, distanceFromAnchorA), (existingIntersection, newIntersection) => existingIntersection.Item3 > newIntersection.Item3);
                }
            }

            return intersectedSegments;
        }


        protected bool WouldBeNewAndValidWallSegment(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            // Always start from the lowest ID anchor.
            ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

            // Ensure the two segments are actually different.
            if (anchorA == anchorB)
            {
                return false;
            }

            // Check whether this exact connection already exist.
            if (wallManager.HasSegmentForAnchorPair(anchorA, anchorB))
            {
                return false;
            }

            // Check whether another connection with the same angle already exists.
            float newAngleAToB = MathHelpers.GetNormalizeAngleBetweenInDegrees(anchorA.PosAtBase, anchorB.PosAtBase);
            float newAngleBtoA = MathHelpers.GetNormalizeAngleBetweenInDegrees(anchorB.PosAtBase, anchorA.PosAtBase);
            return !anchorA.HasConnectionAtAngle(newAngleAToB) && !anchorB.HasConnectionAtAngle(newAngleBtoA);

            // TODO: may still throw errors if the angles are very similar but not quite exact :/
        }

        


        // BUILT IN
        private void Start()
        {
            Init();
        }

        // OR: do drag and on pointer release for previewing?
        public void OnPointerEnter(PointerEventData eventData)
        {
            placementVisualizer.gameObject.SetActive(true);

        }
        public void OnPointerExit(PointerEventData eventData)
        {
            placementVisualizer.gameObject.SetActive(false);

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnFreeGroundClicked(eventData.pointerPressRaycast.worldPosition);
        }

        


        //// TODO: try snap to a certain angle from the center
        //private Vector3 SnapAngle(Vector3 endPoint, float snap = 22.5f)
        //{
        //    float snapRad = Mathf.Deg2Rad * snap;

        //    float lineAngle = MathHelpers.GetNormalizedAngleBetween(selectedAnchor.PosAtBase, endPoint);
        //    float snappedLineAngle = Mathf.Round(lineAngle / snapRad) * snapRad;

        //    float distance = Vector3.Distance(selectedAnchor.PosAtBase, endPoint);

        //    Vector3 test = Quaternion.Euler(0.0f, -snappedLineAngle * Mathf.Rad2Deg, 0.0f) * new Vector3(distance, 0.0f, 0.0f);

        //    return selectedAnchor.PosAtBase + test + new Vector3(0f, 0.1f, 0f);
        //}



        //#if DEBUG
        //        private void OnDrawGizmos()
        //        {
        //            if (selectedAnchor != null)
        //            {
        //                Debug.DrawLine(selectedAnchor.transform.position, previewObject.transform.position);
        //            }
        //        }
        //#endif
    }

}
