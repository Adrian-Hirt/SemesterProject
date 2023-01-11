using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using Unity.Collections;
using TMPro;

public class PlayerNavigator : MonoBehaviour {
  // Line shown in GUI
  public LineRenderer line;

  // The target of the navigation
  public GameObject targetObject;

  // DirectionalArrow plane
  public GameObject DirectionalArrowPlane;

  // GUI texts
  public TMP_Text remainingDistanceText;
  public TMP_Text currentTargetText;

  // Offset from the ground of the nav line in meters
  private float pathHeightOffset = 1.0f;

  // How often (in seconds) should the path be updated
  private float pathUpdateSpeed = 0.25f;

  // By default, we allow all navmeshes to be used
  private int enabledAreaMask = NavMesh.AllAreas;

  // Keep track of wether stairs are enabled or not
  private bool stairsEnabled = true;

  // Our path should have a max length of 500 corners and
  // visit 1024 navmesh polygons. This can be changed, but
  // one needs to make sure that the computation of the path
  // still happens in reasonable times
  private const int maxPathLength = 500;
  private const int maxIterations = 1024;

  // Distance we need to be away from our goal to consider the navigation to be done
  private const float navigationEndThreshold = 0.5f;

  // We set the allowed pathNodePoolSize to the max number possible
  private const int pathNodePoolSize = ushort.MaxValue;

  // Position of the next "node" in the nav path (for directional arrow nav)
  private Vector3 nextNodePosition;

  // Enum to hold the various renderer modes we have
  public enum NavRendererMode {
    Line = 0,
    DirectionalArrow = 1
  }

  // Holds the current renderer mode
  NavRendererMode currentNavRendererMode = NavRendererMode.Line;

  // Keep track if we should render the navigation
  private bool renderNavigation = false;

  // Set the target of the navigation
  public void setTarget(GameObject target) {
    targetObject = target;
    currentTargetText.text = "Current target: " + targetObject.name;
  }

  // Remove the target of the navigation, which also will disable
  // the calculation of the path
  public void clearTarget() {
    targetObject = null;
    currentTargetText.text = "-- No target selected --";
    remainingDistanceText.text = "0m";
    renderNavigation = false;
  }

  // Disable all navmesh areas which are names "Stairs", such that
  // we can only use ramps, escalators and similar things to get
  // up a floow
  public void disableStairs() {
    int areaMask = NavMesh.AllAreas;
    areaMask -= 1 << NavMesh.GetAreaFromName("Stairs");
    enabledAreaMask = areaMask;
  }

  // Enable stairs for navigation
  public void enableStairs() {
    enabledAreaMask = NavMesh.AllAreas;
  }

  // Toggle stairs for navigation
  public void toggleStairs() {
    // Toggle the boolean flag
    stairsEnabled = !stairsEnabled;

    if (stairsEnabled) {
      // Use all areas of the navmesh for navigation
      enabledAreaMask = NavMesh.AllAreas;
    }
    else {
      // Remove stairs from the available areas for navigation
      int areaMask = NavMesh.AllAreas;
      areaMask -= 1 << NavMesh.GetAreaFromName("Stairs");
      enabledAreaMask = areaMask;
    }
  }

  // The Start() method currently just starts the coroutine for updating the path
  void Start() {
    StartCoroutine(UpdatePath());
  }

  IEnumerator UpdatePath() {
    while (true) {
      calculatePath();
      yield return new WaitForSeconds(pathUpdateSpeed);
    }
  }

  void Update() {
    if(currentNavRendererMode == NavRendererMode.DirectionalArrow) {
      UpdateDirectionalArrow();
    }
  }

  // Adapted from https://forum.unity.com/threads/how-to-use-navmeshquery-get-path-points.646861/
  private void calculatePath() {
    // Just don't do anything if we don't have a target object
    if (targetObject == null) {
      return;
    }

    // The query struct we'll use to search the path with later
    NavMeshQuery query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.Persistent, pathNodePoolSize);

    // Start and end position of our navigation, which needs to be put into a MapLocation
    // struct for the query
    NavMeshLocation navigationStart = query.MapLocation(transform.position, Vector3.one * 10, 0);
    NavMeshLocation navigationTarget = query.MapLocation(targetObject.transform.position, Vector3.one * 10, 0);

    // To exit the while loop below
    bool searching = true;

    // Start searching for a path, from the start of our navigation to the
    // target. The enabledAreaMask parameter allows us to selectively enable
    // and disable certain nav mesh types
    PathQueryStatus status = query.BeginFindPath(navigationStart, navigationTarget, enabledAreaMask);

