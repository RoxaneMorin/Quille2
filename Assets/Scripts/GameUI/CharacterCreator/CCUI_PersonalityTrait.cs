using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuilleUI
{
    public class CCUI_PersonalityTrait : MonoBehaviour
    {
        // Test setup for a character creator's individual personality trait UI.


        // VARIABLES
        [SerializeField] Quille.PersonalityTraitSO myPersonalityTraitSO;
        [SerializeField] bool isSelected;

        [Header("Resources")]
        [SerializeField] Button myButton;

        [SerializeField] Sprite mySprite;
        [SerializeField] Color myColourDefault;
        [SerializeField] Color myColourSelected;
        [SerializeField] Color myColourForbidden;
        [SerializeField] Color myColourSelectedeButForbidden;





        // METHODS

        // BUILT IN
        void Start() {}
    }
}

