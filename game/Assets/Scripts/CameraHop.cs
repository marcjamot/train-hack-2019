using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHop : MonoBehaviour
{
    public int level = 0;

    IList<(float, Vector3, float)> targets = new List<(float, Vector3, float)> {
        (0f, Vector3.zero, 0),
        (0.20f, new Vector3(0, 0.25f, 0.4f), -20),
        (0.50f, new Vector3(0, 0.35f, 0.7f), -20),
        (0.75f, new Vector3(0, 0.2f, 1.5f), -15),
        (1f, 2 * Vector3.forward, 0),
    };
    const float kMaxTime = 1.5f;

    const float kCameraOffset = 45f;

    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var progress = speed * Time.fixedTime;
        level = Mathf.FloorToInt(progress / kMaxTime);
        var (position, angle) = Next(progress % kMaxTime);
        transform.position = position + 2 * Vector3.forward * level;
        transform.GetChild(0).localEulerAngles = new Vector3(kCameraOffset + angle, 0, 0);
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
