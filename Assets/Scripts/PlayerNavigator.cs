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

  // GUI texts
  public TMP_Text remainingDistanceText;
  public TMP_Text currentTargetText;

  // Offset from the ground of the nav line in meters
  private float pathHeightOffset = 1.0f;

  // How often (in seconds) should the path be updated
  private float pathUpdateSpeed = 0.25f;

  // By default, we allow all navmeshes to be used
  private int enabledAreaMask = NavMesh.AllAreas;

  // Our path should have a max length of 500 corners and
  // visit 1024 navmesh polygons. This can be changed, but
  // one needs to make sure that the computation of the path
  // still happens in reasonable times
  private const int maxPathLength = 500;
  private const int maxIterations = 1024;

  // We set the allowed pathNodePoolSize to the max number possible
  private const int pathNodePoolSize = ushort.MaxValue;

  // Set the target of the navigation
  public void setTarget(GameObject target) {
    targetObject = target;
    currentTargetText.text = "Current target: " + targetObject.name;
    line.enabled = true;
  }

  // Remove the target of the navigation, which also will disable
  // the calculation of the path
  public void clearTarget() {
    targetObject = null;
    currentTargetText.text = "-- No target selected --";
    remainingDistanceText.text = "0m";
    line.enabled = false;
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

  // Update is called once per frame
  //void Update() {
  //  if (targetObject != null) {
  //    WaitForSeconds Wait = new WaitForSeconds(pathUpdateSpeed);
  //    NavMeshPath path = new NavMeshPath();

  //    // Get the nearest places on the navmesh for the target and the current position
  //    NavMeshHit myNavHitStart;
  //    if (!NavMesh.SamplePosition(transform.position, out myNavHitStart, 100, enabledAreaMask)) {
  //      Debug.Log("Could not find nearest navmesh for the start position");
  //      return;
  //    }

  //    NavMeshHit myNavHitEnd;
  //    if (!NavMesh.SamplePosition(targetObject.transform.position, out myNavHitEnd, 100, enabledAreaMask)) {
  //      Debug.Log("Could not find nearest navmesh for the end position");
  //      return;
  //    }

  //    if (NavMesh.CalculatePath(myNavHitStart.position, myNavHitEnd.position, enabledAreaMask, path)) {
  //      Debug.Log(path.corners.Length);
  //      // Check if we have a complete path or not. If we don't have a complete path, we can't
  //      // proceed with the navigation, and should just return here. We also should hide the
  //      // NavLine, such that people don't get confused
  //      if (path.status != NavMeshPathStatus.PathComplete) {
  //        //line.enabled = false;
  //        //remainingDistanceText.text = "Target not reachable";
  //        //return;
  //      }

  //      // Update ray
  //      line.positionCount = path.corners.Length;

  //      // Turn on the line (in case it's been disabled previously by an incomplete
  //      // havigation
  //      line.enabled = true;

  //      // For calculation of the line length
  //      float lineLength = 0.0f;

  //      for (var i = 0; i < path.corners.Length; i++) {
  //        line.SetPosition(i, path.corners[i] + Vector3.up * pathHeightOffset);
  //        if (i != 0) {
  //          lineLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
  //        }
  //      }

  //      remainingDistanceText.text = lineLength.ToString("n1") + "m";
  //    }
  //    else {
  //      // Target not reachable
  //      line.enabled = false;
  //      remainingDistanceText.text = "Target not reachable";
  //    }
  //  }
  //}

  // The update() method currently just starts the coroutine for updatind the path
  void Update() {
    StartCoroutine(UpdatePath());
  }

  IEnumerator UpdatePath() {
    calculatePath();
    yield return new WaitForSeconds(pathUpdateSpeed);
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
              // Update the size of the line
              line.positionCount = positionCount;

              // Turn on the line (in case it's been disabled previously by an incomplete
              // havigation
              line.enabled = true;

              // For calculation of the line length
              float lineLength = 0.0f;
 
              // Set the positions of the line, and compute the length of the navigation
              for (int i = 0; i < positionCount; i++) {
                line.SetPosition(i, straightPath[i].position + Vector3.up * pathHeightOffset);
                if (i != 0) {
                  lineLength += Vector3.Distance(straightPath[i - 1].position, straightPath[i].position);
                }
              }

              // Update the info in the GUI
              remainingDistanceText.text = lineLength.ToString("n1") + "m";
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

          // Target not reachable
          line.enabled = false;
          remainingDistanceText.text = "Target not reachable";

          // Exit from the loop
          break;

        default:
          // Stop searching
          searching = false;

          // Target not reachable
          line.enabled = false;
          remainingDistanceText.text = "Target not reachable";

          // Exit from the loop
          break;
      }
    }

    // Also free the query object at the end, such that all objects are freed
    query.Dispose();
  }
}
