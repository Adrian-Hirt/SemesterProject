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
    //material.shader = diffuseShader;
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

  //public void UpdateAlphaFromSlider(SliderEventData eventData) {
  //  return;
  //  if (eventData.NewValue < 0.01f) {
  //    // Switch to invisible shader to completely hide the mesh
  //    material.shader = invisibleShader;

  //  }
  //  else if (eventData.NewValue > 0.99f) {
  //    // Switch to standard diffuse shader
  //    material.shader = diffuseShader;

  //    // Enable opaque mode
  //    material.SetFloat("_Mode", 0); // opaque
  //    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
  //    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
  //    material.SetInt("_ZWrite", 1);
  //    material.DisableKeyword("_ALPHATEST_ON");
  //    material.DisableKeyword("_ALPHABLEND_ON");
  //    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
  //    material.renderQueue = -1;

  //    // Update the color
  //    material.color = diffuseInitialColor;
  //  }
  //  else {
  //    // Switch to standard diffuse shader
  //    material.shader = diffuseShader;

  //    // Enable transparent mode
  //    material.SetFloat("_Mode", 3); // transparent
  //    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
  //    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
  //    material.SetInt("_ZWrite", 0);
  //    material.DisableKeyword("_ALPHATEST_ON");
  //    material.DisableKeyword("_ALPHABLEND_ON");
  //    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
  //    material.renderQueue = 3000;

  //    float colorValue = eventData.NewValue * colorStepFactor;

  //    // Get color from material
  //    Color color = new Color(colorValue, colorValue, colorValue, eventData.NewValue);

  //    // Update the color
  //    material.color = color;
  //  }
  //}

}
