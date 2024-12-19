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
        // 


        // VARIABLES/PARAMS
        [SerializeField] [SerializedDictionary("Target Need", "Interactions")] protected SerializedDictionary<BasicNeedSO, List<LocalInteraction>> localInteractions;



        // METHODS

        // UTILITY
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


        // BUILT IN
        private void Start()
        {
            LocalInteraction.SendInteractionNeedAdvertisement += RegisterLocalInteraction;
        }
    }
}

