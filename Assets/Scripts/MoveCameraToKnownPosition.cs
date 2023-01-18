using UnityEngine;
using Microsoft.MixedReality.Toolkit;

public class MoveCameraToKnownPosition : MonoBehaviour {
  public void SetPositionOfCamera(GameObject target) {
    // First we reset the playspace to the zero point
    Vector3 resetPlayspace = Vector3.zero - MixedRealityPlayspace.Transform.position;
    MixedRealityPlayspace.Transform.Translate(resetPlayspace);

    // Compute the transform to the new position
    Vector3 playspaceOffset = target.transform.position - Camera.main.transform.position;
    playspaceOffset.z += 1.7f;

    // Update the playspace position
    MixedRealityPlayspace.Transform.Translate(playspaceOffset);
  }
}
