using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quille.Genome))]
public class GenomeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Compute Phenotype"))
        {
            Quille.Genome current = this.target as Quille.Genome;
            Quille.Genome.SetPhenotype(current);
        }
    }
}