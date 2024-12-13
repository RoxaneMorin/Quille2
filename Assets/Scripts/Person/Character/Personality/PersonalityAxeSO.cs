using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // The ScriptableObject template of PersonalityAxes, used for the creation of specific axes as assets.
    // These axes represent personality spectrums upon which everyone will fall, such as intro vs extraversion.


    [CreateAssetMenu(fileName = "PersonalityAxe", menuName = "Quille/Character/Personality Axe", order = 0)]
    public class PersonalityAxeSO : PersonalityItemSO
    {
        // VARIABLES/PARAMS 
        [SerializeField] private string axeNameLeft = "Undefined (Left)";
        [SerializeField] private string axeNameRight = "Undefined (Right)";

        [SerializeField] private Sprite axeIconLeft;
        [SerializeField] private Sprite axeIconRight;

        [SerializeField] [BeginInspectorReadOnlyGroup] private floatPair axeSpan = (-Constants_Quille.PERSONALITY_HALF_SPAN, Constants_Quille.PERSONALITY_HALF_SPAN); //[EndInspectorReadOnlyGroup]


        // PROPERITES
        public string AxeNameLeft { get { return axeNameLeft; } }
        public string AxeNameRight { get { return axeNameRight; } }
        public Sprite AxeIconLeft { get { return axeIconLeft; } }
        public Sprite AxeIconRight { get { return axeIconRight; } }
        public floatPair AxeSpan { get { return axeSpan; } }
    }
}