using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticEvolutionManager : MonoBehaviour
{
    public int HighestGlobalNeuronID = 0;
    public int HighestGlobalInnovationID = 0;






    public float MutateRate;



    public float SpeciationDisjointedCoEff = 1;
    public float SpeciationWeightCoEff = 1;
    public float SpeciationExcessCoEff = 1;
    public float SpeciesSimiliarityThreshold = 2;

    public float GenotypeComparisonBiggestGenomeThreshold = 15f;

    public int EliteRatio = 20;
    public List<Species> CurrentSpecies = new List<Species>();


    public List<int> ParentSpeciesRatioPool = new List<int>();
    public int SpeciesCounter = 0;



    public int ConnectionDisableRatio;
    public int NewConnectionRatio;
    public int GuassianWeightChangeRatio;
    public int RandomWeightChangeRatio;


    public List<int> MutationRatioPool = new List<int>();

    [SerializeField]
    public List<NeuralNetwork> NewPopulation = new List<NeuralNetwork>();

    void Start()
    {
    for(int i = 0; i < ConnectionDisableRatio; i ++)
        {
            MutationRatioPool.Add(0);
        }
        for (int i = 0; i < NewConnectionRatio; i++)
        {
            MutationRatioPool.Add(1);
        }
        for (int i = 0; i < GuassianWeightChangeRatio; i++)
        {
            MutationRatioPool.Add(2);
        }
        for (int i = 0; i < RandomWeightChangeRatio; i++)
        {
            MutationRatioPool.Add(3);
        }
     
    }

    public bool IsChildValid(List<ConnectionGene> child)
    {
        bool result = false;

        bool foundInput0 = false;
        bool foundInput1 = false;
        bool foundInput2 = false;

        bool foundOutput1 = false;

        foreach(ConnectionGene cg in child)
        {
            if(cg.InNode == 0)
            {
                foundInput0 = true;
            }
            if (cg.InNode == 1)
            {
                foundInput1 = true;
            }
            if (cg.InNode == 2)
            {
                foundInput2 = true;
            }
            if (cg.OutNode == 3)
            {
                foundOutput1 = true;
            }
        }

        if(foundInput0 && foundInput1 && foundInput2 && foundOutput1)
        {
            result = true;
        }
        else
        {
          //  Debug.Log("found early invalid child");
        }
        return result;
    }

    public List<ConnectionGene> CreateChildFromParents(NeuralNetwork parentANet, NeuralNetwork parentBNet, float mutateRate)
    {
        bool madeValidChild = false;
        List<ConnectionGene> child = new List<ConnectionGene>();

        while (madeValidChild == false)
        {
            List<ConnectionGene> parentA = parentANet.Connections;
            List<ConnectionGene> parentB = parentBNet.Connections;

            int inputs = 0;
            int outputs = 0;

            //Cross over


            int highestSharedInnovationID = 0;
            int highestInnovationID = 0;

            bool parentAWasLastInnovator = false;

            foreach (ConnectionGene connectionGene in parentA)
            {
                if (connectionGene.InnovationID > highestInnovationID)
                {
                    highestInnovationID = connectionGene.InnovationID;
                }
            }
            foreach (ConnectionGene connectionGene in parentB)
            {
                if (connectionGene.InnovationID > highestInnovationID)
                {
                    highestInnovationID = connectionGene.InnovationID;
                }
            }


            for (int i = 0; i < highestInnovationID + 1; i++)
            {

                if (GenomeContainsInnovationID(i, parentA) && GenomeContainsInnovationID(i, parentB))
                {
                    highestSharedInnovationID = i;
                    int r = Random.Range(0, 2);
                    if (r == 0)
                    {
                        child.Add(CopyConnectionGene(parentA[IndexOfInnovationID(i, parentA)]));

                    
                    }
                    else
                    {
                        child.Add(CopyConnectionGene(parentB[IndexOfInnovationID(i, parentB)]));
                       

                    }
                }
                else if (GenomeContainsInnovationID(i, parentA))
                {
                    child.Add(CopyConnectionGene(parentA[IndexOfInnovationID(i, parentA)]));

                    parentAWasLastInnovator = true;
                }
                else
                {
                    child.Add(CopyConnectionGene(parentB[IndexOfInnovationID(i, parentB)]));
                    parentAWasLastInnovator = false;

                }


            }
            //check to see who was last innovator to remove excess based of fitness 

            if (highestSharedInnovationID < child.Count)
            {
                if (parentAWasLastInnovator)
                {
                    if (parentBNet.Fitness > parentANet.Fitness)
                    {
                        for (int i = highestSharedInnovationID; i < child.Count; i++)
                        {
                            child.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    if (parentANet.Fitness > parentBNet.Fitness)
                    {
                        for (int i = highestSharedInnovationID; i < child.Count; i++)
                        {
                            child.RemoveAt(i);
                        }
                    }
                }
            }

            //mutate
            int curChildSize = child.Count;
            for (int i = 0; i < curChildSize; i++)
            {
                //   Debug.Log("thinking about  mutate");

                int r = Random.Range(0, 100);
                if (r < mutateRate)
                {
                    child = MutateGenotype(i, child);
                }
            }


            if (IsChildValid(child))
            {
                madeValidChild = true;
            }

        }
            return child;
        
    }

    public List<ConnectionGene> MutateGenotype (int id, List<ConnectionGene> genome)
    {
        List<ConnectionGene> source = new List<ConnectionGene>();
        foreach (ConnectionGene oldCG in genome)
        {
            ConnectionGene newCG = new ConnectionGene();
            newCG.InNode = oldCG.InNode;
            newCG.OutNode = oldCG.OutNode;
            newCG.Weight = oldCG.Weight;
            newCG.Enabled = oldCG.Enabled;
            newCG.InnovationID = oldCG.InnovationID;
            source.Add(newCG);
        }

      //  Debug.Log("Trying to mutate");


        int r = MutationRatioPool[Random.Range(0, MutationRatioPool.Count)];

    

           if(r == 0)
        {
            Debug.Log("mutated by disabling/reenabling connection");
            source[id].Enabled = !source[id].Enabled;
        }
           //Add new connection
        else if(r == 2)
        {
            //change weight

            float changeAmount = 0;
            for (int i = 0; i < 4; i++)
            {
                changeAmount += Random.Range(-0.4f, .4f);
            }
            source[id].Weight += changeAmount;
            //   Debug.Log("mutated Weight");
            if (source[id].Weight > 2)
            {
                source[id].Weight = 2;
            }
            if (source[id].Weight < -2f)
            {
                source[id].Weight = -2f;
            }
            Debug.Log("mutated by changing weight [GUASSIAN]");

        }
        else if (r == 3)
        {
            //change weight

         
            source[id].Weight = Random.Range(-2f,2f);
       
            Debug.Log("mutated by changing weight [RANDOM]");

        }
        else if (r == 1)
        {
            Debug.Log("mutated by adding new node ");

            //   Debug.Log("added a node");
            source[id].Enabled = false;

            //early connection
            ConnectionGene earlyConnection = new ConnectionGene();
            ConnectionGene lateConnection = new ConnectionGene();

            earlyConnection.Enabled = true;
            lateConnection.Enabled = true;
            lateConnection.Weight = source[id].Weight;
            earlyConnection.Weight = 1;

            earlyConnection.InNode = source[id].InNode;

            int newNodeID = GetNextNodeID(source);
            earlyConnection.OutNode = newNodeID;

            lateConnection.InNode = newNodeID;
            lateConnection.OutNode = source[id].OutNode;


            earlyConnection.InnovationID = GetNextInnovationID(source);
            source.Add(earlyConnection);
            lateConnection.InnovationID = GetNextInnovationID(source);

            source.Add(lateConnection);

           
        }


        return source;

    }

    public   ConnectionGene CopyConnectionGene(ConnectionGene source)
    {
        ConnectionGene newGene = new ConnectionGene();
        newGene.InNode = source.InNode;
        newGene.OutNode = source.OutNode;
        newGene.Weight = source.Weight;
        newGene.Enabled = source.Enabled;
        newGene.InnovationID = source.InnovationID;
        return newGene;
    }

    public int GetNextInnovationID(List<ConnectionGene> source)
    {
        int id = 0;

        if (HighestGlobalInnovationID == 0)
        {
            foreach (ConnectionGene connectionGene in source)
            {
                if (connectionGene.InnovationID > id)
                {
                    id = connectionGene.InnovationID;
                }
            }
            id++;
            HighestGlobalInnovationID = id;
        }
        else
        {
            HighestGlobalInnovationID++;
            id = HighestGlobalInnovationID;
        }
        return id;
    }
    public int GetNextNodeID(List<ConnectionGene> source)
    {
        int id = 0;
        if (HighestGlobalNeuronID == 0)
        {

            foreach (ConnectionGene connectionGene in source)
            {
                if (connectionGene.OutNode > id)
                {
                    id = connectionGene.OutNode;
                }
            }
            id++;
            HighestGlobalNeuronID = id;
        }
        else
        {
            HighestGlobalNeuronID++;
            id = HighestGlobalNeuronID;
        }
      

        return id;
    }
    public bool GenomeContainsInnovationID(int ID, List<ConnectionGene> source)
    {
        bool returnBool = false;

        foreach (ConnectionGene connectionGene in source)
        {
            if (connectionGene.InnovationID == ID)
            {
                returnBool = true;
            }
        }
        return returnBool;
    }

    public int IndexOfInnovationID(int ID, List<ConnectionGene> source)
    {
        int index = 0;

      for(int i = 0; i < source.Count; i++)
        {
            if(source[i].InnovationID == ID)
            {
                index = i;
            }
        }
        return index;
    }



    public void SeperatePopulationIntoSpecies(List<NeuralNetwork> population)
    {
          //CurrentSpecies.Clear();

        foreach (Species curSpec in CurrentSpecies)
        {
            curSpec.Members.Clear();
        }
        int testCounter = 0;

        foreach (NeuralNetwork neuralNet in population )
        {
            bool foundSpecies = false;

            foreach(Species curSpec in CurrentSpecies)
            {
                if(CompareGenotypeCompatibility(neuralNet, curSpec.Representitive) < SpeciesSimiliarityThreshold && !foundSpecies)
                {
                    curSpec.Members.Add(neuralNet);
                    testCounter++;
                    neuralNet.gameObject.GetComponent<BirdController>().SpeciesID = curSpec.SpeciesID;
                    foundSpecies = true;
                    neuralNet.SpeciesID = curSpec.SpeciesID;
                }
            }

           
            if(!foundSpecies)
            {
                Species species = new Species();
                SpeciesCounter++;
                species.SpeciesID = SpeciesCounter;

                List<ConnectionGene> repList = new List<ConnectionGene>();
                foreach(ConnectionGene connectionGene in neuralNet.Connections)
                {
                   repList.Add(CopyConnectionGene(connectionGene));
                }
                species.Representitive = repList;
                neuralNet.gameObject.GetComponent<BirdController>().SpeciesID = species.SpeciesID;
                Random.seed = System.DateTime.Now.Millisecond;

                species.MyColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1.0f);

                species.Members.Add(neuralNet);
                testCounter++;
                neuralNet.SpeciesID = species.SpeciesID;



                for(int i = 0; i < GameObject.Find("_SimulationManager").GetComponent<SimulationManager>().CurrentGeneration; i++)
                {
                    species.FitnessPerGeneration.Add(0);
                    species.MemberCountPerGeneration.Add(0);
                }

                CurrentSpecies.Add(species);



            }

            
        }
        Debug.Log("argh: + " + testCounter.ToString());
        //     Debug.Log("Number Of Species: " + CurrentSpecies.Count);

     //   Debug.Log(".................");

        foreach (Species curSpec in CurrentSpecies)
        {
       //     Debug.Log(curSpec.Members.Count);
        }
         //   Debug.Log(".................");

    }

    public float CompareGenotypeCompatibility(NeuralNetwork a, NeuralNetwork b)
    {

        float highestNoOfConnections = a.Connections.Count;
        if(a.Connections.Count < b.Connections.Count)
        {
            highestNoOfConnections = b.Connections.Count;
        }

        float score = 0;


        float noOfSharedConnections = 0;
        float noOfDisjointedConnections = 0;
        float noOfExcessConnections = 0;
        float weightDifferenceOfSharedConnections = 0;

        int highestSharedInnovationID = 0;
        int highestInnovationID = 0;

        bool parentAWasLastInnovator = false;

        foreach (ConnectionGene connectionGene in a.Connections)
        {
            if (connectionGene.InnovationID > highestInnovationID)
            {
                highestInnovationID = connectionGene.InnovationID;
            }
        }
        foreach (ConnectionGene connectionGene in b.Connections)
        {
            if (connectionGene.InnovationID > highestInnovationID)
            {
                highestInnovationID = connectionGene.InnovationID;
            }
        }


        for (int i = 0; i < highestInnovationID + 1; i++)
        {

            if (GenomeContainsInnovationID(i, a.Connections) && GenomeContainsInnovationID(i, b.Connections))
            {
                highestSharedInnovationID = i;
                noOfSharedConnections++;
             //  Debug.Log("found shared connections");


                float weightDiff = Mathf.Abs(a.Connections[IndexOfInnovationID(i, a.Connections)].Weight - b.Connections[IndexOfInnovationID(i, b.Connections)].Weight);

             //   Debug.Log("Parent A Weight: " + a.Connections[IndexOfInnovationID(i, a.Connections)].Weight + " " + "Parent B Weight: " + b.Connections[IndexOfInnovationID(i, b.Connections)].Weight + " Diff: " + weightDiff);

                weightDifferenceOfSharedConnections += weightDiff;

            }
            else if (GenomeContainsInnovationID(i, a.Connections))
            {
            //    Debug.Log("found disjoint connections");

                noOfDisjointedConnections++;
                parentAWasLastInnovator = true;


            }
            else if (GenomeContainsInnovationID(i, b.Connections))
            {
            //    Debug.Log("found disjoint connections");

                noOfDisjointedConnections++;

                parentAWasLastInnovator = false;
            }
            else
            {
                // parentAWasLastInnovator = false;

            }



        }
        for (int i = highestSharedInnovationID; i < highestInnovationID + 1; i++)
        {
            if(GenomeContainsInnovationID(i, a.Connections) && !GenomeContainsInnovationID(i, b.Connections))
            {
             //   Debug.Log("found excess connections");
                noOfExcessConnections++;
            }
            else if (!GenomeContainsInnovationID(i, a.Connections) && GenomeContainsInnovationID(i, b.Connections))
            {
               // Debug.Log("found excess connections");

                noOfExcessConnections++;

            }
        }
      //  Debug.Log("Weight diff:" + weightDifferenceOfSharedConnections);

        noOfDisjointedConnections -= noOfExcessConnections;

        //"if the size of the genome is small ie < 20, N ( size of biggest genome, can be set to 1)"
        if (highestNoOfConnections < GenotypeComparisonBiggestGenomeThreshold)
        {
            highestNoOfConnections = 1;
        }
        score += (noOfDisjointedConnections * SpeciationDisjointedCoEff / highestNoOfConnections);
        score += (noOfExcessConnections * SpeciationExcessCoEff / highestNoOfConnections);
        score += weightDifferenceOfSharedConnections * SpeciationWeightCoEff;


            return score;
    }

    public float CompareGenotypeCompatibility(NeuralNetwork a, List<ConnectionGene> b)
    {

        float highestNoOfConnections = a.Connections.Count;
        if (a.Connections.Count < b.Count)
        {
            highestNoOfConnections = b.Count;
        }

        float score = 0;


        float noOfSharedConnections = 0;
        float noOfDisjointedConnections = 0;
        float noOfExcessConnections = 0;
        float weightDifferenceOfSharedConnections = 0;

        int highestSharedInnovationID = 0;
        int highestInnovationID = 0;

        bool parentAWasLastInnovator = false;

        foreach (ConnectionGene connectionGene in a.Connections)
        {
            if (connectionGene.InnovationID > highestInnovationID)
            {
                highestInnovationID = connectionGene.InnovationID;
            }
        }
        foreach (ConnectionGene connectionGene in b)
        {
            if (connectionGene.InnovationID > highestInnovationID)
            {
                highestInnovationID = connectionGene.InnovationID;
            }
        }


        for (int i = 0; i < highestInnovationID + 1; i++)
        {

            if (GenomeContainsInnovationID(i, a.Connections) && GenomeContainsInnovationID(i, b))
            {
                highestSharedInnovationID = i;
                noOfSharedConnections++;
                //  Debug.Log("found shared connections");


                float weightDiff = Mathf.Abs(a.Connections[IndexOfInnovationID(i, a.Connections)].Weight - b[IndexOfInnovationID(i, b)].Weight);

                //   Debug.Log("Parent A Weight: " + a.Connections[IndexOfInnovationID(i, a.Connections)].Weight + " " + "Parent B Weight: " + b.Connections[IndexOfInnovationID(i, b.Connections)].Weight + " Diff: " + weightDiff);

                weightDifferenceOfSharedConnections += weightDiff;

            }
            else if (GenomeContainsInnovationID(i, a.Connections))
            {
                //    Debug.Log("found disjoint connections");

                noOfDisjointedConnections++;
                parentAWasLastInnovator = true;


            }
            else if (GenomeContainsInnovationID(i, b))
            {
                //    Debug.Log("found disjoint connections");

                noOfDisjointedConnections++;

                parentAWasLastInnovator = false;
            }
            else
            {
                // parentAWasLastInnovator = false;

            }



        }
        for (int i = highestSharedInnovationID; i < highestInnovationID + 1; i++)
        {
            if (GenomeContainsInnovationID(i, a.Connections) && !GenomeContainsInnovationID(i, b))
            {
                //   Debug.Log("found excess connections");
                noOfExcessConnections++;
            }
            else if (!GenomeContainsInnovationID(i, a.Connections) && GenomeContainsInnovationID(i, b))
            {
                // Debug.Log("found excess connections");

                noOfExcessConnections++;

            }
        }
        //  Debug.Log("Weight diff:" + weightDifferenceOfSharedConnections);

        noOfDisjointedConnections -= noOfExcessConnections;

        //"if the size of the genome is small ie < 20, N ( size of biggest genome, can be set to 1)"
        if (highestNoOfConnections < GenotypeComparisonBiggestGenomeThreshold)
        {
            highestNoOfConnections = 1;
        }
        score += (noOfDisjointedConnections * SpeciationDisjointedCoEff / highestNoOfConnections);
        score += (noOfExcessConnections * SpeciationExcessCoEff / highestNoOfConnections);
        score += weightDifferenceOfSharedConnections * SpeciationWeightCoEff;


        return score;
    }
    public void EvaluateFitness()
    {
        foreach (Species curSpec in CurrentSpecies)
        {
            foreach (NeuralNetwork neuralNetwork in curSpec.Members)
            {
              //  neuralNetwork.Fitness = neuralNetwork.GetTotalConnectionWeight() / (neuralNetwork.GlobalNodes.Count * 1 );
                neuralNetwork.Fitness = neuralNetwork.gameObject.GetComponent<BirdController>().MyScore;
            }
        }
    }

    public void ShareFitnessAmongSpecies()
    {
        foreach(Species curSpec in CurrentSpecies)
        {
            foreach (NeuralNetwork neuralNetwork in curSpec.Members)
            {
                neuralNetwork.AdjustedFitness = neuralNetwork.Fitness / curSpec.Members.Count;
            }
        }
    }


    public void PopulateParentSpeciesRatioPool()
    {
        ParentSpeciesRatioPool.Clear();

       for(int i = 0; i < CurrentSpecies.Count; i++)
        {
            for(int n = 0; n < CurrentSpecies[i].GetTotalFitness(); n++)
            {
                ParentSpeciesRatioPool.Add(i);
            }
        }
    }

    public void CullSpecies()
    {

        foreach(Species curSpec in CurrentSpecies)
        {

            //    int amountToKeep = (curSpec.Members.Count / 100) * EliteRatio;
            int amountToKeep = (curSpec.Members.Count / 2);

          //  Debug.Log(curSpec.Members.Count);
         //   Debug.Log("AmountToKeep:" + amountToKeep);
          
            curSpec.Members = curSpec.Members.OrderBy(o => o.Fitness).ToList();
            curSpec.Members.Reverse();

            if (amountToKeep < 1)
            {
               amountToKeep = 1;
            }
            for (int i = curSpec.Members.Count - 1;  i > amountToKeep; i--)
            {
                curSpec.Members.RemoveAt(i);
            }

        }
    }


    public void CreateNewPopulation(int populationSize)
    {
      //  Debug.Log("CreatingNewBufferPopulation");
        NewPopulation.Clear();
        for(int i = 0; i < populationSize; i++)
        {
            int indexOfSpeciesToMake = ParentSpeciesRatioPool[Random.Range(0, ParentSpeciesRatioPool.Count)];
         //   Debug.Log("//////////////////");

            foreach(Species s in CurrentSpecies)
            {
             //   Debug.Log(s.Members.Count);
            }
          //  Debug.Log("//////////////////");


            NeuralNetwork newNet = new NeuralNetwork();

           
            NewPopulation.Add(newNet);
            bool asexual = false;
            NeuralNetwork parentA = CurrentSpecies[indexOfSpeciesToMake].Members[Random.Range(0, CurrentSpecies[indexOfSpeciesToMake].Members.Count)];
            NeuralNetwork parentB = new NeuralNetwork();
            newNet.SpeciesID = parentA.SpeciesID;
            if (CurrentSpecies[indexOfSpeciesToMake].Members.Count > 1)
            {
                bool foundOtherParent = false;

                while(!foundOtherParent)
                {
                    parentB = CurrentSpecies[indexOfSpeciesToMake].Members[Random.Range(0, CurrentSpecies[indexOfSpeciesToMake].Members.Count)];

                    if (parentB != parentA)
                    {
                        foundOtherParent = true;
                    }
                }
            //    Debug.Log("Sexual");
             //   Debug.Log("SpeciesMemberCount: " + CurrentSpecies[indexOfSpeciesToMake].Members.Count);
            }
            else
            {
                asexual = true;
              //  Debug.Log("Asexual");
              //  Debug.Log("SpeciesMemberCount: " + CurrentSpecies[indexOfSpeciesToMake].Members.Count);
               parentB = CurrentSpecies[indexOfSpeciesToMake].Members[Random.Range(0, CurrentSpecies[indexOfSpeciesToMake].Members.Count)];
            }

            //   Debug.Log("ParentA Count " + parentA.Connections.Count);
            //  Debug.Log("ParentB Count " + parentB.Connections.Count);

            bool addedInnov0 = false;

            float modifiedMutationRate = MutateRate ;
            if(asexual)
            {
                modifiedMutationRate = MutateRate * 2;
            }
            foreach (ConnectionGene connectionGene in CreateChildFromParents(parentA, parentB, modifiedMutationRate))
                {
                if (connectionGene.InnovationID != 0)
                {
                    newNet.Connections.Add(CopyConnectionGene(connectionGene));
                }
                else
                {
                    if(addedInnov0 == false)
                    {
                        newNet.Connections.Add(CopyConnectionGene(connectionGene));
                        addedInnov0 = true;

                    }
                }
                }

         //   Debug.Log("TestCount " + newNet.Count);
        }

       // Debug.Log("NewPopulationBufferCount = " + NewPopulation.Count);
    }

}
