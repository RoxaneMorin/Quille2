using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Quille
{
    // Container and controller for characters' genetic information.
    // It is quite old and should be rewritten as a basic C# object.
    // Its instanced data should be JSON serializable.


    [System.Serializable]
    public class Genome : MonoBehaviour
    {
        // STATIC VARIABLES

        static GeneSkintone[] defaultSkintones;
        static GeneEyeColour[] possibleEyeColours;
        static GeneHairColour[] possibleHairColours;


        // INSTANTIATED VARIABLES

        [Header("Parentage")]
        [SerializeField] Genome genomeParentA;
        [SerializeField] Genome genomeParentB;

        [Space(10)]

        [Header("Genotype")]
        [SerializeField] Color skintoneParentA;
        [SerializeField] Color skintoneParentB;
        [SerializeField, Range(0f, 1f)] float skintoneBlendWeight;
        [SerializeField] GeneEyeColour eyeColourAlleleA, eyeColourAlleleB;
        [SerializeField] GeneHairColour hairColourAlleleA, hairColourAlleleB;

        [Space(10)]

        [Header("Phenotype")]
        [SerializeField] public Color skintone;
        [SerializeField] public Color eyeColour;
        [SerializeField] public Color hairColour;



        // STATIC METHODS


        // Creates a quille's genotype from its parents'.
        static void InheritGenotype(Genome parentA, Genome parentB, Genome child)
        {
            // Keep track of parents' skin colours.
            child.skintoneParentA = parentA.skintone;
            child.skintoneParentB = parentB.skintone;

            // Decide on a random blend weight between them.
            child.skintoneBlendWeight = Random.value;

            // Receive eye alleles.
            child.eyeColourAlleleA = GetRandomEyeAllele(parentA);
            child.eyeColourAlleleB = GetRandomEyeAllele(parentB);

            // Receive hair alleles.
            child.hairColourAlleleA = GetRandomHairAllele(parentA);
            child.hairColourAlleleB = GetRandomHairAllele(parentB);
        }

        static GeneEyeColour GetRandomEyeAllele(Genome genome)
        {
            if (Random.value > 0.5) { return genome.eyeColourAlleleA; }
            else return genome.eyeColourAlleleB;
        }

        static GeneHairColour GetRandomHairAllele(Genome genome)
        {
            if (Random.value > 0.5) { return genome.hairColourAlleleA; }
            else return genome.hairColourAlleleB;
        }


        // Updates a quille's phenotype from its genotype.
        public static void SetPhenotype(Genome genome)
        {
            genome.skintone = Color.Lerp(genome.skintoneParentA, genome.skintoneParentB, genome.skintoneBlendWeight);

            genome.eyeColour = ExpressDominantAllele(genome.eyeColourAlleleA, genome.eyeColourAlleleB);
            genome.hairColour = ExpressDominantAllele(genome.hairColourAlleleA, genome.hairColourAlleleB);
        }

        static Color ExpressDominantAllele(GeneWithDominance alleleA, GeneWithDominance alleleB)
        {
            if (alleleA.GeneDominance > alleleB.GeneDominance) { return Color.Lerp(alleleA.Colour, alleleB.Colour, 0.1f); }
            else
            if (alleleA.GeneDominance < alleleB.GeneDominance) { return Color.Lerp(alleleA.Colour, alleleB.Colour, 0.9f); }

            else { return Color.Lerp(alleleA.Colour, alleleB.Colour, 0.5f); }
        }

    }

}

