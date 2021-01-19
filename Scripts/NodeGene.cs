using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeGene : Genome
{

    public enum NodeType {Input,Hidden,Output };

    [SerializeField]
    public int NodeID;
    public float Threshold;
    public NodeType MyNodeType;


    public float CurrentValue = 0;


    public int NoOfIncomingConnections = 0;
    public int CurNoOfConnectionsEvaluatedThisTick = 0;

    public List<ConnectionGene> ChildConnections = new List<ConnectionGene>();
    internal bool isFired;

    public float StepThresholdEvaluation (float input)
    {
        float value = 0;

        if(value > Threshold)
        {
            value = 1;
        }
        return value;
        
    }
   


   
}
