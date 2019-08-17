using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPeek : MonoBehaviour
{
  public KeyCode key;

  public float angle = 45;
  public float speed = 5;

    // Update is called once per frame
    void Update() {
        var target = Input.GetKey(key) ? Quaternion.AngleAxis(angle, Vector3.right) : Quaternion.identity;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speed * Time.deltaTime);
    }
}
