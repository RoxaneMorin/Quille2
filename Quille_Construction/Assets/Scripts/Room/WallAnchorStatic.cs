using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    public partial class WallAnchor : MonoBehaviour
    {
        // METHODS

        // UTILITIES
        public static void ConnectAnchors(WallAnchor anchorA, WallAnchor anchorB, WallSegment connectingSegment)
        {
            anchorA.AddConnection(anchorB, connectingSegment);
            anchorB.AddConnection(anchorA, connectingSegment);
        }

        public static void DisconnectAnchors(WallAnchor anchorA, WallAnchor anchorB)
        {
            anchorA.DeleteConnection(anchorB);
            anchorB.DeleteConnection(anchorA);
        }


        // OVERRIDES
        public int CompareTo(object otherObject)
        {
            if (otherObject == null)
            {
                return 1;
            }

            WallAnchor otherWallAnchor = otherObject as WallAnchor;
            if (otherWallAnchor != null)
            {
                return this.ID.CompareTo(otherWallAnchor.ID);
            }
            else
            {
                throw new ArgumentException("The otherObject is not a WallAnchor.");
            }
        }

        // OPERATOR OVERLOADS
        public static bool operator >(WallAnchor anchorA, WallAnchor anchorB)
        {
            return anchorA.id > anchorB.id;
        }
        public static bool operator <(WallAnchor anchorA, WallAnchor anchorB)
        {
            return anchorA.id < anchorB.id;
        }

        public static bool operator >=(WallAnchor anchorA, WallAnchor anchorB)
        {
            return anchorA.id >= anchorB.id;
        }
        public static bool operator <=(WallAnchor anchorA, WallAnchor anchorB)
        {
            return anchorA.id <= anchorB.id;
        }
    }
}
