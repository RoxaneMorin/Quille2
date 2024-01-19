using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [CreateAssetMenu(fileName = "BasicNeed", menuName = "Quille/Personality/Personality Axe", order = 0)]
    public class PersonalityAxeSO : ScriptableObject
    {
        // VARIABLES/PARAMS 
        [SerializeField]
        private string axeName = "Undefined";
        public string AxeName { get { return axeName; } }

        [SerializeField]
        private string axeNameLeft = "Undefined (Left)";
        public string AxeNameLeft { get { return axeNameLeft; } }

        [SerializeField]
        private string axeNameRight = "Undefined (Right)";
        public string AxeNameRight { get { return axeNameRight; } }

        // Description.

        // AXE GRAPHICS
        public Sprite axeIconLeft;
        public Sprite axeIconRight;

        // OTHER VALUES
        private (float, float) axeSpan = (-Constants.AXE_HALF_SPAN, Constants.AXE_HALF_SPAN);
        public (float, float) AxeSpan { get { return axeSpan; } }
    }
}