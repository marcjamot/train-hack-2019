using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Press : MonoBehaviour
{
    
    

    float _current_amount;

    public KeyCode key = KeyCode.None;
    public float rest = 110.0f;
    public float reach = -50.0f;
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(key)) {
            _current_amount = Mathf.Min(1, _current_amount + speed * Time.deltaTime);
        } else {
            _current_amount = Mathf.Max(0, _current_amount - speed * Time.deltaTime);
        }
        var angle = Mathf.Lerp(rest, rest + reach, _current_amount);
        var euler = transform.localEulerAngles;
        // Debug.Log(euler.y);
        transform.localEulerAngles = new Vector3(euler.x, angle, euler.z);
    }
}
