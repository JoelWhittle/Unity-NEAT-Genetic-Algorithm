using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static NodeGene;

[Serializable]
public class NeuralNetwork : MonoBehaviour
{
    public Genotype MyGenotype;

    public float Fitness;
    public float AdjustedFitness;
    public List<NodeGene> InputNodes = new List<NodeGene>();
    public List<NodeGene> OutputNodes = new List<NodeGene>();
    public List<NodeGene> HiddenNodes = new List<NodeGene>();

    public List<NodeGene> GlobalNodes = new List<NodeGene>();


    public List<ConnectionGene> Connections = new List<ConnectionGene>();


    public List<int> InputNodeIDs = new List<int>();
    public List<int> OutputNodeIDs = new List<int>();

    public int SpeciesID = 0;
    
    public void Tick()
    {
        foreach(NodeGene nodeGene in GlobalNodes)
        {
            nodeGene.CurNoOfConnectionsEvaluatedThisTick = 0;
            nodeGene.isFired = false;
            if (nodeGene.MyNodeType != NodeType.Input)
            {
                nodeGene.CurrentValue = 0;
            }


        }

        //start with each input?
        foreach(NodeGene inputNode in InputNodes)
        {
            foreach(ConnectionGene childConnectionGene in inputNode.ChildConnections)
            {
                if (childConnectionGene.Enabled)
                {
                    childConnectionGene.ChildNode.CurNoOfConnectionsEvaluatedThisTick++;
                    childConnectionGene.ChildNode.CurrentValue += (inputNode.CurrentValue * childConnectionGene.Weight);
                }
            }
        }

        //tommy added 
        while (HiddenNodes.Any(a => a.isFired == true))
        {
            //then hidden? will this work?
            foreach (NodeGene hiddenNode in HiddenNodes.Where(a => a.isFired == false))
            {
                if (hiddenNode.CurNoOfConnectionsEvaluatedThisTick == hiddenNode.NoOfIncomingConnections && hiddenNode.CurrentValue > hiddenNode.Threshold)
                {
                    hiddenNode.isFired = true;

                    foreach (ConnectionGene childConnectionGene in hiddenNode.ChildConnections)
                    {
                        if (childConnectionGene.Enabled)
                        {
                            childConnectionGene.ChildNode.CurNoOfConnectionsEvaluatedThisTick++;
                            childConnectionGene.ChildNode.CurrentValue += (hiddenNode.StepThresholdEvaluation(hiddenNode.CurrentValue) * childConnectionGene.Weight);
                        }
                    }
                }
            }
        }
    }

    public float GetTotalConnectionWeight()
    {
        float weight = 0;

        foreach(ConnectionGene connectionGene in Connections)
        {
            weight += connectionGene.Weight;
        }

        return weight;
    }

    public void CountNodeIncomingConnections()
    {
        foreach(NodeGene nodeGene in GlobalNodes)
        {
            nodeGene.NoOfIncomingConnections = 0;
            foreach(ConnectionGene connectionGene in Connections)
            {
                if(connectionGene.OutNode == nodeGene.NodeID)
                {
                    nodeGene.NoOfIncomingConnections++;
                }

                if(connectionGene.InNode == nodeGene.NodeID)
                {
                    nodeGene.ChildConnections.Add(connectionGene);

                    foreach (NodeGene otherNodeGene in GlobalNodes)
                    {
                        if (otherNodeGene.NodeID == connectionGene.OutNode)
                        {
                            connectionGene.ChildNode = otherNodeGene;
                        }
                    }
                }

           
            }
        }
    }

