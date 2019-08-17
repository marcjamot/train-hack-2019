using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlocker : MonoBehaviour
{
    public float BottomPosition;
    private float StartPosition = 0.17f;
    public float MoveForce;
    public float TriggerTime;
    private bool CanMoveDown = true;

    void Update()
    {
        if (CanMoveDown && transform.position.y > BottomPosition)
        {
            transform.position += Vector3.down * MoveForce * Time.deltaTime;
            CanMoveDown = transform.position.y > BottomPosition;
        }

        if (!CanMoveDown)
        {
            // start a timer, when that one triggers, move the box up again
            TriggerTime -= Time.deltaTime;
            if (TriggerTime < 0f && transform.position.y <= StartPosition)
            {
                Debug.Log("Opening new level");
                transform.position += Vector3.up * MoveForce * Time.deltaTime;
            }
        }
    }
}
