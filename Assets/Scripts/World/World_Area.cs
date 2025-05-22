using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using ChecksAndMods;
using Quille;


namespace World
{
    public class World_Area : MonoBehaviour
    {
        // The representation of the space that physically exists in the world, containing people and objects.
        // Keeps track of interactions, rooms, and the like for ease of access.


        // VARIABLES/PARAMS
        [SerializeField] [SerializedDictionary("Target Need", "Interactions")] protected SerializedDictionary<BasicNeedSO, List<LocalInteraction>> localInteractions;

        
        // PROPERTIES
        public SerializedDictionary<BasicNeedSO, List<LocalInteraction>> LocalInteractions { get { return localInteractions; } }
        public List<LocalInteraction> LocalInteractionsFor(BasicNeedSO thisNeed)
        {
            // TODO: what to return if no interactions are found for the need?

            //
            if (localInteractions.ContainsKey(thisNeed))
            {
                return localInteractions[thisNeed];
            }
            else
            {
                return null;
            }
        }



        // METHODS

        // EVENT LISTENERS
        protected void RegisterLocalInteraction(BasicNeedSO theNeed, LocalInteraction theInteraction)
        {
            // Create the dictionary if it does not already exist.
            if (localInteractions == null)
            {
                localInteractions = new SerializedDictionary<BasicNeedSO, List<LocalInteraction>>();
            }
            
            // Create the key if it does not already exist.
            if (!localInteractions.ContainsKey(theNeed))
            {
                localInteractions.Add(theNeed, new List<LocalInteraction>());
            }

            // Add the interaction to the relevant list.
            localInteractions[theNeed].Add(theInteraction);
        }
        protected void UnregisterLocalInteraction(BasicNeedSO theNeed, LocalInteraction theInteraction)
        {
            if (localInteractions != null && localInteractions.ContainsKey(theNeed))
            {
                if (localInteractions[theNeed].Contains(theInteraction))
                {
                    localInteractions[theNeed].Remove(theInteraction);
                }
            }
        }


        // BUILT IN
        private void Start()
        {
            LocalInteraction.SendInteractionNeedAdvertisement += RegisterLocalInteraction;
            LocalInteraction.SendInteractionNeedDeletion += UnregisterLocalInteraction;
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    TempScoreEachInteractionFor(tempPerson);
            //}
        }



        // TEMPORARY TEST STUFF
        //[SerializeField] private Person tempPerson;
        //protected void TempScoreEachInteractionFor(Person thisPerson)
        //{
        //    foreach (List<LocalInteraction> listOfInteractions in localInteractions.Values)
        //    {
        //        foreach (LocalInteraction interaction in listOfInteractions)
        //        {
        //            float score = interaction.ScoreFor(thisPerson);

        //            Debug.Log(string.Format("{0}'s score for the {1}: {2}", thisPerson.MyPersonCharacter.FirstAndLastName, interaction, score));
        //        }
        //    }
        //}
    }
}

