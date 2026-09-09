using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    public partial class WallAnchor_v2 : MonoBehaviour
    {
        // STATIC METHODS
        public static void Connect(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            anchorA.Connect(anchorB);
            anchorB.Connect(anchorA);
        }
        public static void Disconnect(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            anchorA.Disconnect(anchorB);
            anchorB.Disconnect(anchorA);
        }


        // OVERRIDES
        public int CompareTo(object otherObject)
        {
            if (otherObject == null)
            {
                return 1;
            }

            WallAnchor_v2 otherWallAnchor = otherObject as WallAnchor_v2;
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
        public static bool operator >(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            return anchorA.ID > anchorB.ID;
        }
        public static bool operator <(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            return anchorA.ID < anchorB.ID;
        }

        public static bool operator >=(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            return anchorA.ID >= anchorB.ID;
        }
        public static bool operator <=(WallAnchor_v2 anchorA, WallAnchor_v2 anchorB)
        {
            return anchorA.ID <= anchorB.ID;
        }
    }
}

