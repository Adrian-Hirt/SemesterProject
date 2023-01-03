using UnityEngine;
using Microsoft.MixedReality.Toolkit;

public class MoveCameraToKnownPosition : MonoBehaviour {
  public void SetPositionOfCamera(GameObject target) {
    // Vector3 delta = Vector3.zero;
    // delta = MixedRealityPlayspace.Transform.position - target.transform.position;
    // MixedRealityPlayspace.Transform.Translate(delta);

    Camera.main.transform.position = new Vector3(target.transform.position.x, 0.0f, target.transform.position.z);
  }
}
