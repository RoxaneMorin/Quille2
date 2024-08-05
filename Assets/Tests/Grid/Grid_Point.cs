using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace proceduralGrid
{
    [System.Serializable]
    public class Grid_Point : Grid_Item
    {
        // VARIABLES

        // Grid information
        [SerializeField] private FourAdjacentSquares myAdjacentGridSquares;
        public FourAdjacentSquares MyAdjacentGridSquares { get { return myAdjacentGridSquares; } }



        // METHODS

        // INIT
        public void SetVariables(Grid_Base myParentGrid, CoordPair myGridCoordinates, Grid_Square bl, Grid_Square br, Grid_Square tl, Grid_Square tr)
        {
            this.myParentGrid = myParentGrid;
            this.myGridCoordinates = myGridCoordinates;
            myAdjacentGridSquares = new FourAdjacentSquares(bl, br, tl, tr);
        }

        public void SetAdjacentGridSquare(Adjacencies adjacency, Grid_Square newGridSquare)
        {
            switch (adjacency)
            {
                case Adjacencies.bl:
                    myAdjacentGridSquares.bl = newGridSquare;
                    break;
                case Adjacencies.br:
                    myAdjacentGridSquares.br = newGridSquare;
                    break;
                case Adjacencies.tl:
                    myAdjacentGridSquares.tl = newGridSquare;
                    break;
                case Adjacencies.tr:
                    myAdjacentGridSquares.tr = newGridSquare;
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

            myParentGrid.MoveVertex(myGridCoordinates, upwardMotion3D);
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square != null)
                {
                    square.AverageMyHeightFromMyGridPoints();
                }
            }
        }

        // CHILDREN
        protected override void HoverChildren()
        {
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
            foreach (Grid_Square square in myAdjacentGridSquares)
            {
                if (square != null)
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
            return string.Format("{0} (Grid_Point) : ({1}, {2})", gameObject.name, myGridCoordinates.x, myGridCoordinates.z);
        }
    }

}