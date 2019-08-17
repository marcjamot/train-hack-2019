using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceloratorMadness9000 : MonoBehaviour
{

    List<Rigidbody> BallList = new List<Rigidbody>();
    public float AcceleratorForce;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (BallList.Count > 0)
        {
            //Accelerate the ball forward
            foreach (Rigidbody ball in BallList)
            {
                ball.AddForce(AcceleratorForce * Vector3.forward);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            BallList.Add(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            BallList.Remove(other.gameObject.GetComponent<Rigidbody>());
        }
    }
}
