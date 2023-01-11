using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenu : MonoBehaviour {
  public GameObject FlatMenu;
  public GameObject MultilevelMenu;
  public GameObject SettingsMenu;
  public GameObject SettingsMenuHome;
  public GameObject InitialOpenMenu;

  private bool flatMenuEnabled = false;
  private bool settingsShown = false;
  private GameObject CurrentOpenMenu;
  private GameObject PreviousNestedOpenMenu;

  void Start() {
    CurrentOpenMenu = InitialOpenMenu;
  }

  public void OpenOther(GameObject other) {
    // Disable the current open menu
    CurrentOpenMenu.SetActive(false);

    // Open the other menu
    other.SetActive(true);

    // Set the current open menu to the other
    CurrentOpenMenu = other;
  }

  public void ToggleMenuMode() {
    // Switch the boolean
    flatMenuEnabled = !flatMenuEnabled;

    // If the settings are currently shown, don't
    // do anything other than switching the boolean
    if (settingsShown) {
      return;
    }

    if (flatMenuEnabled) {
      // Turn on the flat menu and turn off the multilevel menu
      FlatMenu.SetActive(true);
      MultilevelMenu.SetActive(false);
    }
    else {
      // Turn on the multilevel menu and turn off the flat menu
      FlatMenu.SetActive(false);
      MultilevelMenu.SetActive(true);
    }
  }

  public void OpenSettings() {
    // Don't do anything if the settings is already open
    if (settingsShown) {
      return;
    }

    // Update boolean flag
    settingsShown = true;

    // Close the current menu
    CurrentOpenMenu.SetActive(false);

    // Close the flat or nested menu
    if (flatMenuEnabled) {
      FlatMenu.SetActive(false);
    }
    else {
      // Store the previously open menu
      PreviousNestedOpenMenu = CurrentOpenMenu;

      MultilevelMenu.SetActive(false);
    }

    // Open the settings menu and its home
    SettingsMenu.SetActive(true);
    SettingsMenuHome.SetActive(true);

    // Set current open menu to settings home
    CurrentOpenMenu = SettingsMenuHome;
  }

  public void CloseSettings() {
    // Update boolean flag
    settingsShown = false;

    // Close the current menu
    CurrentOpenMenu.SetActive(false);

    // Close the settings menu
    SettingsMenu.SetActive(false);

    if (flatMenuEnabled) {
      // Open the flat menu
      FlatMenu.SetActive(true);
    }
    else {
      // Open the nested menu
      MultilevelMenu.SetActive(true);

      // Open the previously opened nested menu
      PreviousNestedOpenMenu.SetActive(true);

      // Set current open menu to previously opened
      CurrentOpenMenu = PreviousNestedOpenMenu;

      // Set the previously opened menu to null
      PreviousNestedOpenMenu = null;
    }
  }
}
