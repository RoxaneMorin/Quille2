using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using JetBrains.Annotations;
using AmplifyShaderEditor;
using System.IO;
using static UnityEditor.ShaderData;
using Unity.Burst.Intrinsics;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEditor.MemoryProfiler;

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
        // TODO: add list with all connections

        [SerializeField] private WallAnchor selectedAnchor;


        [Header("Resources")]
        [SerializeField] protected GameObject wallAnchorPrefab;
        [SerializeField] protected GameObject wallSegmentPrefab;


        // EVENTS
        public event WallAnchorSelected OnWallAnchorSelected;



        // TODO:
        // Rework anchorID, add segmentID to the actual segments.

        // Since walls now exist "physically", simply check for intersections via raycast. 
        // Option to split a wall segment by clicking on it?


        // Room detection!


        // https://cs.stackexchange.com/questions/118306/how-to-get-enclosed-spaces-from-a-series-of-connected-nodes


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

            //// Test finding new cycles
            //if (selectedAnchor != null)
            //{
            //    //FindCycles(selectedAnchor);
            //    //FindCordlessCycles();
            //}
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

            Debug.Log(string.Format("Created a new wall anchor: {0}", newAnchor));

            if (doSegment && selectedAnchor != null)
            {
                CreateWallSegment(selectedAnchor, newAnchor);
            }

            return newAnchor;
        }

        // TODO: ensure we aren't recreating a wall that already exists.

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

            newSegment.OnParameterUpdate();
        }

        private void SplitWallSegment(WallSegment targetSegment, WallAnchor centralAnchor)
        {
            WallSegment newSegmentA = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegmentA.Init(targetSegment.AnchorA, centralAnchor, targetSegment.Thickness);
           
            WallSegment newSegmentB = Instantiate(wallSegmentPrefab, transform.position, Quaternion.identity).GetComponent<WallSegment>();
            newSegmentB.Init(centralAnchor, targetSegment.AnchorB, targetSegment.Thickness);

            targetSegment.AnchorA.ReplaceConnection(targetSegment, centralAnchor, newSegmentA);
            targetSegment.AnchorB.ReplaceConnection(targetSegment, centralAnchor, newSegmentB);

            centralAnchor.AddConnection(newSegmentA);
            centralAnchor.AddConnection(newSegmentB);

            areaWallSegments.Add(newSegmentA);
            areaWallSegments.Add(newSegmentB);

            areaWallSegments.Remove(targetSegment);
            Destroy(targetSegment.gameObject);

            newSegmentA.OnParameterUpdate();
            newSegmentB.OnParameterUpdate();
        }


        // TODO: Clean up. Do raycast at middle and top also.
        private List<(WallSegment, Vector3, float)> ListIntersectingWallSegments(WallAnchor anchorA, WallAnchor anchorB)
        {
            List<(WallSegment, Vector3, float)> intersectingSegments = new List<(WallSegment, Vector3, float)>();

            Vector3 posA = anchorA.transform.position;
            Vector3 dirAtoB = anchorB.transform.position - anchorA.transform.position;
            float distAtoB = dirAtoB.magnitude;
            
            RaycastHit[] wallHits;
            wallHits = Physics.RaycastAll(posA, dirAtoB, distAtoB, (1 << 13));

            foreach(RaycastHit hit in wallHits)
            {
                Vector3 intersectionPoint = hit.point;

                Collider hitCollider = hit.collider;
                GameObject hitGameObject = hitCollider.gameObject;
                WallSegment hitWallSegment = hitGameObject.GetComponent<WallSegment>();

                if ( hitWallSegment != null )
                {
                    float distanceFromAnchorA = Vector3.Distance(posA, intersectionPoint);
                    intersectingSegments.SortedInsert((hitWallSegment, intersectionPoint, distanceFromAnchorA), (existingIntersection, newIntersection) => existingIntersection.Item3 > newIntersection.Item3);
                }
            }
            //    Vector3? intersectionPoint = MathHelpers.CalculatePotentialIntersectionPointXZ(anchorA.transform.position, anchorB.transform.position, otherSegment.AnchorA.transform.position, otherSegment.AnchorB.transform.position);

            return intersectingSegments;
        }




        // Find rooms/cycles

        
        static bool IsPathNew(List<List<WallAnchor>> cycles, List<WallAnchor> path)
        {
            return !cycles.Any(cycle => cycle.SequenceEqual(path));
        }

        static List<WallAnchor> RotatePathToRootNode(List<WallAnchor> path, WallAnchor rootNode)
        {
            if (path.Count == 0)
            {
                return path;
            }

            int rootIndex = path.IndexOf(rootNode);
            return path.Skip(rootIndex).Concat(path.Take(rootIndex)).ToList();
        }
        static List<WallAnchor> InvertPathAroundRootNode(List<WallAnchor> path, WallAnchor rootNode)
        {
            if (path.Count == 0)
            {
                return path;
            }

            return RotatePathToRootNode(Enumerable.Reverse(path).ToList(), rootNode);
        }

        static List<WallAnchor> RotatePathToLowestID(List<WallAnchor> path)
        {
            if (path.Count == 0)
            {
                return path;
            }

            int lowestIDsIndex = path.IndexOf(path.Min());
            return path.Skip(lowestIDsIndex).Concat(path.Take(lowestIDsIndex)).ToList();
        }

        static List<WallAnchor> InvertPathAroundLowestID(List<WallAnchor> path)
        {
            if (path.Count == 0)
            {
                return path;
            }

            return RotatePathToLowestID(Enumerable.Reverse(path).ToList());
        }

        private List<List<WallAnchor>> PruneCordedCycles(List<List<WallAnchor>> cycles)
        {
            List<List<WallAnchor>> prunedCycles = new List<List<WallAnchor>>();

            foreach (List<WallAnchor> cycle in cycles)
            {
                if (!IsCycleCorded(cycle))
                {
                    prunedCycles.Add(cycle);
                }
            }

            return prunedCycles;
        }

        private bool IsCycleCorded(List<WallAnchor> cycle)
        {
            foreach (WallAnchor node in cycle)
            {
                if (node.Connections.Count(connection => cycle.Contains(connection.ConnectedAnchor)) > 2)
                {
                    return true;
                }
            }

            return false;
        }


        // TODO: verify/test which is more efficient.


        // https://www.geeksforgeeks.org/print-all-the-cycles-in-an-undirected-graph/
        private void FindCycles(WallAnchor rootNode)
        {
            List<List<WallAnchor>> cycles = new List<List<WallAnchor>>();
            List<WallAnchor> path = new List<WallAnchor>();

            FindCyclesRecursive(cycles, path, rootNode);

            if (cycles.Count > 0)
            {
                List<List<WallAnchor>> prunedCycles = PruneCordedCycles(cycles);

                string infoString = string.Format("Cycles found for rootNode {0}:", rootNode.ID);
                foreach (List<WallAnchor> pathFound in cycles)
                {
                    infoString += string.Format("\n{0}", string.Join(", ", pathFound.Select(node => node.ID)));
                }

                infoString += string.Format("\nCordless cycles found for rootNode {0}:", rootNode.ID);
                foreach (List<WallAnchor> pathFound in prunedCycles)
                {
                    infoString += string.Format("\n{0}", string.Join(", ", pathFound.Select(node => node.ID)));
                }

                Debug.Log(infoString);
            }
        }
        private void FindCyclesRecursive(List<List<WallAnchor>> cycles, List<WallAnchor> path, WallAnchor currentNode)
        {
            List<WallAnchor> updatedPath = path.Concat(new[] { currentNode }).ToList();
            //Debug.Log(string.Format("The path so far: {0}", string.Join(", ", updatedPath.Select(node => node.ID))));

            foreach (WallConnection connection in currentNode.Connections)
            {
                if (!path.Contains(connection.ConnectedAnchor))
                {
                    FindCyclesRecursive(cycles, updatedPath, connection.ConnectedAnchor);
                }
                else if (updatedPath.Count > 2 && connection.ConnectedAnchor == updatedPath[0])
                {
                    // We found a cycle!

                    // TODO: Rotate path to the smallest ID too if we search all nodes instead of node by node.
                    List<WallAnchor> invertedPath = InvertPathAroundRootNode(updatedPath, connection.ConnectedAnchor);

                    if (IsPathNew(cycles, updatedPath) && IsPathNew(cycles, invertedPath))
                    {
                        cycles.Add(updatedPath);
                    }
                }
            }
        }


        // Inspired by the algorithm described in the paper "Identification of chordless cycles in ecological networks"
        // http://richard.baltensp.home.hefr.ch/Publications/20.pdf
        private void FindCordlessCycles()
        {
            List<List<WallAnchor>> cycles = new List<List<WallAnchor>>();

            foreach (WallAnchor node in areaWallAnchors)
            {
                // Create a new path containing only the starter node.
                FindCordlessCyclesRecursive(cycles, new List<WallAnchor>() { node });
            }

            if (cycles.Count > 0)
            {
                string infoString = string.Format("Cycles found:");
                foreach (List<WallAnchor> pathFound in cycles)
                {
                    infoString += string.Format("\n{0}", string.Join(", ", pathFound.Select(node => node.ID)));
                }

                Debug.Log(infoString);
            }
        }

        private void FindCordlessCyclesRecursive(List<List<WallAnchor>> cycles, List<WallAnchor> previousPath)
        {
            // Vend: the path's current last node.
            WallAnchor vEnd = previousPath.Last();
            foreach (WallConnection adjacentConnection in vEnd.Connections)
            {
                WallAnchor vJ = adjacentConnection.ConnectedAnchor;

                // Select the first adjacent node Vj of the path's last node. Vj should be bigger than vStart, and absent from the current path.
                if (vJ.ID > previousPath[0].ID && !previousPath.Contains(vJ))
                {
                    // If vJ is valid, add it to the path.
                    List<WallAnchor> path = previousPath.Concat(new[] { vJ }).ToList();

                    // If path.Count = 2, recursive call.

                    // If path.Count = 3, check if Vend (which is now Vj) and Vstart are connected.
                    if (path.Count == 3 && vJ.IsConnectedTo(path[0]))
                    {
                        // If they are, output the cycle and continue.
                        List<WallAnchor> rotatedPath = RotatePathToLowestID(path);
                        List<WallAnchor> invertedPath = InvertPathAroundLowestID(path);

                        if (IsPathNew(cycles, rotatedPath) && IsPathNew(cycles, invertedPath))
                        {
                            cycles.Add(rotatedPath);
                        }
                        continue;
                    }
                    // If they are not, recursive call.

                    // If path.Count > 3, 
                    else if (path.Count > 3)
                    {
                        // Check if any non-adjacent vertices are connected (ignoring vStart).
                        if (previousPath.Skip(1).Take(previousPath.Count - 2).Any(node => vJ.IsConnectedTo(node)))
                        {
                            // TODO: try an extra level of depth here?

                            // If any, continue.
                            continue;
                        }

                        // Else, check if Vend and Vstart are connected.
                        if (vJ.IsConnectedTo(path[0]))
                        {
                            // If they are, output the cycle and continue.
                            List<WallAnchor> rotatedPath = RotatePathToLowestID(path);
                            List<WallAnchor> invertedPath = InvertPathAroundLowestID(path);

                            if (IsPathNew(cycles, rotatedPath) && IsPathNew(cycles, invertedPath))
                            {
                                cycles.Add(rotatedPath);
                            }
                            continue;
                        }
                        // If they are not, recursive call.
                    }

                    FindCordlessCyclesRecursive(cycles, path);
                }
            }
        }

        // TODO: clean up function to remove cycles cut through by indirect connections.
        // Aparently these aren't considered cords.




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


//#if DEBUG
//        private void OnDrawGizmos()
//        {
//            foreach (WallSegment segment in areaWallSegments)
//            {
//                if (segment.AnchorA && segment.AnchorB)
//                {
//                    // Draw a line representing this wall.
//                    Gizmos.color = Color.white;
//                    Gizmos.DrawLine(segment.AnchorA.transform.position, segment.AnchorB.transform.position);


//                    // Draw captions for anchors A and B.
//                    Vector3 dirAtoB = (segment.AnchorB.transform.position - segment.AnchorA.transform.position).normalized;
//                    Vector3 dirBtoA = (segment.AnchorA.transform.position - segment.AnchorB.transform.position).normalized;

//                    Vector3 shiftedPosA = segment.AnchorA.transform.position + dirAtoB * 0.3f;
//                    Vector3 shiftedPosB = segment.AnchorB.transform.position + dirBtoA * 0.3f;

//                    Handles.Label(shiftedPosA, "A");
//                    Handles.Label(shiftedPosB, "B");
//                }
//            }
//        }
//#endif
    }

}
