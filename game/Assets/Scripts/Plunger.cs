using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plunger : MonoBehaviour
{
    float Power;
    public float MaxPower;
    public float PowerDelta;
    List<Rigidbody> BallList = new List<Rigidbody>();
    bool BallReady;
    public KeyCode FirstLaunchKey;

    float angle;

    // Start is called before the first frame update
    void Start()
    {
        angle = Random.Range(-45, 45);
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
                    ball.AddForce(Power * Vector3.forward);// (Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward));
                }
            }
        }
        else
        {
            Power = 0;
        }

        if (Input.GetKey(KeyCode.R) && ballCache != null)
        {
            ballCache.transform.position = new Vector3(0, -0.089688f, -0.95f);
            ballCache.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ballCache.SetActive(true);
        }
    }

    GameObject ballCache;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.gameObject.CompareTag("Ball"))
        {
            ballCache = other.gameObject;
            Debug.Log(other, other.gameObject.GetComponent<Rigidbody>());
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
