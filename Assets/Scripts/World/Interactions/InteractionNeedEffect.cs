using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;
using Quille;

namespace World
{
    [System.Serializable]
    public class InteractionNeedEffect
    {
        // VARIABLES/PARAMS
        [SerializeField] protected BasicNeedSO targetNeed;
        
        [SerializeField] protected float defaultNeedChangeRate;
        [SerializeField] protected float defaultMaxNeedChange;
        [Space]
        [SerializeReference] [PopulateModulatorSubtypes] protected Modulator[] needChangeRateModulatedBy;
        [SerializeReference] [PopulateModulatorSubtypes] protected Modulator[] maxNeedChangeModulatedBy;     


        // PROPERTIES
        public BasicNeedSO TargetNeed { get { return targetNeed; } }

        public float DefaultNeedChangeRate { get { return defaultNeedChangeRate; } }
        public float DefaultMaxNeedChange { get { return defaultMaxNeedChange; } }

        public Modulator[] NeedChangeRateModulatedBy { get { return needChangeRateModulatedBy; } }
        public Modulator[] MaxNeedChangeModulatedBy { get { return maxNeedChangeModulatedBy; } }
    }
}

