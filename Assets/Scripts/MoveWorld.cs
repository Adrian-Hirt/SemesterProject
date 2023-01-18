using UnityEngine;
using Microsoft.MixedReality.Toolkit;

public class MoveWorld : MonoBehaviour {
  private const float largeTranslationOffsetMultiplier = 10.0f;
  private const float largeRotationOffsetMultiplier = 15.0f;

  private bool largeOffsetModeEnabled = false;

  public void MoveInXDirection(float offset) {
    MoveSpace(offset, 0, 0);
  }

  public void MoveInYDirection(float offset) {
    MoveSpace(0, offset, 0);
  }

  public void MoveInZDirection(float offset) {
    MoveSpace(0, 0, offset);
  }

  public void ToggleLargeOffsetMode() {
    largeOffsetModeEnabled = !largeOffsetModeEnabled;
  }

  public void RotateAroundY(float degrees) {
    RotateSpace(degrees, Vector3.up);
  }

  private void MoveSpace(float x, float y, float z) {
    // Apply large offset multiplicator
    if (largeOffsetModeEnabled) {
      x *= largeTranslationOffsetMultiplier;
      y *= largeRotationOffsetMultiplier;
      z *= largeRotationOffsetMultiplier;
    }

    // Compute delta
    Vector3 delta = new Vector3(x, y, z);

    // Update the playspace position
    MixedRealityPlayspace.Transform.Translate(delta);
  }

  private void RotateSpace(float angle, Vector3 axis) {
    if (largeOffsetModeEnabled) {
      angle *= largeRotationOffsetMultiplier;
    }

    MixedRealityPlayspace.Transform.RotateAround(MixedRealityPlayspace.Transform.position, axis, angle);
  }
}
