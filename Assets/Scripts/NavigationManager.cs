using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LineRenderer line;
    public float lineYOffset { get; set; } = 0.5f;

    [SerializeField] private List<NavigationTarget> navigationTargets;
    [SerializeField] TMP_Dropdown targetDropdown;
    private NavMeshPath navMeshPath;
    [SerializeField] private NavigationTarget selectedTarget;

    NavMeshHit hit;
    Vector3 targetPosition;

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
        // Subscribe to the dropdown's onValueChanged event
        targetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        if(navigationTargets.Count > 0)
        {
            selectedTarget = navigationTargets[0];

        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }




    private void Update()
    {
        if (selectedTarget != null)
        {
            NavMesh.SamplePosition(selectedTarget.transform.position, out hit, 1.0f, NavMesh.AllAreas);
                targetPosition = hit.position; 
                                               
            NavMesh.CalculatePath(player.position, targetPosition, NavMesh.AllAreas, navMeshPath);
           
            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                line.positionCount = navMeshPath.corners.Length;

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

    void OnDropdownValueChanged(int index)
    {
        selectedTarget = navigationTargets[index];
    }

}
