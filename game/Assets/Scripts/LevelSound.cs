using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSound : MonoBehaviour
{

    private AudioSource Sound;
    // Start is called before the first frame update
    void Start()
    {
        Sound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Sound.Play();
        }
    }
}
