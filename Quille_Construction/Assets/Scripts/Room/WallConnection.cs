using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    [System.Serializable]
    public class WallConnection
    {
        // VARIABLES/PARAMETERS
        [SerializeField] private WallAnchor ownerAnchor;
        [SerializeField] private WallAnchor connectedAnchor;
        [SerializeField] private WallSegment connectedWallSegment;
        [SerializeField] private float angle;
        // Should we also track surrounding connections?


        // PROPERTIES
        public WallAnchor OwnerAnchor { get { return ownerAnchor; } set { ownerAnchor = value; } }
        public WallAnchor ConnectedAnchor { get { return connectedAnchor; } set { connectedAnchor = value; } }
        public WallSegment ConnectedWallSegment { get { return connectedWallSegment; } set { connectedWallSegment = value; } }
        
        public float Angle { get { return angle; } set { angle = value; } }


        // CONSTRUCTOR
        public WallConnection(WallAnchor ownerAnchor, WallAnchor connectedAnchor, WallSegment connectedWallSegment, float angle)
        {
            this.ownerAnchor = ownerAnchor;
            this.connectedAnchor = connectedAnchor;
            this.connectedWallSegment = connectedWallSegment;
            this.angle = angle;
        }
    }
}