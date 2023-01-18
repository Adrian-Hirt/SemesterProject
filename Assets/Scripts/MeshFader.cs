using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class MeshFader : MonoBehaviour {
  public Material material;
  private Color initialColor = new Color(0.7176f, 0.7176f, 0.7176f, 1.0f);
  private float currentRenderMode = 0;

  public void Start() {
    // Reset color
    material.color = initialColor;

    // Enable opaque mode
    material.SetFloat("_Mode", 0); // opaque
    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
    material.SetInt("_ZWrite", 1);
    material.DisableKeyword("_ALPHATEST_ON");
    material.DisableKeyword("_ALPHABLEND_ON");
    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    material.renderQueue = -1;

  }

  public void UpdateAlphaFromSlider(SliderEventData eventData) {
    // The color we'll modify
    Color color = material.color;

    if (eventData.NewValue < 0.01f) {
      // Enable cutout mode to completely hide the meshes
      material.SetFloat("_Mode", 1); // cutout
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
      material.SetInt("_ZWrite", 1);
      material.EnableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = 2450;

      // Set alpha value to zero
      color.a = 0.0f;
    }
    else if(eventData.NewValue > 0.99f) {
      // Enable opaque mode
      material.SetFloat("_Mode", 0); // opaque
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
      material.SetInt("_ZWrite", 1);
      material.DisableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = -1;

      // Set alpha value to one
      color.a = 1.0f;
    }
    else {
      // Enable transparent mode
      material.SetFloat("_Mode", 3); // transparent
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
      material.SetInt("_ZWrite", 0);
      material.DisableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = 3000;

      // Update the alpha value
      color.a = eventData.NewValue;
    }

    // Update the color
    material.color = color;
  }

}
