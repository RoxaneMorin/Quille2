using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // Data container used to define the wider effects of personality items on the character AI.
    // TODO: the actual loading into a Person_AI.


    [CreateAssetMenu(fileName = "PersonalityAIEffects", menuName = "Quille/Personality AI Effects", order = 10)]
    public class PersonalityAIEffects : ScriptableObject
    {
        // VARIABLES/PARAMS

        // Concerning Basic Needs
        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] basicNeedThresholdsModulatedBy;
        public ChecksAndMods.Modulator[] BasicNeedThresholdsModulatedBy { get { return basicNeedThresholdsModulatedBy; } }


        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] basicNeedNoticeModulatedBy;
        public ChecksAndMods.Modulator[] BasicNeedNoticeModulatedBy { get { return basicNeedNoticeModulatedBy; } }


        // Concerning Subjective Needs
        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] subjectiveNeedThresholdsModulatedBy;
        public ChecksAndMods.Modulator[] SubjectiveNeedThresholdsModulatedBy { get { return subjectiveNeedThresholdsModulatedBy; } }


        [SerializeReference][PopulateModulatorSubtypes] private ChecksAndMods.Modulator[] subjectiveNeedNoticeModulatedBy;
        public ChecksAndMods.Modulator[] SubjectiveNeedNoticeModulatedBy { get { return subjectiveNeedNoticeModulatedBy; } }
    }
}
 