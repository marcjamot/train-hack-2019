using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{

    private readonly float EPSILON = 0.01f;
    private readonly float ROTATION_EPSILON = 1f;
    private readonly float BUFFER = 3f;
    public float RotationSpeed = 10f;
    public float StartRotation = 60;
    public float EndRotation = 0;

    public bool IsLeft;

    public GameObject RotationObject;

    public string InputName;

    private Vector3 StartRotationVector;

    // Start is called before the first frame update
    void Start()
    {
        StartRotationVector = transform.localEulerAngles;
        transform.localEulerAngles.Set(StartRotationVector.x, StartRotation, StartRotationVector.z);
    }

    // Update is called once per frame
    void Update()
    {
        float yAngle = transform.localEulerAngles.y;
        float angle = (IsLeft ? -1 : 1) * RotationSpeed * Time.deltaTime;
        Debug.Log(yAngle);

        bool shouldRotate = (StartRotation - BUFFER) <= yAngle && yAngle <= (EndRotation + BUFFER);
        bool keyPressed = System.Math.Abs(Input.GetAxis(InputName) - 1) < EPSILON;

        if (keyPressed && shouldRotate)
        {
            transform.RotateAround(RotationObject.transform.position, transform.up, angle);
        }
        else if (!keyPressed)
        {
            angle *= -1;
            bool IsInNormalState = System.Math.Abs(yAngle - StartRotation) < ROTATION_EPSILON;
            if (!IsInNormalState)
            {
                // Rotate back to original position
                transform.RotateAround(RotationObject.transform.position, transform.up, angle);
            }
        }

    }

}
