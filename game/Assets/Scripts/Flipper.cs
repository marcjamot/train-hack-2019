using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{

    private readonly float EPSILON = 0.01f;
    private readonly float ROTATION_EPSILON = 1f;
    private readonly float BUFFER = 3f;
    public float RotationSpeed = 10f;
    public float StartRotation;
    public float EndRotation;

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

        bool shouldRotate = StartRotation <= yAngle && yAngle <= EndRotation;
        Debug.Log($"shouldRotate:{shouldRotate} ({yAngle})");

        if (!shouldRotate)
        {
            return;
        }

        bool keyPressed = System.Math.Abs(Input.GetAxis(InputName) - 1) < EPSILON;

        if (keyPressed)
        {
            transform.RotateAround(RotationObject.transform.position, transform.up, angle);
        }
        else
        {
            angle *= -1;
            transform.RotateAround(RotationObject.transform.position, transform.up, angle);
        }

    }

}
