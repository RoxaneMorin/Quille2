using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

namespace Quille
{
    // The C# object class containing a person's identity and personality. 
    // This data is editable, but needs not be updated or accessed at every tick.
    // The instance data in this part of the class is JSON serialized.


    [System.Serializable]
    public partial class Person_Character
    {
        // VARIABLES/PARAMS

        // IDENTITY
        // Names
        [SerializeField] private string firstName, lastName, nickName;
        [SerializeField] private List<string> secondaryNames; // In case of multiple middle names :p not sure it'll actually be useful.

        // Age, gender, etc.


        // PERSONALITY
        // Personality scores.
        [SerializeField, SerializedDictionary("Personality Axe", "Score")]
        private SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes;

        [SerializeField, SerializedDictionary("Personality Trait", "Score")]
        private SerializedDictionary<PersonalityTraitSO, float> myPersonalityTraits;
        // Scores should be limited to 0.5 or 1.
        // A trait with a score of zero should be pruned away.

        // Drives
        [SerializeField, SerializedDictionary("Drives", "Score")]
        private SerializedDictionary<DriveSO, float> myDrives;
        // Scores should be limited to 0.5 or 1.
        // A drive with a score of zero should be pruned away.

        // Interests and preferences.
        [SerializeField, SerializedDictionary("Interest", "Score")]
        private SerializedDictionary<InterestSO, float> myInterests;
        //Scores range between 1 (loved) and -1 (hated).
        // A score of zero means the character know of this interest but is indifferent to it.
        // Interests that do not appear in the dictionary are unknown.

        // Misc personality stuff
        // Quirks, demeanor, etc.

        // Misc favourites
        // Colours, styles, etc.



        // PROPERTIES IN PARTIAL CLASS



        // CONSTRUCTORS
        // TODO: better constructors.
        public Person_Character(string myFirstName = null, string myLastName = null, string myNickName = null, List<string> mySecondaryNames = null,
                                SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes = null, SerializedDictionary<PersonalityTraitSO, float> myPersonalityTraits = null, SerializedDictionary<DriveSO, float> myDrives = null, SerializedDictionary<InterestSO, float> myInterests = null)
        {
            this.FirstName = myFirstName;
            this.LastName = myLastName;
            this.NickName = myNickName;
            this.SecondaryNames = mySecondaryNames;

            // PersonalityAxes are a special case. Everyone has them, even if their values are zero.
            if (myPersonalityAxes == null || myPersonalityAxes.Count == 0)
            {
                PopulatePersonalityAxesDict();
            }
            else
            {
                this.SetAxeScoreDict(myPersonalityAxes);
            }

            this.SetTraitScoreDict(myPersonalityTraits != null ? myPersonalityTraits : new SerializedDictionary<PersonalityTraitSO, float>(), false);
            this.SetDriveScoreDict(myDrives != null ? myDrives : new SerializedDictionary<DriveSO, float>(), false);
            this.SetInterestScoreDict(myInterests != null ? myInterests : new SerializedDictionary<InterestSO, float>(), false);
        }



        // METHODS

        // JSON SAVE & LOAD
        internal string SaveToJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        internal void LoadFromJSON(string sourceJson)
        {
            // Empty the existing lists and dictionaries.
            // TODO: is there a better way to manage this?
            if (secondaryNames != null)
                secondaryNames.Clear();

            if (myPersonalityAxes != null)
                myPersonalityAxes.Clear();
            if (myPersonalityTraits != null)
                myPersonalityTraits.Clear();
            if (myDrives != null) 
                myDrives.Clear();
            if (myInterests != null)
                myInterests.Clear();

            JsonConvert.PopulateObject(sourceJson, this);
        }

        internal static Person_Character CreateFromJSON(string sourceJson)
        {
            return JsonConvert.DeserializeObject<Person_Character>(sourceJson);
        }


        // UTILITY
        public void PopulatePersonalityAxesDict(bool randomize = false)
        {
            PersonalityAxeSO[] personalityAxeSOs = Resources.LoadAll<PersonalityAxeSO>(Constants_PathResources.SO_PATH_PERSONALITYAXES);
            this.myPersonalityAxes =personalityAxeSOs.ToSerializedDictionary(personalityAxeSO => personalityAxeSO, personalityAxeSO => (randomize ? RandomExtended.RangeStepped(-1f, 1f, 0.125f) : 0f));
        }
        public void ClearPersonalityAxesDict()
        {
            this.myPersonalityAxes = new SerializedDictionary<PersonalityAxeSO, float>();
        }

        public void RandomPopulatePersonalityTraitsDict(bool randomize = false)
        {
            // TODO: avoid incompatible traits.

            PersonalityTraitSO[] personalityTraitSOs = Resources.LoadAll<PersonalityTraitSO>(Constants_PathResources.SO_PATH_PERSONALITYTRAITS);
            int SOCount = personalityTraitSOs.Length;

            // Make sure we're not trying to get more than the existing number of traits.
            int targetCount = Mathf.Min(Constants_Quille.DEFAULT_PERSONALITY_TRAIT_COUNT, SOCount);
            List<int> targetIDs = RandomExtended.NonRepeatingIntegersInRange(0, SOCount, targetCount);

            this.myPersonalityTraits = targetIDs.ToSerializedDictionary(ID => personalityTraitSOs[ID], ID => (randomize ? RandomExtended.CoinFlipBetween(0.5f, 1f) : 1f));
        }
        public void ClearPersonalityTraitsDict()
        {
            this.myPersonalityTraits = new SerializedDictionary<PersonalityTraitSO, float>();
        }

        public void RandomPopulateDrivesDict(bool randomize = false)
        {
            // TODO: avoid incompatible drives.

            DriveSO[] driveSOs = Resources.LoadAll<DriveSO>(Constants_PathResources.SO_PATH_DRIVES);
            int SOCount = driveSOs.Length;

            // Make sure we're not trying to get more than the existing number of drives.
            int targetCount = Mathf.Min(Constants_Quille.DEFAULT_DRIVES_COUNT, SOCount);
            List<int> targetIDs = RandomExtended.NonRepeatingIntegersInRange(0, SOCount, targetCount);

            this.myDrives = targetIDs.ToSerializedDictionary(ID => driveSOs[ID], ID => (randomize ? RandomExtended.CoinFlipBetween(0.5f, 1f) : 1f));
        }
        public void ClearDrivesDict()
        {
            this.myDrives = new SerializedDictionary<DriveSO, float>();
        }

        public void RandomPopulateInterestsDict(bool randomize = false)
        {
            InterestSO[] interestSOs = Resources.LoadAll<InterestSO>(Constants_PathResources.SO_PATH_INTERESTS);
            int SOCount = interestSOs.Length;

            // Make sure we're not trying to get more than the existing number of drives.
            int targetCount = Mathf.Min(Constants_Quille.DEFAULT_INTEREST_COUNT, SOCount);
            List<int> targetIDs = RandomExtended.NonRepeatingIntegersInRange(0, SOCount, targetCount);

            this.myInterests = targetIDs.ToSerializedDictionary(ID => interestSOs[ID], ID => (randomize ? RandomExtended.RangeStepped(-1f, 1f, 0.125f) : 0f));
        }
        public void ClearInterestDict()
        {
            this.myInterests = new SerializedDictionary<InterestSO, float>();
        }

    }
}
