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
        [SerializeField] private WallAnchor ownerWallAnchor;
        [SerializeField] private WallAnchor connectedWallAnchor;
        [SerializeField] private WallSegment connectedWallSegment;
        [SerializeField] private float angle;
        // Should we also track surrounding connections?


        // PROPERTIES
        public WallAnchor OwnerAnchor { get { return ownerWallAnchor; } set { ownerWallAnchor = value; } }
        public WallAnchor ConnectedAnchor { get { return connectedWallAnchor; } set { connectedWallAnchor = value; } }
        public WallSegment ConnectedSegment { get { return connectedWallSegment; } set { connectedWallSegment = value; } }
        
        public float Angle { get { return angle; } set { angle = value; } }


        // CONSTRUCTOR
        public WallConnection(WallAnchor ownerAnchor, WallAnchor connectedAnchor, WallSegment connectedWallSegment, float angle)
        {
            this.ownerWallAnchor = ownerAnchor;
            this.connectedWallAnchor = connectedAnchor;
            this.connectedWallSegment = connectedWallSegment;
            this.angle = angle;
        }
    }
}