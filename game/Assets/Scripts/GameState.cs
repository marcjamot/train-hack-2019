using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int LivesLeft;
    public int Score;
    public int CurrentStationIndex;
    public string NextStation;
    public System.DateTime NextArrival;
    public ICollection<StationInformation> Stations;



    // Start is called before the first frame update
    void Start()
    {
        LivesLeft = 3;
        Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
