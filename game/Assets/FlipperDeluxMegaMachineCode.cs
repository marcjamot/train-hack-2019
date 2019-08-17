using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipperDeluxMegaMachineCode : MonoBehaviour
{
  private HingeJoint hinge;
  public float pressed = 45;
  private const float kRest = 0;

    public KeyCode key;

  // Start is called before the first frame update
  void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // Update is called once per frame
    void Update()
    {
        var spring = new JointSpring();
        spring.spring = 20000f;
        spring.damper = 150f;
        spring.targetPosition = Input.GetKey(key) ? pressed : kRest;
        hinge.spring = spring;
        hinge.useLimits = true;
    }
}
