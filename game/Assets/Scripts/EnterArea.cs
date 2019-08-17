using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnterArea : MonoBehaviour
{
    private GameState gameState;
    private LevelInformation levelInformation;

    private Dictionary<int, AudioSource> levelSound;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.Find("GameProgression").GetComponent<GameState>();
        levelInformation = GameObject.Find("Stop").GetComponent<LevelInformation>();

        string[] levelSoundNames = Directory.GetFiles("Sound");
        Debug.Log($"Sounds: {levelSoundNames}");
        //Resources.Load<AudioSource>("Sound/")
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            gameState.CurrentStopIndex++;
            if (gameState.CurrentStopIndex != gameState.Stops.Count)
            {
                gameState.NextArrival = gameState.Stops[gameState.CurrentStopIndex + 1].ArrivalTime;
                gameState.NextStation = gameState.Stops[gameState.CurrentStopIndex + 1].Name;
            }

            levelInformation.IsActive = true;

            gameObject.SetActive(false);

        }
    }
}
