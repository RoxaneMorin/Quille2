using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Quille
{
    public class Person_Mindspace : MonoBehaviour
    {
        // VARIABLES
        [SerializeReference] private List<Thought> myThoughts;


        // TEMPORARY TESTS
        [SerializeField] private Person targetPerson;
        [SerializeField] private ThoughtData exampleThoughtData;

        [SerializeField] private Thought newThought;


        [ContextMenu("Test giving a thought")]
        public void GivePersonThought()
        {
            if (myThoughts == null)
            {
                myThoughts = new List<Thought>();
            }

            newThought = exampleThoughtData.TryCreateThoughtFor(targetPerson);
            if (newThought != null)
            {
                myThoughts.Add(newThought);
            }
        }
    }
}

