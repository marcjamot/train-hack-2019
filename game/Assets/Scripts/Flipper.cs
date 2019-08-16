using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{

    private readonly float EPSILON = 0.01f;
    public float RotationSpeed = 10f;
    public float StartRotation = 60;

    public string InputName;

    private Quaternion RotationVector;

    // Start is called before the first frame update
    void Start()
    {
        RotationVector = Quaternion.Euler(0, Time.deltaTime * RotationSpeed, 0);
        Quaternion startRotationVector = transform.rotation;
        transform.rotation.Set(startRotationVector.x, startRotationVector.y + StartRotation, startRotationVector.z, startRotationVector.w);
    }

    // Update is called once per frame
    void Update()
    {
        if (System.Math.Abs(Input.GetAxis(InputName) - 1) < EPSILON)
        {
            //transform.Rotate(RotationAxisVector, Space.World);
            transform.rotation *= RotationVector;
        }
        else
        {
            transform.Rotate(0, 0, 0, Space.World);
        }

        Debug.Log(transform.position);
    }

}
