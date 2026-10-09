using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.FilePathAttribute;

namespace Building
{
    // TODO: separate wall creation and registering to the wallManager?

    // TODO: add safety checks for a new segment potentially hitting existing but unconnected anchors
    // TODO: add safety checks for a new segment potentially creating overly steep angles

    // TODO: add more visual feedback for potential actions, like the anchors that would be linked by a click, and indication that a segment won't be created as it would not be valid.

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
                ListWallStuffHitBetweenSelectedAndLocation(targetAnchor.PosAtBase);

                TryCreateWallSegment(selectedAnchor, targetAnchor);
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
        protected void OnFreeGroundClicked(Vector3 clickedLocation)
        {
            WallAnchor_v2 newAnchor;
            if (selectedAnchor == null)
            {
                newAnchor = CreateIndividualWallAnchor(clickedLocation);
            }
            else
            {
                ListWallStuffHitBetweenSelectedAndLocation(clickedLocation);

                newAnchor = TryCreateWallAnchorAndSegment(clickedLocation);
            }
            
            SelectWallAnchor(newAnchor);
        }





        // TODO: actually do smt with the collected hits.
        protected void ListWallStuffHitBetweenSelectedAndLocation(Vector3 targetLocation)
        {
            if (selectedAnchor != null)
            {
                RaycastHit[] raycastHits;
                Vector3 dirSelectedToTarget = targetLocation - selectedAnchor.PosAtBase;
                Vector3 dirNormalized = dirSelectedToTarget.normalized;
                raycastHits = Physics.RaycastAll(selectedAnchor.PosAtBase, dirNormalized, dirSelectedToTarget.magnitude, (1 << 13) | (1 << 14));

                Debug.Log($"Start: {selectedAnchor.PosAtBase}. End: {targetLocation}. Ray: {dirNormalized}.");

                foreach (RaycastHit hit in raycastHits)
                {
                    GameObject hitGameObject = hit.collider.gameObject;

                    WallAnchor_v2 hitAnchor = hitGameObject.GetComponent<WallAnchor_v2>();
                    if (hitAnchor != null)
                    {
                        Debug.Log($"Hit the anchor {hitAnchor.name} at {hit.point}.");
                    }
                    
                    WallSegment_v2 hitSegment = hitGameObject.GetComponent<WallSegment_v2>();
                    if (hitSegment != null)
                    { 
                        float dotProduct = Mathf.Abs(Vector3.Dot(hit.normal, dirNormalized));

                        Debug.Log($"Hit the segment {hitSegment.name} at {hit.point}. Surface normal: {hit.normal}. DotProduct: {dotProduct}.");
                    }
                }
            }
        }






        // ITEM CREATION

        // -> Individual items
        protected WallAnchor_v2 CreateIndividualWallAnchor(Vector3 location, float height = 1.0f)
        {
            WallAnchor_v2 newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor_v2>();
            newAnchor.Init(height);

            newAnchor.OnClicked += this.OnWallAnchorClicked;
            this.OnWallAnchorSelected += newAnchor.OnWallAnchorSelected;

            wallManager.RegisterWallAnchor(newAnchor);

            return newAnchor;
        }
        protected WallSegment_v2 CreateIndividualWallSegment(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            // Always start from the lowest ID anchor.
            ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

            WallSegment_v2 newSegment = Instantiate(wallSegmentPrefab, anchorA.transform.position, Quaternion.identity).GetComponent<WallSegment_v2>();
            newSegment.Init(anchorA, anchorB);

            // Event subscriptions

            wallManager.RegisterWallSegment(newSegment);

            return newSegment;
        }

        // -> 
        protected WallAnchor_v2 TryCreateWallAnchorAndSegment(Vector3 newAnchorLocation)
        {
            if (WouldBeNewAndLegalAngleForAnchor(selectedAnchor, newAnchorLocation))
            {
                WallAnchor_v2 newAnchor = CreateIndividualWallAnchor(newAnchorLocation);
                WallSegment_v2 newSegment = TryCreateWallSegment(selectedAnchor, newAnchor);
                if (newSegment == null)
                {
                    DeleteWallAnchor(newAnchor);
                    return null;
                }
                return newAnchor;
            }
            else
            {
                return null;
            }
        }
        protected WallSegment_v2 TryCreateWallSegment(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            // Always start from the lowest ID anchor.
            ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

            if (wallManager.HasSegmentForAnchorPair(anchorA, anchorB))
            {
                Debug.Log(string.Format("A WallSegment already exists between anchors '{0}' and '{1}'. It will be returned instead.", anchorA.name, anchorB.name));
                return wallManager.GetSegmentForAnchorPair((anchorA, anchorB));
            }

            if (WouldBeValidWallSegment(anchorA, anchorB))
            {
                List<(WallSegment_v2, Vector3, float)> intersectedSegments = FindPotentialIntersectedSegments(anchorA, anchorB);

                if (intersectedSegments.Count > 0)
                {
                    WallAnchor_v2 anchorAtSplit;
                    foreach ((WallSegment_v2, Vector3, float) intersection in intersectedSegments)
                    {
                        anchorAtSplit = SplitExistingWallSegment(intersection.Item1, intersection.Item2);
                        CreateIndividualWallSegment(anchorA, anchorAtSplit);
                        anchorA = anchorAtSplit;
                    }
                }
                return CreateIndividualWallSegment(anchorA, anchorB);
            }
            else
            {
                Debug.Log(string.Format("A WallSegment between anchors '{0}' and '{1}' would not be valid and was not created.", anchorA.name, anchorB.name));
                return null;
            }
        }
        protected WallAnchor_v2 SplitExistingWallSegment(WallSegment_v2 targetSegment, Vector3 splitAt)
        {
            WallAnchor_v2 anchorA = targetSegment.AnchorA;
            WallAnchor_v2 anchorB = targetSegment.AnchorB;

            DeleteWallSegment(targetSegment);

            float splitAtLerpValue = ExtensionMethods.Vector3InverseLerp(anchorA.PosAtBase, anchorB.PosAtBase, splitAt);
            float heigthAtSplitPoint = Mathf.Lerp(anchorA.Height, anchorB.Height, splitAtLerpValue);
            WallAnchor_v2 anchorAtSplit = CreateIndividualWallAnchor(splitAt, heigthAtSplitPoint);

            CreateIndividualWallSegment(anchorA, anchorAtSplit);
            CreateIndividualWallSegment(anchorB, anchorAtSplit);

            return anchorAtSplit;
        }


