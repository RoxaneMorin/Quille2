using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChecksAndMods;
using Quille;

namespace World
{
    [System.Serializable]
    public class InteractionNeedEffectSettings
    {
        // VARIABLES/PARAMS
        [SerializeField] protected BasicNeedSO targetNeed;

        [SerializeField] protected float defaultNeedChangeRate;
        [SerializeField] protected float defaultMaxNeedChange;

        [SerializeReference] [PopulateModulatorSubtypes] protected ModulatorArithmetic[] needChangeRateModulatedBy;
        [SerializeReference] [PopulateModulatorSubtypes] protected ModulatorArithmetic[] maxNeedChangeModulatedBy;

        // TODO: should interactions have different base scores?


        // PROPERTIES
        public BasicNeedSO TargetNeed { get { return targetNeed; } }

        public float DefaultNeedChangeRate { get { return defaultNeedChangeRate; } }
        public float DefaultMaxNeedChange { get { return defaultMaxNeedChange; } }

        public ModulatorArithmetic[] NeedChangeRateModulatedBy { get { return needChangeRateModulatedBy; } }
        public ModulatorArithmetic[] MaxNeedChangeModulatedBy { get { return maxNeedChangeModulatedBy; } }
    }
}