    public void CreateEmptyNeuralNetwork(int noOfInputs, int noOfOutputs, int noOfHiddenNodes )
    {
        if(!MyGenotype)
        {
            MyGenotype = gameObject.AddComponent<Genotype>();
        }

        int nodeIDCount = 0;

        for(int i = 0; i < noOfInputs; i++)
        {
            NodeGene newNode = gameObject.AddComponent<NodeGene>();
            MyGenotype.Genomes.Add(newNode);
            newNode.NodeID = nodeIDCount;
            newNode.MyNodeType = NodeType.Input;

            nodeIDCount++;

            InputNodes.Add(newNode);
            InputNodeIDs.Add(newNode.NodeID);
            GlobalNodes.Add(newNode);
        }

        for (int i = 0; i < noOfOutputs; i++)
        {
            NodeGene newNode = gameObject.AddComponent<NodeGene>();
            MyGenotype.Genomes.Add(newNode);
            newNode.NodeID = nodeIDCount;
            newNode.MyNodeType = NodeType.Output;

            nodeIDCount++;
            OutputNodes.Add(newNode);
            GlobalNodes.Add(newNode);
            OutputNodeIDs.Add(newNode.NodeID);


        }
        for (int i = 0; i < noOfHiddenNodes; i++)
        {
            NodeGene newNode = gameObject.AddComponent<NodeGene>();
            MyGenotype.Genomes.Add(newNode);
            newNode.NodeID = nodeIDCount;
            newNode.MyNodeType = NodeType.Hidden;

            nodeIDCount++;
            HiddenNodes.Add(newNode);
            GlobalNodes.Add(newNode);


        }

        int innovationCount = 0;
        foreach(NodeGene inNode in InputNodes)
        {
            foreach (NodeGene outNode in OutputNodes)
            {
                ConnectionGene newConnection = gameObject.AddComponent<ConnectionGene>();
                MyGenotype.Genomes.Add(newConnection);

                newConnection.InnovationID = innovationCount;
                newConnection.InNode = inNode.NodeID;
                newConnection.OutNode = outNode.NodeID;
                newConnection.Enabled = true;
                newConnection.Weight = UnityEngine.Random.Range(-2f, 2f);
                Connections.Add(newConnection);

                innovationCount++;
            }
        }
        CountNodeIncomingConnections();
    }



    public void CreateNeuralNetworkFromGenome(List<ConnectionGene> genome)
    {
        EraseNeuralNetwork();
        //  Debug.Log("==CreatingFromeGenomeDebug==");

        foreach (ConnectionGene oldCG in genome)
        {
            ConnectionGene newCG = gameObject.AddComponent<ConnectionGene>();
            newCG.InNode = oldCG.InNode;
            newCG.OutNode = oldCG.OutNode;
            newCG.Weight = oldCG.Weight;
            newCG.Enabled = oldCG.Enabled;

            newCG.InnovationID = oldCG.InnovationID;
        
           // Debug.Log("Old Innov Id: " + oldCG.InnovationID + "  New Innov Id:" + newCG.InnovationID);
           // Debug.Log("Old Innov Weight: " + oldCG.Weight + "  New  Innov Weight :" + newCG.Weight);

            

            MyGenotype.Genomes.Add(newCG);
            Connections.Add(newCG);

            if (!NeuralNetworkContainsNeuronWithID(newCG.InNode))
            {

                NodeGene newNode = gameObject.AddComponent<NodeGene>();
                MyGenotype.Genomes.Add(newNode);
                newNode.NodeID = newCG.InNode;
                GlobalNodes.Add(newNode);

                if (InputNodeIDs.Contains(newNode.NodeID))
                {
                    InputNodes.Add(newNode);
                    newNode.MyNodeType = NodeType.Input;
                }
                else if (OutputNodeIDs.Contains(newNode.NodeID))
                {
                    OutputNodes.Add(newNode);
                    newNode.MyNodeType = NodeType.Output;

                }
                else
                {
                    HiddenNodes.Add(newNode);
                    newNode.MyNodeType = NodeType.Hidden;
                }

            }
            if (!NeuralNetworkContainsNeuronWithID(newCG.OutNode))
            {
              

                NodeGene newNode = gameObject.AddComponent<NodeGene>();
                MyGenotype.Genomes.Add(newNode);
                newNode.NodeID = newCG.OutNode;
                GlobalNodes.Add(newNode);

                if (InputNodeIDs.Contains(newNode.NodeID))
                {
                    InputNodes.Add(newNode);
                    newNode.MyNodeType = NodeType.Input;
                }
                else if (OutputNodeIDs.Contains(newNode.NodeID))
                {
                    OutputNodes.Add(newNode);
                    newNode.MyNodeType = NodeType.Output;

                }
                else
                {
                    HiddenNodes.Add(newNode);
                    newNode.MyNodeType = NodeType.Hidden;
                }
            }
        }
        // Debug.Log("==End==");

        CountNodeIncomingConnections();

    }

    public bool NeuralNetworkContainsNeuronWithID(int ID)
    {
        bool returnBool = false;

        foreach(NodeGene nodeGene in GlobalNodes)
        {
            if(nodeGene.NodeID == ID)
            {
                returnBool = true;
            }
        }
        return returnBool;
    }

    public void EraseNeuralNetwork()
    {
        foreach(Genome g in MyGenotype.Genomes)
        {
            Destroy(g);
        
        }
        MyGenotype.Genomes.Clear();
        InputNodes.Clear();
        OutputNodes.Clear();
        HiddenNodes.Clear();
        Connections.Clear();
        GlobalNodes.Clear();

    }
}
