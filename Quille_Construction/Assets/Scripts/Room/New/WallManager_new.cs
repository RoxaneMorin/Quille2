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

    public class WallManager_new : MonoBehaviour, IPointerDownHandler
    {
        // VARIABLES/PARAMETERS
        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;
        [SerializeField] protected GameObject wallSegmentPrefab;

        [SerializeField] protected GameObject controlArrowPrefab;
        [SerializeField] protected GameObject previewObjectPrefab;

        [Header("Data and References")]
        [SerializeField] private int highestAnchorID = -1;
        [SerializeField] private int highestSegmentID = -1;

        [SerializeField] private List<WallAnchor_v2> areaWallAnchors;
        [SerializeField] private List<WallSegment_v2> areaWallSegments;

        // TODO: keep a dict of them too?


        [SerializeField] private WallAnchor_v2 selectedAnchor;
        // TODO: Any selectable from the ISelectable interface?

        [SerializeField] private ControlArrow anchorControlArrow;
        // TÒDO: where should the controlArrows live??



        // TODO: have this be some kind of reticule instead?
        [SerializeField] private PreviewObject previewObject;

        [SerializeField] private LineRenderer myLineRenderer;


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
        public event ItemSelected<WallAnchor_v2> OnWallAnchorSelected;



        // METHODS

        // INIT
        public void Init()
        {
            areaWallAnchors = new List<WallAnchor_v2>();

            anchorControlArrow = Instantiate(controlArrowPrefab, Vector3.zero, Quaternion.identity).GetComponent<ControlArrow>();
            anchorControlArrow.Init(ControlArrowOrientation.YPlus);

            previewObject = Instantiate(previewObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<PreviewObject>();
            previewObject.gameObject.SetActive(false);

            myLineRenderer = GetComponent<LineRenderer>();
            myLineRenderer.enabled = false;

        }


        // EVENT LISTENERS
        private void OnWallAnchorClicked(WallAnchor_v2 targetAnchor, PointerEventData.InputButton clickType)
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


        // UTILITY
        private void SelectWallAnchor(WallAnchor_v2 targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);

            selectedAnchor = targetAnchor;
            anchorControlArrow.ArrowTarget = selectedAnchor;

            //// Temp line renderer stuff
            //if (selectedAnchor != null)
            //{
            //    myLineRenderer.enabled = true;
            //    myLineRenderer.SetPosition(0, selectedAnchor.transform.position);
            //}
            //else
            //{
            //    myLineRenderer.enabled = false;
            //}
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
            WallSegment_v2 newSegment = Instantiate(wallSegmentPrefab, anchorA.transform.position, Quaternion.identity).GetComponent<WallSegment_v2>();
            newSegment.Init(NextSegmentID, anchorA, anchorB);

            // Event stuff

            areaWallSegments.Add(newSegment);

            return newSegment;
        }





        // BUILT IN
        private void Start()
        {
            Init();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            WallAnchor_v2 newAnchor = CreateWallAnchor(eventData.pointerPressRaycast.worldPosition);

            if (selectedAnchor != null)
            {
                Debug.Log(selectedAnchor);
                Debug.Log(newAnchor);

                CreateWallSegment(selectedAnchor, newAnchor);
            }

            SelectWallAnchor(newAnchor);
        }




        //// Hacky preview/visualization stuff
        //// TODO: move to a separate system
        //private void OnMouseOver()
        //{
        //    RaycastHit cursorHit;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out cursorHit))
        //    {
        //        previewObject.transform.position = cursorHit.point;

        //        if (selectedAnchor != null)
        //        {
        //            myLineRenderer.SetPosition(1, cursorHit.point);
        //        }
        //    }
        //}

        //private void OnMouseEnter()
        //{
        //    previewObject.gameObject.SetActive(true);
        //    if (selectedAnchor != null)
        //    {
        //        myLineRenderer.enabled = true;
        //    }
        //}
        //private void OnMouseExit()
        //{
        //    previewObject.gameObject.SetActive(false);
        //    myLineRenderer.enabled = false;
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

