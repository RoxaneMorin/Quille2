using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        [SerializeField] protected List<int> freeAnchorIDs;
        [SerializeField] protected List<int> freeSegmentIDs;

        [SerializeField] protected List<WallAnchor_v2> areaWallAnchors;
        [SerializeField] protected List<WallSegment_v2> areaWallSegments;
        protected Dictionary<(WallAnchor_v2, WallAnchor_v2), WallSegment_v2> anchorPairsToSegments;


        // PROPERTIES
        //Not sure these two are useful:
        public int HighestAnchorID
        {
            get { return highestAnchorID; }
        }
        public int HighestSegmentID
        {
            get { return highestSegmentID; }
        }

        protected int NextAvailableAnchorID
        {
            get
            {
                int? firstFreeID = TryGetFirstUnusedAnchorID();
                if (firstFreeID != null)
                {
                    return firstFreeID.Value;
                }
                else
                {
                    highestAnchorID++;
                    return highestAnchorID;
                }   
            }
        }
        protected int NextAvailableSegmentID
        {
            get
            {
                int? firstFreeID = TryGetFirstUnusedSegmentID();
                if (firstFreeID != null)
                {
                    return firstFreeID.Value;
                }
                else
                {
                    highestSegmentID++;
                    return highestSegmentID;
                }
            }
        }


        // OTHER SETTERS/GETTERS
        protected bool FreeAnchorID(int theID)
        {
            if (freeAnchorIDs.Contains(theID))
            {
                Debug.Log(string.Format("The ID '{0}' is already in the list of anchor IDs waiting for reuse.", theID));
                return false;
            }

            freeAnchorIDs.SortedInsert(theID, (existingID, newID) => existingID > newID);
            return true;
        }
        protected int? TryGetFirstUnusedAnchorID()
        {
            if (freeAnchorIDs.Count > 0)
            {
                int theID = freeAnchorIDs[0];
                freeAnchorIDs.RemoveAt(0);
                return theID;
            }
            else
            {
                return null;
            }
        }

        protected bool FreeSegmentID(int theID)
        {
            if (freeSegmentIDs.Contains(theID))
            {
                Debug.Log(string.Format("The ID '{0}' is already in the list of segment IDs waiting for reuse.", theID));
                return false;
            }

            freeSegmentIDs.SortedInsert(theID, (existingID, newID) => existingID > newID);
            return true;
        }

        protected int? TryGetFirstUnusedSegmentID()
        {
            if (freeSegmentIDs.Count > 0)
            {
                int theID = freeSegmentIDs[0];
                freeSegmentIDs.RemoveAt(0);
                return theID;
            }
            else
            {
                return null;
            }
        }



        // METHODS

        // INIT
        public void Init()
        {
            // Create containers.
            freeAnchorIDs = new List<int>();
            freeSegmentIDs = new List<int>();

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

            newAnchor.AssignID(NextAvailableAnchorID);
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
            FreeAnchorID(wallAnchor.ID);
            return true;
        }

        // -> ANCHOR PAIRS
        protected bool IsViableAnchorPair((WallAnchor_v2, WallAnchor_v2) anchorPair)
        {
            if (anchorPair.Item1 == null || anchorPair.Item2 == null)
            {
                Debug.Log("This pair of WallAnchors is invalid. One or both are null.");
                return false;
            }
            else if (!IsWallAnchorRegistered(anchorPair.Item1) || !IsWallAnchorRegistered(anchorPair.Item2))
            {
                Debug.Log("This pair of WallAnchors is invalid. One or both are not individually registered with the WallManager.");
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

            if (!IsViableAnchorPair(segmentAnchors)) // ignore invalid pairs
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

        public bool HasSegmentForAnchorPair(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            (WallAnchor_v2, WallAnchor_v2) anchorPair = (anchorA, anchorB);

            if (IsViableAnchorPair(anchorPair))
            {
                return anchorPairsToSegments.ContainsKey(anchorPair);
            }
            else
            {
                return false;
            }
        }
        public WallSegment_v2 GetSegmentForAnchorPair((WallAnchor_v2, WallAnchor_v2) anchorPair)
        {
            if (IsAnchorPairRegistered(anchorPair))
            {
                return anchorPairsToSegments[anchorPair];
            }
            else
            {
                return null;
            }
        }

        // -> WALL SEGMENTS
        protected bool IsWallSegmentRegisteredIgnoreAnchorPair(WallSegment_v2 wallSegment)
        {
            return areaWallSegments.Contains(wallSegment); 
        }
        public bool IsWallSegmentRegistered(WallSegment_v2 wallSegment)
        {
            (WallAnchor_v2, WallAnchor_v2) segmentAnchors = (wallSegment.AnchorA, wallSegment.AnchorB);
            return IsWallSegmentRegisteredIgnoreAnchorPair(wallSegment) && IsAnchorPairRegistered(segmentAnchors);
        }

        public bool RegisterWallSegment(WallSegment_v2 newSegment)
        {
            if (newSegment == null) // ignore null object
            {
                return false;
            }

            // TODO: simplify this section as the checks are already handled by the various subfunctions.
            (WallAnchor_v2, WallAnchor_v2) segmentAnchors = (newSegment.AnchorA, newSegment.AnchorB);
            if (!IsViableAnchorPair(segmentAnchors))
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
            else
            {
                RegisterAnchorPairForSegment(newSegment);
            }

            if (!areaWallSegments.Contains(newSegment))
            {
                newSegment.AssignID(NextAvailableSegmentID);
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

            if (!IsWallSegmentRegisteredIgnoreAnchorPair(wallSegment))
            {
                Debug.Log(string.Format("The WallAnchor '{0}' is not recognised by the WallManager. Nothing to remove.", wallSegment.name));
                return false;
            }

            areaWallSegments.Remove(wallSegment);
            FreeSegmentID(wallSegment.ID);
            return true;
        }

        // TODO: maybe clean up some of the repetitiveness in anchorPair/wallSegment checks.


        // BUILT IN
        private void Start()
        {
            Init();
        }
    }
}

