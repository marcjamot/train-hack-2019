using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavingArea : MonoBehaviour
{

    private GameState gameState;
    private LevelInformation levelInformation;
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.Find("GameProgression").GetComponent<GameState>();
        levelInformation = transform.parent.GetComponent<LevelInformation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (levelInformation.IsActive)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            other.gameObject.SetActive(false);
            gameState.LivesLeft--;
            if (gameState.LivesLeft == 0)
            {
                //You Lose;
            }
        }
    }

}
