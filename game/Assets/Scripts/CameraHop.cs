using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHop : MonoBehaviour
{
    public int level2 = 0;

    IList<(float, Vector3, float)> targets = new List<(float, Vector3, float)> {
        (0f, Vector3.zero, 0),
        (0.20f, new Vector3(0, 0.35f, 0.4f), -20),
        (0.50f, new Vector3(0, 0.55f, 0.7f), -20),
        (0.75f, new Vector3(0, 0.30f, 1.5f), -15),
        (1f, 2 * Vector3.forward, 0),
    };
ShouldGoToNextLevel backref;
  internal void startMeAgain(ShouldGoToNextLevel shouldGoToNextLevel)
  {
    backref = shouldGoToNextLevel;
  }

  const float kMaxTime = 1.5f;

    const float kCameraOffset = 45f;

    public float speed = 1f;
  public bool _transitioning;
  private float _transitionStart;

  // Start is called before the first frame update
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var progress = _transitioning ? (Time.time - _transitionStart) : 0;
        if (progress >= kMaxTime) {
          ++level2;
          _transitioning = false;
          progress = 0;
      backref.StartBallAgainPleease();
        }
        Debug.Log("PROGRESS " + progress);
        var (position, angle) = Next(progress);
        transform.position = position + 2 * Vector3.forward * level2;
        transform.localEulerAngles = new Vector3(angle, 0, 0);
    }

  internal void Hop()
  {
    _transitioning = true;
    _transitionStart = Time.time;
  }

  public (Vector3, float) Next(float t) {
        var prevTarget = targets[0];
        foreach (var target in targets) {
            var (time, position, angle) = target;
            if (time <= t) {
                prevTarget = target;
                continue;
            }
            var (prevTime, prevPosition, prevAngle) = prevTarget;
            var amount = (t - prevTime) / (time - prevTime);
            return (
                Vector3.Lerp(prevPosition, position, amount),
                Mathf.Lerp(prevAngle, angle, amount)
            );
        }
        var (_, lastPosition, lastAngle) = prevTarget;
        return (lastPosition, lastAngle);
    }
}