        // -> Checks
        protected bool WouldBeNewAndLegalAngleForAnchor(WallAnchor_v2 existingAnchor, Vector3 newAnchorLocation)
        {
            float angleExistingToLocation = MathHelpers.GetNormalizeAngleBetweenInDegrees(existingAnchor.PosAtBase, newAnchorLocation);
            return existingAnchor.WouldBeNewAndLegalConnectionAngle(angleExistingToLocation);
        }

        protected bool WouldBeValidWallSegment(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            // Always start from the lowest ID anchor.
            ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

            if (anchorA == anchorB)
            {
                return false;
            }

            if (wallManager.HasSegmentForAnchorPair(anchorA, anchorB))
            {
                return false;
            }

            float newAngleAToB = MathHelpers.GetNormalizeAngleBetweenInDegrees(anchorA.PosAtBase, anchorB.PosAtBase);
            float newAngleBtoA = MathHelpers.GetNormalizeAngleBetweenInDegrees(anchorB.PosAtBase, anchorA.PosAtBase);

            return anchorA.WouldBeNewAndLegalConnectionAngle(newAngleAToB) && anchorB.WouldBeNewAndLegalConnectionAngle(newAngleBtoA);
        }


        protected List<(WallSegment_v2, Vector3, float)> FindPotentialIntersectedSegments(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            List<(WallSegment_v2, Vector3, float)> intersectedSegments = new List<(WallSegment_v2, Vector3, float)>();

            RaycastHit[] segmentHits;
            Vector3 dirAtoB = anchorB.PosAtBase - anchorA.PosAtBase;
            segmentHits = Physics.RaycastAll(anchorA.PosAtBase, dirAtoB, dirAtoB.magnitude, 1 << 14);

            foreach (RaycastHit hit in segmentHits)
            {
                Collider hitCollider = hit.collider;
                GameObject hitGameObject = hitCollider.gameObject;
                WallSegment_v2 hitSegment = hitGameObject.GetComponent<WallSegment_v2>();

                if (hitSegment != null)
                {
                    if (hitSegment.AnchorA == anchorA || hitSegment.AnchorB == anchorA || hitSegment.AnchorA == anchorB || hitSegment.AnchorB == anchorB)
                    {
                        //Debug.Log($"The segment {hitSegment.name} disqualified as a potential intersection as it's connected to either {anchorA.name} or {anchorB}.");
                        continue; // Ignore segments connected to either of the anchors.
                    }

                    Vector3 intersectionPoint = hit.point;
                    float distanceFromAnchorA = Vector3.Distance(anchorA.PosAtBase, intersectionPoint);
                    intersectedSegments.SortedInsert((hitSegment, intersectionPoint, distanceFromAnchorA), (existingIntersection, newIntersection) => existingIntersection.Item3 > newIntersection.Item3);
                }
            }

            return intersectedSegments;
        }



        // ITEM DELETION
        protected void DeleteWallAnchor(WallAnchor_v2 targetAnchor)
        {
            if (targetAnchor != null && wallManager.RemoveWallAnchor(targetAnchor))
            {
                targetAnchor.OnClicked -= this.OnWallAnchorClicked;

                Destroy(targetAnchor.gameObject);
            }
        }
        protected void DeleteWallSegment(WallSegment_v2 targetSegment)
        {
            if (targetSegment != null && wallManager.RemoveWallSegment(targetSegment))
            {
                // TODO: unregister events when  they get added.

                Destroy(targetSegment.gameObject);
            }
        }



        // UTILITY
        protected void SelectWallAnchor(WallAnchor_v2 targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);

            selectedAnchor = targetAnchor;
            anchorControlArrow.ArrowTarget = selectedAnchor;

            placementVisualizer.SetLineSource(targetAnchor == null ? null : targetAnchor.gameObject);
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
