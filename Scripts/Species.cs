using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Species : MonoBehaviour
{
   [SerializeField]
    public List<NeuralNetwork> Members = new List<NeuralNetwork>();
    [SerializeField]
    public int SpeciesID = 0;


    [SerializeField]

    public List<ConnectionGene> Representitive = new List<ConnectionGene>();


    public List<int> MemberCountPerGeneration = new List<int>();
    public List<float> FitnessPerGeneration = new List<float>();


    public Color MyColor = new Color(0, 0, 0, 0);
    public float GetTotalFitness()
    {
        float fitness = 0;
        foreach(NeuralNetwork neuralNet in Members)
        {
            fitness += neuralNet.AdjustedFitness;
        }
        return fitness;
    }


 
}
