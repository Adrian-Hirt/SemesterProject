using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuModeToggler : MonoBehaviour {

  public GameObject FlatMenu;
  public GameObject MultilevelMenu;
  private bool flatMenuEnabled = false;

  public void ToggleMenuMode() {
    // Switch the boolean
    flatMenuEnabled = !flatMenuEnabled;

    if (flatMenuEnabled) {
      // Turn on the flat menu and turn off the multilevel menu
      FlatMenu.SetActive(true);
      MultilevelMenu.SetActive(false);
    }
    else  {
      // Turn on the multilevel menu and turn off the flat menu
      FlatMenu.SetActive(false);
      MultilevelMenu.SetActive(true);
    }
  }
}
