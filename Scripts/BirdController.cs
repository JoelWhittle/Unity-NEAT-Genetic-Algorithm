using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{

    public Rigidbody2D rb;

    public float FlapStrength = 200;

    public float TimeBetweenFlaps = 1.0f;
    public float CurTimeBetweenFlaps = 1.0f;

    public bool StillInSim = true;
    public float MyScore = 0;



    public int SpeciesID = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        CurTimeBetweenFlaps = TimeBetweenFlaps;
    }

    // Update is called once per frame
    void Update()
    {
        CurTimeBetweenFlaps -= Time.deltaTime;

        if(Input.GetKeyUp(KeyCode.Space))
        {
            if (CurTimeBetweenFlaps < 0)
            {
                rb.velocity = new Vector2(0, 0);

                rb.AddForce(transform.up * FlapStrength);
                CurTimeBetweenFlaps = TimeBetweenFlaps;
            }
        }


        if (rb.velocity.y > 100)
        {
            rb.velocity = new Vector2(0, 50);
        }


        if(gameObject.transform.position.y > 10  || gameObject.transform.position.y < -7)
        {
            if (StillInSim)
            {
             //   Debug.Log("died to out of bounds");
                StillInSim = false;
                MyScore = GameObject.Find("_GameManager").GetComponent<GameManager>().Score;
              //  MyScore *= GameObject.Find("_GameManager").GetComponent<GameManager>().TunnelsMoved ;
                GameObject.Find("_SimulationManager").GetComponent<SimulationManager>().NoOfAliveBots--;
            }
        }


        if (gameObject.GetComponent<NeuralNetwork>().InputNodes.Count < 3 || gameObject.GetComponent<NeuralNetwork>().OutputNodes.Count < 1)
        {
            if (StillInSim)
            {
                Debug.Log("aaarghghg invalid child detected", gameObject);

                StillInSim = false;

                MyScore = 0;
                GameObject.Find("_SimulationManager").GetComponent<SimulationManager>().NoOfAliveBots--;

            }
        }


        float r = 0;
        float aValue;
        float normal = Mathf.InverseLerp(-20, 20, rb.velocity.y);
        float bValue = Mathf.Lerp(-90, 45, normal);

        gameObject.transform.rotation = Quaternion.Euler(0, 0, bValue);
    }

    public void Jump()
    {
        if (CurTimeBetweenFlaps < 0 && StillInSim)
        {
            rb.velocity = new Vector2(0, 0);

            rb.AddForce(Vector2.up * FlapStrength);
            CurTimeBetweenFlaps = TimeBetweenFlaps;

         //   Debug.Log("jump");
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
      
        if (StillInSim)
        {
         //   Debug.Log("died to tunnel");
            StillInSim = false;
            MyScore = GameObject.Find("_GameManager").GetComponent<GameManager>().Score;
            GameObject.Find("_SimulationManager").GetComponent<SimulationManager>().NoOfAliveBots--;
          //  MyScore *= GameObject.Find("_GameManager").GetComponent<GameManager>().TunnelsMoved;


        }
    }



    
}
