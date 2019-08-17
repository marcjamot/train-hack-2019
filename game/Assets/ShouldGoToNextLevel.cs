using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShouldGoToNextLevel : MonoBehaviour
{
  private Vector3 _pausedVelocity;
  private Vector3 _pausedAngularVelocity;

  // Start is called before the first frame update




  private CameraHop cameraHop;
  private Rigidbody ball;

  void Start()
    {
cameraHop = GameObject.Find("GameProgression").GetComponent<CameraHop>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
    if (cameraHop._transitioning){
    return;
    }

      if (other.gameObject.CompareTag("Ball"))
      {

          ball = other.gameObject.GetComponent<Rigidbody>();
          Debug.Log("SHOULD GO TO NEXT LEVEL", ball);
          _pausedVelocity = ball.velocity;
          _pausedAngularVelocity = ball.angularVelocity;
 ball.isKinematic = true;

          
          cameraHop.Hop();
          cameraHop.startMeAgain(this);

      }
    }

    private void OnTriggerExit(Collider other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // BallList.Remove(other.gameObject.GetComponent<Rigidbody>());
      }
    }

  internal void StartBallAgainPleease()
  {
    if (ball == null) {
      return;
    }
    Debug.Log("nu kör vi igen");
    ball.isKinematic = false;
    ball.velocity = _pausedVelocity;
    ball.angularVelocity = _pausedAngularVelocity;
  }


  // public void OnMouseEnter()
  // {
  //   Debug.Log("Pause with velocity=" + _rigidBody.velocity + " & angularVelocity=" + _rigidBody.angularVelocity);

  // }

  // public void OnMouseExit()
  // {
  //   _rigidBody.isKinematic = false;
  //   _rigidBody.velocity = _pausedVelocity;
  //   _rigidBody.angularVelocity = _pausedAngularVelocity;
  //   Debug.Log("Resume with velocity=" + _rigidBody.velocity + " & angularVelocity=" + _rigidBody.angularVelocity);
  // }

}
