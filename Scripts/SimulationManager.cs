using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{

    public GeneticEvolutionManager MyGEM;

    public  NeuralNetwork MyNN;

    public NeuralNetwork TestNN;


    public NeuralNetwork TestNN2;


    public GameObject Prefab;
    public GameManager MyGameManager;

    public int PopulationSize = 100;
    public int NoOfGenerationsToSimulate = 20;

    public int CurrentGeneration = 0;
    public List<NeuralNetwork> Population = new List<NeuralNetwork>();


    public bool SimActive = false;
    public int NoOfAliveBots;


    public bool testBool = false;

    public string TimeStamp;

    public List<float> averagePopFitness = new List<float>();

    // Start is called before the first frame update
    void Start()
    {

        TimeStamp = DateTime.Now.Day.ToString() + DateTime.Now.Month + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second ;
        Application.runInBackground = true;
        Time.timeScale = 1f;
        //bots not working on beat
        //INIT
        for(int i = 0; i < PopulationSize; i ++)
        {
            GameObject go = (GameObject)Instantiate(Prefab, new Vector3(0, 0, 0),Quaternion.identity);
            NeuralNetwork neuralNet = go.AddComponent<NeuralNetwork>();
            neuralNet.CreateEmptyNeuralNetwork(3, 1, 0);
            Population.Add(neuralNet);
        }

        NoOfAliveBots = PopulationSize;

        //LOOP

        //for (int gen = 0; gen < NoOfGenerationsToSimulate; gen++)
        //{

        //    //MyGameManager.ResetGame(Population);

        //    float bestFitness = -Mathf.Infinity; ;

        //    MyGEM.SeperatePopulationIntoSpecies(Population);
        //    MyGEM.EvaluateFitness();

        //    foreach (NeuralNetwork neuralNet in Population)
        //    {
        //        if (neuralNet.Fitness > bestFitness)
        //        {
        //            bestFitness = neuralNet.Fitness;
        //        }
        //    }
        //    Debug.Log("Generation " + gen);
        //    Debug.Log("Fitness " + bestFitness);
        //    MyGEM.CullSpecies();
        //    MyGEM.ShareFitnessAmongSpecies();
        //    MyGEM.PopulateParentSpeciesRatioPool();

        //    MyGEM.CreateNewPopulation(PopulationSize);

        //    for (int i = 0; i < Population.Count; i++)
        //    {
        //        // Debug.Log(MyGEM.NewPopulation[i].Count);
        //        Population[i].CreateNeuralNetworkFromGenome(MyGEM.NewPopulation[i]);
        //    }

        //    foreach (NeuralNetwork neuralNet in Population)
        //    {
        //        foreach (NodeGene inputNode in neuralNet.InputNodes)
        //        {
        //            inputNode.CurrentValue = Random.Range(-10f, 10f);
        //        }
        //        neuralNet.Tick();
        //    }
        //}
        SimActive = true;

        //END LOOP
        //MyNN.CreateEmptyNeuralNetwork(3, 2, 0);


        //TestNN.CreateEmptyNeuralNetwork(3, 2, 0);


        //float wb = MyNN.GetTotalConnectionWeight();
        //Debug.Log("Compatibility: " + MyGEM.CompareGenotypeCompatibility(MyNN, TestNN));

        //float pbwb = TestNN.GetTotalConnectionWeight();

        //TestNN2.CreateEmptyNeuralNetwork(3, 2, 0);

        // TestNN2.CreateNeuralNetworkFromGenome(MyGEM.CreateChildFromParents(MyNN, TestNN));

        //Debug.Log("Parent A Weight Before: " + wb);

        //Debug.Log("Parent A Weight After: " + MyNN.GetTotalConnectionWeight());
        //Debug.Log("Parent B Weight Before: " + pbwb);

        //Debug.Log("Parent B Weight After: " + TestNN.GetTotalConnectionWeight());

        //Debug.Log("Compatibility: " + MyGEM.CompareGenotypeCompatibility(MyNN, TestNN2));


        //   Debug.Log("Compatibility: " + MyGEM.CompareGenotypeCompatibility(MyNN, MyNN));


        //  Debug.Log("Compatibility: " + MyGEM.CompareGenotypeCompatibility(MyNN, TestNN2));


    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextGeneration()
    {

    }

    void FixedUpdate()
    {
        if(SimActive)
        {
            foreach (NeuralNetwork neuralNet in Population)
            {
                if(testBool = true)
                {
                 //   Debug.Log("fdafa");
                }
                if (neuralNet.InputNodes.Count > 0)
                {
                    neuralNet.InputNodes[0].CurrentValue = neuralNet.gameObject.transform.position.y;
                }

                if (neuralNet.InputNodes.Count >1)
                {
                    neuralNet.InputNodes[1].CurrentValue = MyGameManager.NextTunnelY;
                }

                if (neuralNet.InputNodes.Count >2)
                {
                    neuralNet.InputNodes[2].CurrentValue = MyGameManager.NextTunnelX;
                }


                neuralNet.Tick();
                if (neuralNet.OutputNodes.Count > 0)

                {
                    if (neuralNet.OutputNodes[0].CurrentValue > 0)
                    {
                       
                        neuralNet.GetComponent<BirdController>().Jump();
                    }
                }
            }

            if (NoOfAliveBots == 0 && CurrentGeneration < NoOfGenerationsToSimulate)
            {
                NoOfAliveBots = PopulationSize;

                SimActive = false;
                NextGeneration();
                Debug.Log("what");

                float bestFitness = -Mathf.Infinity; ;

                MyGEM.SeperatePopulationIntoSpecies(Population);
                MyGEM.EvaluateFitness();

                var totalFit = 0f;
                foreach (NeuralNetwork neuralNet in Population)
                {
                    totalFit += neuralNet.Fitness;
                }
                totalFit /= Population.Count;
                averagePopFitness.Add(totalFit);


                foreach (NeuralNetwork neuralNet in Population)
                {
                    if (neuralNet.Fitness > bestFitness)
                    {
                        bestFitness = neuralNet.Fitness;
                    }
                }
                CurrentGeneration++;
                Debug.Log("Generation " + CurrentGeneration);
                Debug.Log("Fitness " + bestFitness);

                //Take metrics here?
                int sanityTotalPopInSpecsCount = 0;
                int actualCurSpecCount = 0;

                string path = "C:\\FlappyLogs\\" + TimeStamp + ".csv";
                StreamWriter sw = File.AppendText(path);

                foreach (Species spec in MyGEM.CurrentSpecies)
                {
                    float f = 0;
                    if (spec.Members.Count > 0)
                    {
                        foreach (NeuralNetwork neuralNetwork in spec.Members)
                        {
                            f += neuralNetwork.Fitness;
                        }
                        f /= spec.Members.Count;
                    }
                    spec.MemberCountPerGeneration.Add(spec.Members.Count);
                    spec.FitnessPerGeneration.Add(f);

                    Debug.Log("...SPECIES...");
                    Debug.Log("SpeciesID: " + spec.SpeciesID);
                    Debug.Log("MemberCount:" + spec.Members.Count.ToString());
                    sanityTotalPopInSpecsCount += spec.Members.Count;
                    if (spec.Members.Count > 0)
                    {
                        actualCurSpecCount++;
                    }
                    try
                    {
                  
                        //Pass the filepath and filename to the StreamWriter Constructor
                        //Write a line of text
                        string line = spec.SpeciesID.ToString() + "," + spec.Members.Count.ToString() + "," + f.ToString();
                        sw.WriteLine(line);
                    
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Exception: " + e.Message);
                    }
                    finally
                    {
                        Debug.Log("Executing finally block.");
                    }
                }
                sw.WriteLine("#");
                sw.Close();



                Debug.Log("SanityCheck Active Spec Count :" + actualCurSpecCount.ToString());

                Debug.Log("SanityCheck Total Pop in Species Count :" + sanityTotalPopInSpecsCount.ToString());
                //
                MyGEM.CullSpecies();
                MyGEM.ShareFitnessAmongSpecies();
                MyGEM.PopulateParentSpeciesRatioPool();

                MyGEM.CreateNewPopulation(PopulationSize);

                for (int i = 0; i < Population.Count; i++)
                {
                    // Debug.Log(MyGEM.NewPopulation[i].Count);
                    Population[i].CreateNeuralNetworkFromGenome(MyGEM.NewPopulation[i].Connections);
                    Population[i].SpeciesID = MyGEM.NewPopulation[i].SpeciesID;
                }
                MyGameManager.ResetGame(Population);
                SimActive = true;
                //   testBool = true;


              //  MyGEM.SeperatePopulationIntoSpecies(Population);
                foreach(Species spec in MyGEM.CurrentSpecies)
                {

                    foreach(NeuralNetwork neuralNet in spec.Members)
                    {
                        neuralNet.gameObject.GetComponent<SpriteRenderer>().color = spec.MyColor;
                    }
                }



                foreach (NeuralNetwork neuralNet in Population)
                {
                    neuralNet.GetComponent<BirdController>().StillInSim = true;

                    neuralNet.gameObject.GetComponent<BirdController>().SpeciesID = neuralNet.SpeciesID;

                    foreach(Species spec in MyGEM.CurrentSpecies)
                    {
                        if(spec.SpeciesID == neuralNet.SpeciesID)
                        {
                            neuralNet.gameObject.GetComponent<SpriteRenderer>().color = spec.MyColor;
                        }
                    }
                }


        
            }

            if(CurrentGeneration == NoOfGenerationsToSimulate)
            {
                CurrentGeneration++;

                //member count
                Debug.Log("Finished");
                string path = "C:\\FlappyLogs\\" +"MEMBERSCOUNT" +  TimeStamp + ".csv";
                StreamWriter sw = File.AppendText(path);

                for (int i = 0; i < NoOfGenerationsToSimulate; i++)
                {
                    string line = "";

                    foreach (Species spec in MyGEM.CurrentSpecies)
                    {


                        if (spec.MemberCountPerGeneration.Count > i)
                        {
                            line = line + spec.MemberCountPerGeneration[i].ToString() + ",";
                        }  

                       
                      
                    }
                    sw.WriteLine(line);

                }
                sw.Close();

                //fitness log
                string fitnessPath = "C:\\FlappyLogs\\" + "FITNESS" + TimeStamp + ".csv";
                StreamWriter fsw = File.AppendText(fitnessPath);

                for (int i = 0; i < NoOfGenerationsToSimulate; i++)
                {
                    string line = "";

                    foreach (Species spec in MyGEM.CurrentSpecies)
                    {


                        if (spec.MemberCountPerGeneration.Count > i)
                        {
                            line = line + spec.FitnessPerGeneration[i].ToString() + ",";
                        }



                    }

                
                 
                        line = line + averagePopFitness[i].ToString() + ",";
                 
                
                    fsw.WriteLine(line);

                }
                fsw.Close();

            }

        }

     
         

        
    }
    
}
