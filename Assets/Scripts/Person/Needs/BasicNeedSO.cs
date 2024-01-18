using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    [CreateAssetMenu(fileName = "BasicNeed", menuName = "Quille/Needs/Basic Need", order = 0)]
    public class BasicNeedSO : ScriptableObject
    {
        // VARIABLES/PARAMS   
        [SerializeField]
        private string needName = "Undefined";
        public string NeedName { get { return needName; } }

        // NEED GRAPHICS
        public Sprite needIcon;

        // NEED'S DEFAULT VALUES
        [SerializeField, Range(0, Constants.MAX_PRIORITY)] // Priorize larger values? 
        private int aiPriorityWeighting; // Should this be static, since it'll likely by the same for all characters?
        public int AiPriorityWeighting { get { return aiPriorityWeighting; } }

        [SerializeField] // not sure about the nomenclature for these. level, range, gauge, etc? 
        private float levelFull = Constants.LEVEL_FULL;
        public float LevelFull { get { return levelFull; } }
        public float LevelEmpty { get { return Constants.LEVEL_EMPTY; } }// Will always be 0?

        [SerializeField]
        private float defaultChangeRate = 0; // the need's universal default decay rate.
        public float DefaultChangeRate { get { return defaultChangeRate; } }


        //Default values modulated by ? (List of functions/references)
    }
}