    while (searching) {
      switch (status) {
        case PathQueryStatus.InProgress:
          // If we're still in the InProgress state, we need to call UpdateFindPath again
          // to continue the search for the path
          status = query.UpdateFindPath(maxIterations, out int currentIterations);
          break;

        case PathQueryStatus.Success:
          // Create a struct to hold the result of the Nav mesh query
          NativeArray<PolygonId> result = new NativeArray<PolygonId>(maxPathLength, Allocator.Persistent);

          // Prepares the data found by the find path methods such that we can retrieve them later
          PathQueryStatus finalStatus = query.EndFindPath(out int pathLength);

          // Copy the NavMesh nodes that the path found to the result array
          query.GetPathResult(result);

          // We now need to prepare some data structures, such that we can compute a
          // path from the found navmesh nodes
          NativeArray<NavMeshLocation> straightPath = new NativeArray<NavMeshLocation>(pathLength, Allocator.Temp);
          NativeArray<StraightPathFlags> straightPathFlags = new NativeArray<StraightPathFlags>(pathLength, Allocator.Temp);
          NativeArray<float> vertexSide = new NativeArray<float>(pathLength, Allocator.Temp);

          try {
            // Variable to hold the number of corners in our path
            int positionCount = 0;

            // Compute the path from the navmesh nodes
            PathQueryStatus pathStatus = PathUtils.FindStraightPath(query, transform.position, targetObject.transform.position, result, pathLength, ref straightPath, ref straightPathFlags, ref vertexSide, ref positionCount, maxPathLength);

            // If computing the actual path was successfull, we can now update the line and render the line
            if (pathStatus == PathQueryStatus.Success) {
              // Turn on the navigation
              renderNavigation = true;

              // For calculation of the line length
              float lineLength = 0.0f;
 
              // Compute the length of the navigation
              for (int i = 1; i < positionCount; i++) {
                lineLength += Vector3.Distance(straightPath[i - 1].position, straightPath[i].position);
              }

              // Render the line (if we want to render a line)
              DrawLine(straightPath, positionCount);

              // Update the position of the "next" node
              nextNodePosition = straightPath[1].position;

              // Update the info in the GUI
              remainingDistanceText.text = lineLength.ToString("n1") + "m";

              // If we're closer to the target than a certain threshold, we consider
              // the navigation to be complete, and therefore disable the navigation
              if (lineLength < navigationEndThreshold) {
                remainingDistanceText.text = "Target reached!";
                targetObject = null;
                line.enabled = false;
              }
            }
          }
          finally {
            // Always call the Dispose methods, or otherwise we get a memory leak
            straightPath.Dispose();
            straightPathFlags.Dispose();
            vertexSide.Dispose();
            result.Dispose();
          }

          searching = false;
          break;

        case PathQueryStatus.Failure:
          // Stop searching
          searching = false;

          DisableRenderings();

          // Exit from the loop
          break;

        default:
          // Stop searching
          searching = false;

          DisableRenderings();

          // Exit from the loop
          break;
      }
    }

    // Also free the query object at the end, such that all objects are freed
    query.Dispose();
  }

  public void ChangeLineRenderer(LineRenderer other) {
    // Disable old line
    line.enabled = false;

    // Update line
    line = other;

    //// Enable new line
    //line.enabled = true;
  }

  public void SetNavRenderMode(int newMode) {
    // Disable old renderer
    line.enabled = false;
    DirectionalArrowPlane.SetActive(false);

    // Update the mode of rendering
    currentNavRendererMode = (NavRendererMode)newMode;

    // Return early if we don't render the nav right now
    if (!renderNavigation) {
      return;
    }

    // Enable the new renrerer
    if (currentNavRendererMode == NavRendererMode.DirectionalArrow) {
      DirectionalArrowPlane.SetActive(true);
    }
    else {
      line.enabled = true;
    }
  }

  private void DisableRenderings() {
    renderNavigation = false;

    // Disable the line
    line.enabled = false;

    // Disable the directional arrow
    DirectionalArrowPlane.SetActive(false);

    // Update infotext
    remainingDistanceText.text = "Target not reachable";

    // No next position
    nextNodePosition = Vector3.zero;
  }

  private void DrawLine(NativeArray<NavMeshLocation> straightPath, int positionCount) {
    // Check if we even want to render the navigation
    if(!renderNavigation) {
      line.enabled = false;
      return;
    }

    // If the chosen mode is not one where we draw a line we don't need to do anything
    if(currentNavRendererMode != NavRendererMode.Line) {
      line.enabled = false;
      return;
    }

    // Otherwise, enable the line and draw it
    line.enabled = true;

    // Update the size of the line
    line.positionCount = positionCount;

    // Set the positions of the line, and compute the length of the navigation
    for (int i = 0; i < positionCount; i++) {
      line.SetPosition(i, straightPath[i].position + Vector3.up * pathHeightOffset);
    }
  }

  private void UpdateDirectionalArrow() {
    if (renderNavigation && nextNodePosition != Vector3.zero) {
      DirectionalArrowPlane.SetActive(true);
      // Point the directional arrow towards the next position.
      DirectionalArrowPlane.transform.LookAt(nextNodePosition + Vector3.up * pathHeightOffset);
    }
    else {
      DirectionalArrowPlane.SetActive(false);
    }
  }
}
