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
                //FindCirclesRoot(selectedAnchor);
                FindCordlessCycles();
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

        // TODO: version for checking everything in one go, which will require rotating around the minID node instead.

        // TODO: rotate path to smallest ID

        static List<WallAnchor> RotatePathToRootNode(List<WallAnchor> path, WallAnchor rootNode)
        {
            if (path.Count == 0)
            {
                return path;
            }

            int rootIndex = path.IndexOf(rootNode);
            return path.Skip(rootIndex).Concat(path.Take(rootIndex)).ToList();
        }

        static List<WallAnchor> InvertPath(List<WallAnchor> path, WallAnchor rootNode)
        {
            if (path.Count == 0)
            {
                return path;
            }

            return RotatePathToRootNode(Enumerable.Reverse(path).ToList(), rootNode);
        }

        static bool IsPathNew(List<List<WallAnchor>> cycles, List<WallAnchor> path)
        {
            return !cycles.Any(cycle => cycle.SequenceEqual(path));
        }

        static bool HasNodeBeenVisited(WallAnchor node, List<WallAnchor> path)
        {
            return path.Contains(node);
        }


        private void FindCyclesRecursive(List<List<WallAnchor>> cycles, List<WallAnchor> path, WallAnchor currentNode)
        {
            List<WallAnchor> updatedPath = path.Concat(new[] { currentNode }).ToList();
            //Debug.Log(string.Format("The path so far: {0}", string.Join(", ", updatedPath.Select(node => node.ID))));

            foreach (WallAnchor.WallConnection connection in currentNode.Connections)
            {
                if (!HasNodeBeenVisited(connection.ConnectedAnchor, path))
                {
                    FindCyclesRecursive(cycles, updatedPath, connection.ConnectedAnchor);
                }
                else if (updatedPath.Count > 2 && connection.ConnectedAnchor == updatedPath[0])
                {
                    // We found a cycle!

                    // TODO: Rotate path to the smallest ID too if we search all nodes instead of node by node.
                    List<WallAnchor> invertedPath = InvertPath(updatedPath, connection.ConnectedAnchor);

                    if (IsPathNew(cycles, updatedPath) && IsPathNew(cycles, invertedPath))
                    {
                        cycles.Add(updatedPath);
                    }
                }
            }
        }


        private void FindCirclesRoot(WallAnchor rootNode)
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




        // http://richard.baltensp.home.hefr.ch/Publications/20.pdf
        // 1. Adjacency matrix

        // 2. Add a first vertex Vstart to an empty path. (Do until all vertices are handled)

        private void FindCordlessCycles()
        {
            List<List<WallAnchor>> cycles = new List<List<WallAnchor>>();
            List<WallAnchor> path = new List<WallAnchor>();

            foreach (WallAnchor node in areaWallAnchors)
            {
                FindCordlessCyclesRecursive(cycles, path, node);
            }

            if (cycles.Count > 0)
            {
                List<List<WallAnchor>> prunedCycles = PruneCordedCycles(cycles);

                string infoString = string.Format("Cycles found:");
                foreach (List<WallAnchor> pathFound in cycles)
                {
                    infoString += string.Format("\n{0}", string.Join(", ", pathFound.Select(node => node.ID)));
                }

                Debug.Log(infoString);
            }
        }

        // 3. Select first adjacent vertex Vj of the last vertex Vend in path (Vstart the first time around).
        // It must not exist in P already, and be bigger than Vstart.
        // If no Vj exists, delete the last vertex of path and redo this step.
        // When all the adjacent vertices Vj of Vs have been handled, go to step two and do it with Vstart+1

        // 4. If path.Count = 2, go to step 3

        // 5. If path.Count = 3, check if Vend and Vstart are connected.
        // If they are not, go to step 3 in search of expansion. 
        // If they are connected, we have a cycle. Output. Delete the last vertex in path and go back to 3.

        // 6. If path.Count > 3, check if any two non-adjacent vertices are connected ignoring Vstart.
        // If any, delete the last vertex in path and go back to 3.
        // Else, see if Vend and Vstart are connected.
        // If we have a cycle, output. Output. Delete the last vertex in path and go back to 3.
        // Else, just continue from step 3.

        // 7. Do for the whole list.

        private void FindCordlessCyclesRecursive(List<List<WallAnchor>> cycles, List<WallAnchor> path, WallAnchor currentNode)
        {
            List<WallAnchor> updatedPath = path.Concat(new[] { currentNode }).ToList();

            foreach (WallAnchor.WallConnection connection in currentNode.Connections)
            {
                WallAnchor adjacentNode = connection.ConnectedAnchor;


                Debug.Log(adjacentNode.Connections.Any(nextNode => path.Skip(1).Take(path.Count - 2).Contains(nextNode.ConnectedAnchor)));


                if (adjacentNode.ID > updatedPath[0].ID && !updatedPath.Contains(adjacentNode))
                {
                    FindCordlessCyclesRecursive(cycles, updatedPath, adjacentNode);
                }
                else if (updatedPath.Count == 3  && adjacentNode == updatedPath[0])
                {
                    cycles.Add(updatedPath);
                }
                else if (updatedPath.Count > 3)
                {
                    // are we connecting to something that's already in path?
                    // yes -> continue

                    // else, are we connecting to the final node?
                    


                    //  && !nextNode.Connections.Any(adjacent => path.Skip(1).Take(path.Count - 2).Contains(adjacent.ConnectedAnchor))

                    Debug.Log("Long path");
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
