using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [SerializeField] private int charID;
        // TODO: determine whether and how to use this unique numerical ID.
        // TODO: decide whether it should reside in the base Person class instead.

        [Space]

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

        // Interests and preferences.
        [SerializeField, SerializedDictionary("Interest", "Score")]
        private SerializedDictionary<InterestSO, float> myInterests;
        //Scores range between 1 (loved) and -1 (hated).
        // A score of zero means the character know of this interest but is indifferent to it.
        // Interests that do not appear in the dictionary are unknown.

        // Ambitions and drives.

        // Misc personality stuff
        // Quirks, demeanor, etc.

        // Misc favourites
        // Colours, styles, etc.



        // PROPERTIES IN PARTIAL CLASS



        // CONSTRUCTORS
        // TODO: better constructors.
        public Person_Character(string myFirstName = null, string myLastName = null, string myNickName = null, List<string> mySecondaryNames = null,
                                SerializedDictionary<PersonalityAxeSO, float> myPersonalityAxes = null, SerializedDictionary<PersonalityTraitSO, float> myPersonalityTraits = null, SerializedDictionary<InterestSO, float> myInterests = null)
        {
            // TODO: handle charID.
            
            this.FirstName = myFirstName;
            this.LastName = myLastName;
            this.NickName = myNickName;
            this.SecondaryNames = mySecondaryNames;

            this.SetAxeScoreDict(myPersonalityAxes != null ? myPersonalityAxes: new SerializedDictionary<PersonalityAxeSO, float>(), false);
            this.SetTraitScoreDict(myPersonalityTraits != null ? myPersonalityTraits : new SerializedDictionary<PersonalityTraitSO, float>(), false);
            this.SetInterestScoreDict(myInterests != null ? myInterests : new SerializedDictionary<InterestSO, float>(), false);
        }



        // METHODS

        // SAVE
        internal string SaveToJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        // LOAD
        internal void LoadFromJSON(string sourceJson)
        {
            JsonConvert.PopulateObject(sourceJson, this);
        }

        // UTILITY
        internal string CreateJSONFileName()
        {
            // TODO: handle what happens when a character is renamed.
            string strippedFirstName = Regex.Replace(firstName, "[^\\w\\._]", "");
            string strippedLastName = Regex.Replace(lastName, "[^\\w\\._]", "");

            return string.Format("{0}_{1}_{2}.json", charID, strippedFirstName, strippedLastName);
        }
    }
}
