using UnityEngine;

public class PopBumper : MonoBehaviour
{
    public float PushBackForce;

    Vector3 PopBumberLocation;
    private AudioSource HitSound;
    void Start()
    {
        PopBumberLocation = gameObject.transform.position;
        HitSound = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ball = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 forceVector = (ball.position - PopBumberLocation).normalized * PushBackForce;
            ball.AddForce(forceVector);
            HitSound.Play();
        }
    }

}
