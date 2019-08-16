using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlocker : MonoBehaviour
{
    public float BottomPosition;
    public float MoveForce;

    void Update()
    {
        if (transform.position.y > BottomPosition)
        {
            transform.position += Vector3.down * MoveForce * Time.deltaTime;
        }
    }
}
