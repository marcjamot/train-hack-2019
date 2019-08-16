using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plunger : MonoBehaviour
{
    float Power;
    public float MaxPower;
    public float PowerDelta;
    List<Rigidbody> BallList;
    bool BallReady;
    public KeyCode FirstLaunchKey;

    // Start is called before the first frame update
    void Start()
    {
        BallList = new List<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        BallReady = BallList.Count > 0;
        if (BallReady)
        {
            if (Input.GetKey(FirstLaunchKey) && Power <= MaxPower)
            {
                Debug.Log("ADDING POWER");
                Power += PowerDelta * Time.deltaTime;
            }
            if (Input.GetKeyUp(FirstLaunchKey))
            {
                Debug.Log("LETTING GO");
                foreach (Rigidbody ball in BallList)
                {
                    ball.AddForce(Power * Vector3.forward);
                }
            }
        }
        else
        {
            Power = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.gameObject.CompareTag("Ball"))
        {
            BallList.Add(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        if (other.gameObject.CompareTag("Ball"))
        {
            BallList.Remove(other.gameObject.GetComponent<Rigidbody>());
            Power = 0;
        }
    }
}
