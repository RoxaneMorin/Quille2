using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of PersonalityAxes, used for the creation of specific axes as assets.
    // These axes represent personality spectrums upon which everyone will fall, such as intro vs extraversion.


    [CreateAssetMenu(fileName = "PersonalityAxe", menuName = "Quille/Character/Personality Axe", order = 0)]
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
        [SerializeField] [BeginInspectorReadOnlyGroup] private floatPair axeSpan = (-Constants.PERSONALITY_HALF_SPAN, Constants.PERSONALITY_HALF_SPAN);
        //[EndInspectorReadOnlyGroup]
        public floatPair AxeSpan { get { return axeSpan; } }
    }
}