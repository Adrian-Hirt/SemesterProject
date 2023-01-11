using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class MultilevelNavigationMenuButton : MonoBehaviour {
  public GameObject container;

  // Can be used on a button to open the "child" button collection,
  // e.g. when pressing a button for the E-Floor => open the collection
  // of rooms in the E-Floor
  public void openChild(GameObject child) {
    container.SetActive(false);
    child.SetActive(true);
  }

  // Can be used for the "back" button functionality, where we
  // can go back one layer in the menu
  public void backToParent(GameObject parent) {
    container.SetActive(false);
    parent.SetActive(true);
  }
}
