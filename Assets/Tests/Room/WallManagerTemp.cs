using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

namespace Building
{
    public class WallManagerTemp : MonoBehaviour
    {
        // VARIABLES/PARAMETERS
        [Header("Data and References")]
        [SerializeField] private int highestAnchorID = -1;
        [SerializeField] private int highestSegmentID = -1;

        [SerializeField] private List<WallAnchor> areaWallAnchors;
        [SerializeField] private List<WallSegment> areaWallSegments;

        [SerializeField] private WallAnchor selectedAnchor;


        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;
        [SerializeField] protected GameObject wallSegmentPrefab;


        // EVENTS
        public event WallAnchorSelected OnWallAnchorSelected;



        // TODO:
        // Rework anchorID, add segmentID to the actual segments.
        // Option to split a wall segment by clicking on it? Will wall segments have physical objects anyways?
        // Room detection!


        // METHODS

        // INIT
        public void Init()
        {
            areaWallAnchors = new List<WallAnchor>();
            areaWallSegments = new List<WallSegment>();
        }


         // EVENT LISTENERS
         private void OnWallAnchorClicked(WallAnchor targetAnchor, PointerEventData.InputButton clickType)
        {
            if (clickType == PointerEventData.InputButton.Left)
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
            else if (clickType == PointerEventData.InputButton.Right)
            {
                if (targetAnchor != selectedAnchor && selectedAnchor != null && !targetAnchor.IsConnectedTo(selectedAnchor))
                {
                    CreateWallSegment(targetAnchor, selectedAnchor);
                }
            }
        }


        // UTILITY
        private void SelectWallAnchor(WallAnchor targetAnchor)
        {
            OnWallAnchorSelected?.Invoke(targetAnchor);
            selectedAnchor = targetAnchor;

            // Test finding new cycles
            if (selectedAnchor != null)
            {
                FindCirclesRoot(selectedAnchor);
            }
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

            // TODO: can we toggle whether the wall gets selected after its creation?
            // Atm, it "gets clicked" which throws the event.

            //Debug.Log(String.Format("Created a new wall anchor: {0}", newAnchor));

            if (doSegment && selectedAnchor != null)
            {
                CreateWallSegment(selectedAnchor, newAnchor);
            }

            return newAnchor;
        }

        private void CreateWallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            // Collect potential intersections with other segments.
            List<(WallSegment, Vector3, float)> intersectingSegments = ListIntersectingWallSegments(anchorA, anchorB);

            if (intersectingSegments.Count > 0)
            {
                // If there are any, split this new segment in consequence.
                foreach ((WallSegment, Vector3, float) intersection in intersectingSegments)
                {
                    WallAnchor newAnchor = CreateWallAnchor(intersection.Item2, false);
                    CreateSingleWallSegment(anchorA, newAnchor);

                    SplitWallSegment(intersection.Item1, newAnchor);

                    anchorA = newAnchor;
                }
            }
            // Else, just create the one segment.

            CreateSingleWallSegment(anchorA, anchorB);
        }

        private void CreateSingleWallSegment(WallAnchor anchorA, WallAnchor anchorB)
        {
            WallSegment newSegment = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegment.Init(anchorA, anchorB);

            anchorA.AddConnection(newSegment);
            anchorB.AddConnection(newSegment);

            areaWallSegments.Add(newSegment);
        }

        private void SplitWallSegment(WallSegment targetSegment, WallAnchor centralAnchor)
        {
            WallSegment newSegmentA = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegmentA.Init(targetSegment.AnchorA, centralAnchor);
           
            WallSegment newSegmentB = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegmentB.Init(centralAnchor, targetSegment.AnchorB);

            targetSegment.AnchorA.ReplaceConnection(targetSegment, centralAnchor, newSegmentA);
            targetSegment.AnchorB.ReplaceConnection(targetSegment, centralAnchor, newSegmentB);

            centralAnchor.AddConnection(newSegmentA);
            centralAnchor.AddConnection(newSegmentB);

            areaWallSegments.Add(newSegmentA);
            areaWallSegments.Add(newSegmentB);

            areaWallSegments.Remove(targetSegment);
            Destroy(targetSegment.gameObject);
        }

        private List<(WallSegment, Vector3, float)> ListIntersectingWallSegments(WallAnchor anchorA, WallAnchor anchorB)
        {
            List<(WallSegment, Vector3, float)> intersectingSegments = new List<(WallSegment, Vector3, float)>();

            foreach (WallSegment otherSegment in areaWallSegments)
            {
                // TODO: Further skip segments we know won't intersect.

                // Skip segments connected to the same anchors.
                if (anchorA == otherSegment.AnchorA || anchorA == otherSegment.AnchorB || anchorB == otherSegment.AnchorA || anchorB == otherSegment.AnchorB)
                {
                    continue;
                }

                Vector3? intersectionPoint = Building_MathHelpers.CalculatePotentialIntersectionPointXZ(anchorA.transform.position, anchorB.transform.position, otherSegment.AnchorA.transform.position, otherSegment.AnchorB.transform.position);
                if (intersectionPoint.HasValue)
                {
                    float distanceFromAnchorA = Vector3.Distance(anchorA.transform.position, intersectionPoint.Value);
                    intersectingSegments.SortedInsert((otherSegment, intersectionPoint.Value, distanceFromAnchorA), (existingIntersection, newIntersection) => existingIntersection.Item3 > newIntersection.Item3);
                }
            }

            return intersectingSegments;
        }




        // Find rooms/cycles

        // https://www.geeksforgeeks.org/print-all-the-cycles-in-an-undirected-graph/

        // Cycles are a list of lists of nodes

        // For each edge,
        // For each node in the edge:

