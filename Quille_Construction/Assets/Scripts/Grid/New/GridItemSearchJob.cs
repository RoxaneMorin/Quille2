using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace ProceduralGrid
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct GridItemSearchJob : IJob
    {
        // PARAMETERS
        [ReadOnly] public float3 CursorPosition;

        [ReadOnly] public NativeArray<GridItem> GridItems;
        [ReadOnly] public int2 GridResolution;
        [ReadOnly] public float TileSize;

        // NativeArray of size 3. [0] is the closest item's index. [1] is the number of items evaluated. [2] is the "depth" of the search.
        public NativeArray<int> ClosestItemsIndexAndOutput; 


        // SEARCH VARIABLES
        float halfTileSize;

        int currentResolutionX;
        int currentResolutionZ;
        [DeallocateOnJobCompletion] NativeArray<int> currentCornerIndices;

        float closestItemsDistance;
        int currentItemsIndex;
        float currentItemsDistance;


        // PSEUDO PROPERTIES
        [BurstCompile] public int GetIndexForCoords(int2 coords)
        {
            return coords.x + (coords.y * GridResolution.x);
        }
        [BurstCompile] public int GetIndexForCoords(int x, int z)
        {
            return x + (z * GridResolution.x);
        }



        // METHODS
        void SetUp()
        {
            // Initial dimensions, etc.
            halfTileSize = TileSize / 2 + 0.05f;
            currentResolutionX = GridResolution.x;
            currentResolutionZ = GridResolution.y;
            currentCornerIndices = new NativeArray<int>(4, Allocator.TempJob);

            // bottomLeft, bottomRight, topLeft, topRight
            currentCornerIndices[0] = GetIndexForCoords(0, 0);
            currentCornerIndices[1] = GetIndexForCoords(currentResolutionX - 1, 0);
            currentCornerIndices[2] = GetIndexForCoords(0, currentResolutionZ - 1);
            currentCornerIndices[3] = GetIndexForCoords(currentResolutionX - 1, currentResolutionZ - 1);

            // Holder variables.
            ClosestItemsIndexAndOutput[0] = -1;
            closestItemsDistance = float.PositiveInfinity;

            // Tracking.
            ClosestItemsIndexAndOutput[1] = 0;
            ClosestItemsIndexAndOutput[2] = 0;
        }

        [BurstCompile]
        void ReturnClosestIndexFrom(int previousClosestIndex)
        {
            ClosestItemsIndexAndOutput[2]++;
            for (int i = 0; i < 4; i++)
            {
                currentItemsIndex = currentCornerIndices[i];

                if (currentItemsIndex != previousClosestIndex)
                {
                    ClosestItemsIndexAndOutput[1]++;

                    // Calculate the distance between this corner and the cursor.
                    currentItemsDistance = distance(CursorPosition, GridItems[currentItemsIndex].WorldPosition);
                    // Debug.Log(string.Format("Item {0}'s distance from the cursor is {1}.", currentItemsIndex, currentItemsDistance));

                    if (currentItemsDistance < closestItemsDistance)
                    {
                        closestItemsDistance = currentItemsDistance;
                        ClosestItemsIndexAndOutput[0] = currentItemsIndex;
                    }
                }
            }
        }

        // BUILT IN
        [BurstCompile]
        public void Execute()
        {
            //Debug.Log("Initial X Resolution: " + currentResolutionX);
            //Debug.Log("Initial Z Resolution: " + currentResolutionZ);

            while (currentResolutionX > 0 || currentResolutionZ > 0)
            {
                //Debug.Log(string.Format("Corner points of the current search area: {0}, {1}, {2}, {3}", currentCornerIndices[0], currentCornerIndices[1], currentCornerIndices[2], currentCornerIndices[3]));

                ReturnClosestIndexFrom(ClosestItemsIndexAndOutput[0]);
                if (closestItemsDistance < halfTileSize)
                {
                    //Debug.Log(string.Format("Final closest point: {0}, distance of {1}.", ClosestItemsIndexAndOutput[0], closestItemsDistance));
                    return;
                }


                //TODO: optimize for even resolutions.
                // The stuff I've tried so far (subtracting one in certain conditions) always ended up fucking up later along the line.
                // Do it when picking the next corner points instead?
                currentResolutionX = max(currentResolutionX / 2, 0);
                currentResolutionZ = max(currentResolutionZ / 2, 0);

                //Debug.Log("Current X Resolution: " + currentResolutionX);
                //Debug.Log("Current Z Resolution: " + currentResolutionZ);
                

                // Update the currentCornerIndices based on the closest item's position.
                int2 closestItemsGridCoordinates = GridItems[ClosestItemsIndexAndOutput[0]].GridCoordinates;

                // Botton left.
                if (ClosestItemsIndexAndOutput[0] == currentCornerIndices[0])
                {
                    // currentCornerIndices[0] = currentCornerIndices[0];
                    currentCornerIndices[1] = GetIndexForCoords(closestItemsGridCoordinates + int2(currentResolutionX, 0));
                    currentCornerIndices[2] = GetIndexForCoords(closestItemsGridCoordinates + int2(0, currentResolutionZ));
                    currentCornerIndices[3] = GetIndexForCoords(closestItemsGridCoordinates + int2(currentResolutionX, currentResolutionZ));
                }
                // Botton right.
                else if (ClosestItemsIndexAndOutput[0] == currentCornerIndices[1])
                {
                    currentCornerIndices[0] = GetIndexForCoords(closestItemsGridCoordinates + int2(-currentResolutionX, 0));
                    // currentCornerIndices[1] = currentCornerIndices[1];
                    currentCornerIndices[2] = GetIndexForCoords(closestItemsGridCoordinates + int2(-currentResolutionX, currentResolutionZ));
                    currentCornerIndices[3] = GetIndexForCoords(closestItemsGridCoordinates + int2(0, currentResolutionZ));
                }
                // Top left.
                else if (ClosestItemsIndexAndOutput[0] == currentCornerIndices[2])
                {
                    currentCornerIndices[0] = GetIndexForCoords(closestItemsGridCoordinates + int2(0, -currentResolutionZ));
                    currentCornerIndices[1] = GetIndexForCoords(closestItemsGridCoordinates + int2(currentResolutionX, -currentResolutionZ));
                    // currentCornerIndices[2] = currentCornerIndices[2];
                    currentCornerIndices[3] = GetIndexForCoords(closestItemsGridCoordinates + int2(currentResolutionX, 0));
                }
                // Top right.
                else if (ClosestItemsIndexAndOutput[0] == currentCornerIndices[3])
                {
                    // WONKY
                    currentCornerIndices[0] = GetIndexForCoords(closestItemsGridCoordinates + int2(-currentResolutionX, -currentResolutionZ));
                    currentCornerIndices[1] = GetIndexForCoords(closestItemsGridCoordinates + int2(0, -currentResolutionZ));
                    currentCornerIndices[2] = GetIndexForCoords(closestItemsGridCoordinates + int2(-currentResolutionX, 0));
                    // currentCornerIndices[3] = currentCornerIndices[3];
                }
            }
        }

        public static JobHandle Schedule(float3 cursorPosition, NativeArray<GridItem> gridItems, int2 gridResolution, float tileSize, NativeArray<int> outputIndex, JobHandle dependency)
        {
            var job = new GridItemSearchJob()
            {
                CursorPosition = cursorPosition,
                GridItems = gridItems,
                GridResolution = gridResolution,
                TileSize = tileSize,
                ClosestItemsIndexAndOutput = outputIndex
            };
            job.SetUp();

            return job.Schedule(dependency);
        }
    }
}


