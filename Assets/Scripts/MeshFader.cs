using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class MeshFader : MonoBehaviour {
  public Material material;
  public Shader invisibleShader;
  public Shader wireframeShader;

  private bool invisibleShaderEnabled = true;

  public void Start() {
    // Reset shader to the invisible shader
    material.shader = invisibleShader;
  }

  public void ToggleEnvironmentShaders() {
    // Toggle the boolean
    invisibleShaderEnabled = !invisibleShaderEnabled;

    // Update the shader
    if (invisibleShaderEnabled) {
      material.shader = invisibleShader;
    }
    else {
      material.shader = wireframeShader;
    }

  }
}
