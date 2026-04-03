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
    public class WallManager_new : MonoBehaviour, IPointerDownHandler
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

        [SerializeField] private List<WallAnchor_v2> areaWallAnchors;

        [SerializeField] private WallAnchor_v2 selectedAnchor;
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
        public event ItemSelected<WallAnchor_v2> OnWallAnchorSelected;



        // METHODS

        // INIT
        public void Init()
        {
            previewObject = Instantiate(previewObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<PreviewObject>();

            areaWallAnchors = new List<WallAnchor_v2>();
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





        // BUILT IN
        private void Start()
        {
            Init();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            SelectWallAnchor(CreateWallAnchor(eventData.pointerPressRaycast.worldPosition));
        }

        //private void OnMouseOver()
        //{
        //    RaycastHit cursorHit;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out cursorHit))
        //    {
        //        previewObject.transform.position = cursorHit.point;
        //    }
        //}
    }
}

