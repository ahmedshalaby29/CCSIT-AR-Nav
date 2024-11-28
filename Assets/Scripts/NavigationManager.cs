using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private GameObject navigationPanel;

    [SerializeField] private Transform worldPosition;
        [SerializeField] private Transform arCamera;

     [SerializeField] private NavMeshAgent navMeshAgent;

    [SerializeField] private LineRenderer line;
    public float lineYOffset { get; set; } = 0.5f;

    [SerializeField] public List<NavigationTarget> navigationTargets;
    [SerializeField] public TMP_Dropdown targetDropdown;
      [SerializeField] public TMP_Dropdown currentLocationDropdown;

        [SerializeField] TMP_Dropdown addLocationsDropdown;

    private NavMeshPath navMeshPath;
    public NavigationTarget selectedTarget;

    NavMeshHit hit;
    Vector3 targetPosition;
    public static NavigationManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    private void Start()
    {
        navMeshPath = new UnityEngine.AI.NavMeshPath();
        // disable screen dimming
        // Create a list to hold the dropdown options
        List<string> options = new List<string>();

        // Loop through the navigation targets and add their names to the options list
        foreach (var target in navigationTargets)
        {
            // Assuming NavigationTarget has a property called "name"
            options.Add(target.name); // or target.ToString() if applicable
        }

        // Add options to the dropdown
        targetDropdown.AddOptions(options);
        addLocationsDropdown.AddOptions(options);
        currentLocationDropdown.AddOptions(options);
     
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }



private void Update()
{
    // Continuously sync the NavMeshAgent with the AR Camera's position
    if (NavMesh.SamplePosition(Camera.main.transform.position, out hit, 10.0f, NavMesh.AllAreas))
    {
        // If the camera's position is valid on the NavMesh, move the agent
        navMeshAgent.Warp(hit.position);
    }
    else
    {
        Debug.LogWarning("Camera moved outside the NavMesh area.");
    }

    // Continue drawing the path if a valid target is selected
    if (selectedTarget != null)
    {
        targetPosition = selectedTarget.transform.position;

        // Calculate the path
        NavMesh.CalculatePath(navMeshAgent.transform.position, targetPosition, NavMesh.AllAreas, navMeshPath);

        if (navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            line.positionCount = navMeshPath.corners.Length;

            // Adjust the line's Y-offset and set positions
            for (int i = 0; i < navMeshPath.corners.Length; i++)
            {
                navMeshPath.corners[i].y += lineYOffset;
            }

            line.SetPositions(navMeshPath.corners);
        }
        else
        {
            line.positionCount = 0;
        }
    }
}

 public void OnNavigateClick()
    {
        // Set the selected target based on the dropdown value
        selectedTarget = navigationTargets[targetDropdown.value];
        // Get the current location based on the user's dropdown selection
        NavigationTarget currentLocation = navigationTargets[currentLocationDropdown.value];

        if (selectedTarget == null)
        {
            Debug.LogError("Selected target is null.");
            return;
        }

        // Reposition the world to align the selected target with the AR Camera
        RepositionWorld(currentLocation.transform);

        // Hide the navigation panel
        navigationPanel.SetActive(false);
    }

    private void RepositionWorld(Transform selectedTarget)
    {
        // Calculate the offset between the ARCamera and the selected target
        Vector3 offset = arCamera.position - selectedTarget.position;

        // Apply the offset to the world (move the entire parent)
        worldPosition.position += offset;

        Debug.Log($"World repositioned to align {selectedTarget.name} with AR Camera.");
    }


}
