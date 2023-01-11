using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalArrow : MonoBehaviour {
  public GameObject DirectionalArrowPlane;

  // Update is called once per frame
  void Update() {
    // Compute position of arrow in front of the camers
    Vector3 targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * 1.5f);

    // Move the arrow down a bit
    targetPosition.y -= 0.5f;

    // Set the position of the arrow
    DirectionalArrowPlane.transform.position = targetPosition;  
  }
}
