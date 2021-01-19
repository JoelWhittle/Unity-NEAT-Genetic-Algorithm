using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConnectionGene : Genome
{

    public int InNode;
    public int OutNode;
    public float Weight;
    public bool Enabled;
    public int InnovationID;

    public NodeGene ChildNode;

}
