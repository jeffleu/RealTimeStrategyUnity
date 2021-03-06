using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
  Transform mainCameraTransform;

  void Start()
  {
    mainCameraTransform = Camera.main.transform;
  }

  void LateUpdate()
  {
    // Face health bar at camera
    transform.LookAt(
      transform.position + mainCameraTransform.rotation * Vector3.forward,
      mainCameraTransform.rotation * Vector3.up
    );
  }
}
