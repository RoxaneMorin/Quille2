using AYellowpaper;
using Building;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    // TODO: separate wall creation and management into two different controllers.

    // TODO: detect intersections, do wall splits

    public class WallManager_new : MonoBehaviour, IPointerClickAndHoverHandler
    {
        // VARIABLES/PARAMETERS
        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;
        [SerializeField] protected GameObject wallSegmentPrefab;

        [SerializeField] protected GameObject controlArrowPrefab;
        [SerializeField] protected GameObject previewObjectPrefab;

        [Header("Data and References")]
        [SerializeField] protected int highestAnchorID = -1;
        [SerializeField] protected int highestSegmentID = -1;

        [SerializeField] protected List<WallAnchor_v2> areaWallAnchors;
        [SerializeField] protected List<WallSegment_v2> areaWallSegments;

        protected Dictionary<(WallAnchor_v2, WallAnchor_v2), WallSegment_v2> anchorPairsToSegments;


        [SerializeField] protected WallAnchor_v2 selectedAnchor;
        // TODO: Any selectable from the ISelectable interface?

        [SerializeField] protected ControlArrow anchorControlArrow;
        // TÒDO: where should the controlArrows live??



        // TODO: have this be some kind of reticule instead?
        [SerializeField] protected PreviewObject previewObject;

        [SerializeField] protected LineRenderer myLineRenderer;


        // PROPERTIES
        protected int HighestAnchorID
        {
            get { return highestAnchorID; }
        }
        protected int HighestSegmentID
        {
            get { return highestSegmentID; }
        }

        protected int NextAnchorID
        {
            get
            {
                highestAnchorID++;
                return highestAnchorID;
            }
        }
        protected int NextSegmentID
        {
            get
            {
                highestSegmentID++;
                return highestSegmentID;
            }
        }


        // EVENTS
        public event ItemSelected<WallAnchor_v2> OnWallAnchorSelected;



        // METHODS

        // INIT
        public void Init()
        {
            // Create containers.
            areaWallAnchors = new List<WallAnchor_v2>();
            areaWallSegments = new List<WallSegment_v2>();
            anchorPairsToSegments = new Dictionary<(WallAnchor_v2, WallAnchor_v2), WallSegment_v2>();


            // To review
            anchorControlArrow = Instantiate(controlArrowPrefab, Vector3.zero, Quaternion.identity).GetComponent<ControlArrow>();
            anchorControlArrow.Init(new Vector3(0, 0.1f, 0));

            previewObject = Instantiate(previewObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<PreviewObject>();
            previewObject.gameObject.SetActive(false);

            myLineRenderer = GetComponent<LineRenderer>();
            DeactivateLineRenderer();

        }


        // EVENT LISTENERS
        private void OnWallAnchorClicked(WallAnchor_v2 targetAnchor, PointerEventData.InputButton clickType)
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


        // UTILITY
        private void SelectWallAnchor(WallAnchor_v2 targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);

            selectedAnchor = targetAnchor;
            anchorControlArrow.ArrowTarget = selectedAnchor;

            // Temp line renderer stuff
            if (selectedAnchor != null)
            {
                myLineRenderer.enabled = true;
                myLineRenderer.SetPosition(0, selectedAnchor.PosAtBase);
                myLineRenderer.SetPosition(1, selectedAnchor.PosAtBase);
            }
            else
            {
                myLineRenderer.enabled = false;
                myLineRenderer.SetPosition(0, Vector3.zero);
                myLineRenderer.SetPosition(1, Vector3.zero);
            }
        }


        // -> ANCHOR AND SEGMENT CREATION
        private WallAnchor_v2 CreateWallAnchor(Vector3 location)
        {
            WallAnchor_v2 newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor_v2>();
            newAnchor.Init(NextAnchorID);

            newAnchor.OnClicked += this.OnWallAnchorClicked;
            this.OnWallAnchorSelected += newAnchor.OnWallAnchorSelected;

            areaWallAnchors.Add(newAnchor);

            return newAnchor;
        }

        private WallSegment_v2 CreateWallSegment(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            // Always start from the lowest ID anchor.
            ExtensionMethods.SwapIfGreater(ref anchorA, ref anchorB);

            // Do not recreate existing wall segments.
            if (!anchorPairsToSegments.ContainsKey((anchorA, anchorB)))
            {
                WallSegment_v2 newSegment = Instantiate(wallSegmentPrefab, anchorA.transform.position, Quaternion.identity).GetComponent<WallSegment_v2>();
                newSegment.Init(NextSegmentID, anchorA, anchorB);

                // Event subscriptions

                areaWallSegments.Add(newSegment);
                anchorPairsToSegments.Add((anchorA, anchorB), newSegment);

                return newSegment;
            }
            else
            {
                Debug.Log(string.Format("A wall segment already exists between anchors '{0}' and '{1}'. The CreateWallSegment function will return it instead.", anchorA, anchorB));
                return anchorPairsToSegments[(anchorA, anchorB)];
            } 
        }




        // BUILT IN
        private void Start()
        {
            Init();
        }


        // OR: do drag and on pointer release for previewing?


        public void OnPointerClick(PointerEventData eventData)
        {
            WallAnchor_v2 newAnchor = CreateWallAnchor(eventData.pointerPressRaycast.worldPosition);

            if (selectedAnchor != null && selectedAnchor != newAnchor)
            {
                CreateWallSegment(selectedAnchor, newAnchor);
            } 

            SelectWallAnchor(newAnchor);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (selectedAnchor != null)
            {
                myLineRenderer.SetPosition(0, selectedAnchor.PosAtBase);
                myLineRenderer.enabled = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DeactivateLineRenderer();
        }

        // Hacky preview/visualization stuff
        // TODO: move to a separate system
        private void OnMouseOver()
        {
            RaycastHit cursorHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out cursorHit))
            {
                if (selectedAnchor != null)
                {
                    myLineRenderer.SetPosition(1, cursorHit.point);
                }
                else
                {
                    myLineRenderer.SetPosition(1, Vector3.zero);
                }
            }
        }

        private void DeactivateLineRenderer()
        {
            myLineRenderer.enabled = false;
            myLineRenderer.SetPosition(0, Vector3.zero);
            myLineRenderer.SetPosition(1, Vector3.zero);
        }


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

