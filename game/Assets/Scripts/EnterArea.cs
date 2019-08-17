using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnterArea : MonoBehaviour
{
    private GameState gameState;
    private LevelInformation levelInformation;
    private bool isActive;

    private Dictionary<int, AudioSource> levelSound;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.Find("GameProgression").GetComponent<GameState>();
        
        Debug.Log(gameState);
        levelInformation = transform.parent.GetComponent<LevelInformation>();

        string[] levelSoundNames = Directory.GetFiles(".");
        Debug.Log($"Sounds: {levelSoundNames}");
        //Resources.Load<AudioSource>("Sound/")
    }

    // Update is called once per frame
    void Update()
    {
        if(levelInformation.IsActive) 
        {
            isActive = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(!isActive) return;
        
        if (other.gameObject.CompareTag("Ball"))
        {
            gameState.CurrentStopIndex++;
            if (gameState.CurrentStopIndex != gameState.Stops?.Count)
            {
                gameState.NextArrival = gameState.Stops[gameState.CurrentStopIndex + 1].ArrivalTime;
                gameState.NextStation = gameState.Stops[gameState.CurrentStopIndex + 1].Name;
            }

            levelInformation.IsActive = true;

            gameObject.SetActive(false);

        }
    }
}
