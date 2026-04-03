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