        // FindNewCycles (List<node> path, graph information, cycles):
        //*
        // If path.count == zero, return

        // startNode = path[0]

        // for each edge in the graph,
        // node1 = edge[0], node2 = edge[1]

        // if the startNode is either node1 or node2,
        // nextNode = the other node in that edge

        // if this nextNode has not yet been visited,
        // sub = a new list containing nextNode
        // add the current path to sub
        // FindNewCycles(sub, graph, cycles)

        // Else, if path.Count > 2 and the nextNode == path.Last(),
        // p = RotateToSmallest(new list from path)
        // inv = Invert(new list from p)

        // if IsNew(p, cycles) and IsNew(inv, cycles)
        // add p to cycles
        //*



        //void FindNewCycles(List<WallAnchor> path, List<List<WallAnchor>> cycles)
        //{
        //    if (path.Count == 0)
        //    {
        //        return;
        //    }

        //    WallAnchor startNode = path[0];



        //    // for each edge in the graph,
        //    // node1 = edge[0], node2 = edge[1]

        //    // if the startNode is either node1 or node2,
        //    // nextNode = the other node in that edge

        //    // if this nextNode has not yet been visited,
        //    // sub = a new list containing nextNode
        //    // add the current path to sub
        //    // FindNewCycles(sub, graph, cycles)

        //    // Else, if path.Count > 2 and the nextNode == path.Last(),
        //    // p = RotateToSmallest(new list from path)
        //    // inv = Invert(new list from p)

        //    // if IsNew(p, cycles) and IsNew(inv, cycles)
        //    // add p to cycles

        //}




        // Visited: does the given path contain the given node?

        // RotateToSmallest(path): change the path's first node to start with the lowest index

        // Invert(path): invert the list, and do RotateToSmallest

        // IsNew: !cycles.Any(c => c.SequenceEqual(path))


        static List<WallAnchor> RotatePathToSmallest(List<WallAnchor> path)
        {
            if (path.Count == 0)
            {
                return path;
            }

            int minIndex = path.IndexOf(path.Min());
            return path.Skip(minIndex).Concat(path.Take(minIndex)).ToList();
        }

        static List<WallAnchor> InvertPath(List<WallAnchor> path)
        {
            if (path.Count == 0)
            {
                return path;
            }

            path.Reverse();
            return RotatePathToSmallest(path);
        }

        static bool IsPathNew(List<List<WallAnchor>> cycles, List<WallAnchor> path)
        {
            return !cycles.Any(cycle => cycle.SequenceEqual(path));
        }

        static bool HasNodeBeenVisited(WallAnchor node, List<WallAnchor> path)
        {
            return path.Contains(node);
        }



        /// Trying to rewrite it to use my data structure

        //currentPath

        //initialNode of this path
        //currentNode we are looking at
        //previousNode we came from (track as visited instead?)

        //for each connection of the currentNode (but the one we came from),

        // if its otherNode is the initialNode,
        //   We found a cycle!
        //   rotatedPath = path starting from the node with the lowest id
        //  invertedpath = path inverted, starting from the node with the lowest id
        //  if both of these aren't already present in cycles,
        //            add rotatedPath to cycles

        // else,
        //        add otherNode to the currentPath,
        //        call this function recursively using a copy of currentPath


        private void FindCyclesRecursive(List<List<WallAnchor>> cycles, List<WallAnchor> path, WallAnchor currentNode, WallAnchor previousNode)
        {
            Debug.Log(string.Format("Current Node: {0}\nPrevious Node: {1}", currentNode, previousNode));


            foreach (WallAnchor.WallConnection connection in currentNode.Connections)
            {
                if (HasNodeBeenVisited(connection.ConnectedAnchor, path))
                {
                    Debug.Log(string.Format("The node {0} has supposedly already been visited", connection.ConnectedAnchor));
                    continue;
                }

                else if (connection.ConnectedAnchor == path[0])
                {
                    // We found a cycle!
                    List<WallAnchor> rotatedPath = RotatePathToSmallest(path);
                    List<WallAnchor> invertedPath = InvertPath(path);

                    if (IsPathNew(cycles, rotatedPath) && IsPathNew(cycles, invertedPath))
                    {
                        cycles.Add(rotatedPath);
                    }
                }

                else
                {
                    List<WallAnchor> updatedPath = new List<WallAnchor>(path);
                    updatedPath.Add(connection.ConnectedAnchor);

                    // Something seems to be going wrong with the list when an anchor has more than one connection?
                    FindCyclesRecursive(cycles, updatedPath, connection.ConnectedAnchor, currentNode);
                }
            }
        }


        private void FindCirclesRoot(WallAnchor rootNode)
        {
            List<List<WallAnchor>> cycles = new List<List<WallAnchor>>();

            List<WallAnchor> rootPath = new List<WallAnchor>();
            rootPath.Add(rootNode);

            FindCyclesRecursive(cycles, rootPath, rootNode, null);

            if (cycles.Count > 0)
            {
                string infoString = string.Format("Cycles found for rootNode {0}:", rootNode.ID, cycles.Count);

                foreach (List<WallAnchor> pathFound in cycles)
                {
                    infoString += "\n";
                    foreach (WallAnchor node in pathFound)
                    {
                        infoString += node.ID + " ";
                    }

                    Debug.Log(infoString);
                }
            }
            
        }

        





        // BUILT IN
        private void Start()
        {
            Init();
        }


        private void OnMouseDown()
        {
            // TODO: use OnPointerDown instead?

            RaycastHit cursorHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out cursorHit))
            {
                CreateWallAnchor(cursorHit.point);
            }
        }


#if DEBUG
        private void OnDrawGizmos()
        {
            foreach (WallSegment segment in areaWallSegments)
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
