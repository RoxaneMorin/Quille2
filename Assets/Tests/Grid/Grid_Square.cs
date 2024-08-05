using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_Square : Grid_Item
    {
        // VARIABLES

        // Grid information
        [SerializeField] private CoordPair[,] myGridPointsCoordinates; // Is this necessary?
        [SerializeField] private Grid_Point[,] myGridPoints;
        [SerializeField] private EightAdjacentSquares myAdjacentGridSquares;
        public Grid_Point[,] MyGridPoints { get { return myGridPoints; } }
        public EightAdjacentSquares MyAdjacentGridSquares { get { return myAdjacentGridSquares; } }



        // METHODS

        // INIT
        public void SetVariables(Grid_Base myParentGrid, CoordPair myGridCoordinates, Grid_Point[,] myGridPoints)
        {
            this.myParentGrid = myParentGrid;
            this.myGridCoordinates = myGridCoordinates;
            this.myGridPoints = myGridPoints;
            myGridPointsCoordinates = new CoordPair[2, 2] { { new CoordPair(myGridCoordinates.x, myGridCoordinates.z), new CoordPair(myGridCoordinates.x + 1, myGridCoordinates.z) },
                                                                  { new CoordPair(myGridCoordinates.x, myGridCoordinates.z + 1), new CoordPair(myGridCoordinates.x + 1, myGridCoordinates.z + 1) }};
        }

        public void SetAdjacentGridSquare(Adjacencies adjacency, Grid_Square newGridSquare)
        {
            switch (adjacency)
            {
                case Adjacencies.bl:
                    myAdjacentGridSquares.bl = newGridSquare;
                    break;
                case Adjacencies.b:
                    myAdjacentGridSquares.b = newGridSquare;
                    break;
                case Adjacencies.br:
                    myAdjacentGridSquares.br = newGridSquare;
                    break;
                case Adjacencies.tl:
                    myAdjacentGridSquares.tl = newGridSquare;
                    break;
                case Adjacencies.t:
                    myAdjacentGridSquares.t = newGridSquare;
                    break;
                case Adjacencies.tr:
                    myAdjacentGridSquares.tr = newGridSquare;
                    break;
                case Adjacencies.l:
                    myAdjacentGridSquares.l = newGridSquare;
                    break;
                case Adjacencies.r:
                    myAdjacentGridSquares.r = newGridSquare;
                    break;
            }
        }


        // UTILITY

        // MOVEMENT
        override public void MoveMeAndRelatives(Vector3 mouseMouvementDelta)
        {
            float upwardMotion = mouseMouvementDelta.y * Time.deltaTime;
            Vector3 upwardMotion3D = new Vector3(0f, upwardMotion, 0f);

            MoveMe(upwardMotion3D);

            myParentGrid.MoveSquare(myGridPointsCoordinates, upwardMotion3D);
            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        myGridPoints[i, j].MoveMe(upwardMotion3D);
                    }
                }
            }
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square != null)
                {
                    square.AverageMyHeightFromMyGridPoints();
                }
            }
        }
        public void AverageMyHeightFromMyGridPoints()
        {
            int noOfGridPoints = 0;
            float heightAccumulator = 0;

            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        noOfGridPoints++;
                        heightAccumulator += myGridPoints[i, j].transform.position.y;
                    }
                }
            }
            heightAccumulator /= noOfGridPoints;

            transform.position = new Vector3(transform.position.x, heightAccumulator, transform.position.z);
        }

        // CHILDREN
        override protected void HoverChildren() 
        {
            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        myGridPoints[i, j].MouseHoveringOnRelative = true;
                    }
                }
            }
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square)
                {
                    square.MouseHoveringOnRelative = true;
                }
            }
        }
        protected override void UnhoverChildren()
        {
            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        myGridPoints[i, j].MouseHoveringOnRelative = false;
                    }
                }
            }
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square)
                {
                    square.MouseHoveringOnRelative = false;
                }
            }
        }
        override protected void ClickChildren()
        {
            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        myGridPoints[i, j].MouseClickingOnRelative = true;
                    }
                }
            }
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square)
                {
                    square.MouseClickingOnRelative = true;
                }
            }
        }
        protected override void UnclickChildren()
        {
            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        myGridPoints[i, j].MouseClickingOnRelative = false;
                    }
                }
            }
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square)
                {
                    square.MouseClickingOnRelative = false;
                }
            }
        }

        override protected void ColourAndHoverChildren()
        {
            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        myGridPoints[i, j].MouseHoveringOnRelative = true;
                        myGridPoints[i, j].SetColour(adjacentSelectedColour);
                    }
                }
            }
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square)
                {
                    square.MouseHoveringOnRelative = true;
                    square.SetColour(adjacentSelectedColour);
                }
            }
        }
        override protected void UncolourAndUnhoverChildren()
        {
            for (int i = 0; i < myGridPoints.GetLength(0); i++)
            {
                for (int j = 0; j < myGridPoints.GetLength(1); j++)
                {
                    if (myGridPoints[i, j] != null)
                    {
                        myGridPoints[i, j].MouseHoveringOnRelative = false;
                        myGridPoints[i, j].DistanceFadeIfUnclickedAndUnhovered();
                    }
                }
            }
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square)
                {
                    square.MouseHoveringOnRelative = false;
                    square.DistanceFadeIfUnclickedAndUnhovered();
                }
            }
        }


        // BUILT IN


        // OVERRIDES
        public override string ToString()
        {
            return string.Format("{0} (Grid_Square) : ({1}, {2})", gameObject.name, myGridCoordinates.x, myGridCoordinates.z);
        }
    }

}