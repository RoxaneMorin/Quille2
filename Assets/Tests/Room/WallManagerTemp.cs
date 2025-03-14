using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Building
{
    public class WallManagerTemp : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [Header("Data and References")]
        [SerializeField] private List<WallAnchor> sceneWallAnchors;
        [SerializeField] private List<WallSegment> sceneWallSegments;

        [SerializeField] private WallAnchor selectedAnchor;



        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;






        // TODO:
        // click on an anchor to select it, click it again to unselect



        // How to detect intersections between wall segments?

        // Upon detection, break the wall in two, create a new anchor.







        // METHODS

        // INIT
        public void Init()
        {
            sceneWallAnchors = new List<WallAnchor>();
            sceneWallSegments = new List<WallSegment>();
        }


        //
        private void CreateWallAnchor(Vector3 location)
        {
            WallAnchor newAnchor = Instantiate(wallAnchorPrefab, location, Quaternion.identity).GetComponent<WallAnchor>();
            newAnchor.Init();

            // TODO: other stuff

            if (selectedAnchor != null)
            {
                WallSegment newSegment = new WallSegment(selectedAnchor, newAnchor);
                selectedAnchor.AddConnection(newSegment);
                newAnchor.AddConnection(newSegment);
                sceneWallSegments.Add(newSegment);
            }

            sceneWallAnchors.Add(newAnchor);
            selectedAnchor = newAnchor;
        }



        // BUILT IN

        private void Start()
        {
            Init();
        }



        private void OnMouseDown()
        {
            RaycastHit cursorHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out cursorHit))
            {
                Debug.Log(string.Format("The WallManager was clicked at the point {0}.", cursorHit.point));

                CreateWallAnchor(cursorHit.point);
            }
        }




#if DEBUG
        private void OnDrawGizmos()
        {
            foreach (WallSegment segment in sceneWallSegments)
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
