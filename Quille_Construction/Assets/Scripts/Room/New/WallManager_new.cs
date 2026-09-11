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
    // Manager keeping track of the wall anchors, segments and rooms in a scene.
    // (Will be renamed in the future. Could be a singleton?)
    public class WallManager_new : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [Header("Data and References")]
        [SerializeField] protected int highestAnchorID = -1;
        [SerializeField] protected int highestSegmentID = -1;

        [SerializeField] protected List<WallAnchor_v2> areaWallAnchors;
        [SerializeField] protected List<WallSegment_v2> areaWallSegments;
        protected Dictionary<(WallAnchor_v2, WallAnchor_v2), WallSegment_v2> anchorPairsToSegments;


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


        // TODO: track deleted/freed IDs for reused?
        // Though would this fuck with the ID ordering stuff?



        // METHODS

        // INIT
        public void Init()
        {
            // Create containers.
            areaWallAnchors = new List<WallAnchor_v2>();
            areaWallSegments = new List<WallSegment_v2>();
            anchorPairsToSegments = new Dictionary<(WallAnchor_v2, WallAnchor_v2), WallSegment_v2>();
        }


        // MANAGEMENT

        // -> WALL ANCHORS
        public bool IsWallAnchorRegistered(WallAnchor_v2 wallAnchor)
        {
            return areaWallAnchors.Contains(wallAnchor);
        }
        public bool RegisterWallAnchor(WallAnchor_v2 newAnchor)
        {
            if (newAnchor == null) // ignore null object
            {
                return false;
            }
            else if (IsWallAnchorRegistered(newAnchor))
            {
                Debug.Log(string.Format("The WallAnchor '{0}' is already registered with the WallManager for this scene. It will not be registered twice.", newAnchor.name));
                return false;
            }

            areaWallAnchors.Add(newAnchor);
            return true;
        }
        public bool RemoveWallAnchor(WallAnchor_v2 wallAnchor)
        {
            if (wallAnchor == null) // ignore null object
            {
                return false;
            }
            else if (!IsWallAnchorRegistered(wallAnchor))
            {
                Debug.Log(string.Format("The WallAnchor '{0}' is not recognised by the WallManager. Nothing to remove.", wallAnchor.name));
                return false;
            }

            areaWallAnchors.Remove(wallAnchor);
            return true;
        }

        // -> ANCHOR PAIRS (internal use only)
        protected bool IsValidAnchorPair((WallAnchor_v2, WallAnchor_v2) anchorPair)
        {
            if (anchorPair.Item1 == null || anchorPair.Item2 == null)
            {
                Debug.Log("This pair of WallAnchors is invalid for registrations. One or both are null.");
                return false;
            }
            else if (!IsWallAnchorRegistered(anchorPair.Item1) || !IsWallAnchorRegistered(anchorPair.Item2))
            {
                Debug.Log("This pair of WallAnchors is invalid for registration. One or both are not individually registered with the WallManager.");
                return false;
            }

            return true;
        }
        protected bool IsAnchorPairRegistered((WallAnchor_v2, WallAnchor_v2) anchorPair)
        {
            return anchorPairsToSegments.ContainsKey(anchorPair);
        }
        protected bool RegisterAnchorPairForSegment(WallSegment_v2 newSegment)
        {
            (WallAnchor_v2, WallAnchor_v2) segmentAnchors = (newSegment.AnchorA, newSegment.AnchorB);

            if (!IsValidAnchorPair(segmentAnchors)) // ignore invalid pairs
            {
                return false;
            }
            else if (IsAnchorPairRegistered(segmentAnchors))
            {
                Debug.Log(string.Format("The WallAnchors '{0}' and '{1}' are already registered as the pair for the WallSegment '{2}'.", segmentAnchors.Item1.name, segmentAnchors.Item2.name, anchorPairsToSegments[segmentAnchors]));
                return false;
            }

            anchorPairsToSegments.Add(segmentAnchors, newSegment);
            return true;
        }
        protected bool RemoveAnchorPair((WallAnchor_v2, WallAnchor_v2) anchorPair)
        {
            if (!IsAnchorPairRegistered(anchorPair))
            {
                Debug.Log(string.Format("The pair of WallAnchors '{0}' and '{1}' are not registered with the WallManager. Nothing to remove.", anchorPair.Item1.name, anchorPair.Item2.name));
                return false;
            }

            anchorPairsToSegments.Remove(anchorPair);
            return true;
        }

        // -> WALL SEGMENTS
        protected bool IsWallSegmentRegistered(WallSegment_v2 wallSegment)
        {
            return areaWallSegments.Contains(wallSegment); 
        }
        public bool IsWallSegmentFullyRegistered(WallSegment_v2 wallSegment)
        {
            (WallAnchor_v2, WallAnchor_v2) segmentAnchors = (wallSegment.AnchorA, wallSegment.AnchorB);
            return IsWallSegmentRegistered(wallSegment) && IsAnchorPairRegistered(segmentAnchors);
        }

        public bool RegisterWallSegment(WallSegment_v2 newSegment)
        {
            if (newSegment == null) // ignore null object
            {
                return false;
            }

            (WallAnchor_v2, WallAnchor_v2) segmentAnchors = (newSegment.AnchorA, newSegment.AnchorB);
            if (!IsValidAnchorPair(segmentAnchors))
            {
                Debug.Log(string.Format("The WallSegment '{0}' cannot be registered as one or both of its WallAnchors are not valid or individually registered.", newSegment.name));
            }

            if (IsAnchorPairRegistered(segmentAnchors))
            {
                WallSegment_v2 segmentRegisteredForThisPair = anchorPairsToSegments[segmentAnchors];
                if (segmentRegisteredForThisPair != newSegment)
                {
                    Debug.Log(string.Format("A different WallSegment is already registered with '{0}''s pair of WallAnchors. It cannot be registered.", newSegment.name));
                    return false;
                }
                else
                {
                    Debug.Log(string.Format("The WallSegment '{0}''s pair of WallAnchors is already registered with the WallManager for this scene. It will not be registered twice.", newSegment.name));
                }
            }

            if (!areaWallSegments.Contains(newSegment))
            {
                areaWallSegments.Add(newSegment);
                return true;
            }
            else
            {
                Debug.Log(string.Format("The WallSegment '{0}' is already registered with the WallManager for this scene. It will not be registered twice.", newSegment.name));
                return false;
            }
        }

        public bool RemoveWallSegment(WallSegment_v2 wallSegment)
        {
            if (wallSegment == null) // ignore null object
            {
                return false;
            }

            // First unlist the associated pair of anchors.
            (WallAnchor_v2, WallAnchor_v2) segmentAnchors = (wallSegment.AnchorA, wallSegment.AnchorB);
            RemoveAnchorPair(segmentAnchors);

            if (!IsWallSegmentRegistered(wallSegment))
            {
                Debug.Log(string.Format("The WallAnchor '{0}' is not recognised by the WallManager. Nothing to remove.", wallSegment.name));
                return false;
            }

            areaWallSegments.Remove(wallSegment);
            return true;
        }


        // update/split?


        // BUILT IN
        private void Start()
        {
            Init();
        }
    }
}

