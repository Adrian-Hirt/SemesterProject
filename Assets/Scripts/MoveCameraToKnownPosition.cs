using UnityEngine;
using Microsoft.MixedReality.Toolkit;

public class MoveCameraToKnownPosition : MonoBehaviour {
  public GameObject sceneContent;

  public void SetPositionOfCamera(GameObject target) {
    // First we reset the playspace to the zero point
    //Vector3 resetPlayspace = Vector3.zero - MixedRealityPlayspace.Transform.position;
    //MixedRealityPlayspace.Transform.Translate(resetPlayspace);

    //// Compute the transform to the new position
    //Vector3 playspaceOffset = target.transform.position; // - Camera.main.transform.position;
    ////playspaceOffset.z += 1.7f;

    //// Update the playspace position
    //MixedRealityPlayspace.Transform.Translate(playspaceOffset);

    // Set the position of the playspace to the chosen target
    //Vector3 targetTranslate =  target.transform.position - MixedRealityPlayspace.Transform.position;
    //MixedRealityPlayspace.Transform.Translate(targetTranslate, Space.World);

    // Reset position
    sceneContent.transform.position = Vector3.zero;

    // Update position of scene content
    sceneContent.transform.position = (-1 * target.transform.position) + Camera.main.transform.position;
  }
}
