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
        private void SelectWallAnchor(WallAnchor targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);
            selectedAnchor = targetAnchor;

            if (targetAnchor != null)
                Debug.Log(string.Format("Selected the WallAnchor {0}", targetAnchor));
            else
                Debug.Log(string.Format("Unselected the current WallAnchor", targetAnchor));
        }


        // -> ANCHOR AND SEGMENT CREATION
        private WallAnchor CreateWallAnchor(Vector3 location, bool doSegment = true)
        {
            highestAnchorID++;

            WallAnchor newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor>();
            newAnchor.Init(highestAnchorID);

            newAnchor.OnWallAnchorClicked += this.OnWallAnchorClicked;
            this.OnWallAnchorSelected += newAnchor.OnWallAnchorSelected;

            areaWallAnchors.Add(newAnchor);

            return newAnchor;
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

