using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class MeshFader : MonoBehaviour {
  public Material material;
  private Color initialColor = new Color(0.7176f, 0.7176f, 0.7176f, 1.0f);

  public void Start() {
    material.color = initialColor;
  }

  public void UpdateAlphaFromSlider(SliderEventData eventData) {
    Color color = material.color;
    color.a = eventData.NewValue;

    material.color = color;
  }

}
