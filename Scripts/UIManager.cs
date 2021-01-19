using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameManager MyGameManager;
    public SimulationManager MySimulationManager;
    public GeneticEvolutionManager MyGEM;

    public Text ScoreText;
    public Text GenerationText;
    public Text SpeciesCountText;
    public Text StillAliveCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ScoreText.text = "Score: "+  MyGameManager.Score.ToString();
        GenerationText.text = "Generation: " + MySimulationManager.CurrentGeneration.ToString();
        StillAliveCount.text = "Alive Count: " + MySimulationManager.NoOfAliveBots.ToString();

        SpeciesCountText.text = "Species Count: " + MyGEM.CurrentSpecies.Count.ToString();

    }
}
