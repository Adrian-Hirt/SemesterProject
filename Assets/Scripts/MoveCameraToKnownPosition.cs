using UnityEngine;
using Microsoft.MixedReality.Toolkit;

public class MoveCameraToKnownPosition : MonoBehaviour {
  public GameObject sceneContent;

  public void SetPositionOfCamera(GameObject target) {
    // First we reset the playspace to the zero point, such that it's
    // easier to compute the other translations below
    MixedRealityPlayspace.Position = Vector3.zero;

    // Compute the new position we'll need to set
    Vector3 targetTranslate = target.transform.position - Camera.main.transform.position;

    // Update position of playspace
    MixedRealityPlayspace.Position = targetTranslate;
  }
}
