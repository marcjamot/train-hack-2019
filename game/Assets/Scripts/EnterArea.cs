using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterArea : MonoBehaviour
{
    private GameState gameState;
    private LevelInformation levelInformation;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.Find("GameProgression").GetComponent<GameState>();
        levelInformation = GameObject.Find("Stop").GetComponent<LevelInformation>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Ball")) 
        {
            gameState.CurrentStopIndex++;
            if(gameState.CurrentStopIndex != gameState.Stops.Count) {
                gameState.NextArrival = gameState.Stops[gameState.CurrentStopIndex + 1].ArrivalTime;
                gameState.NextStation = gameState.Stops[gameState.CurrentStopIndex + 1].Name;
            }

            levelInformation.IsActive = true;

            gameObject.SetActive(false);
            
        }
    }
}
