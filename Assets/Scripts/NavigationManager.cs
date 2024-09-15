using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LineRenderer line;
    [SerializeField] float lineYOffset = 0.5f;
    [SerializeField] private List<NavigationTarget> navigationTargets;
    private UnityEngine.AI.NavMeshPath navMeshPath;
    NavMeshHit hit;
    Vector3 targetPosition;

    private void Start()
    {
        navMeshPath = new UnityEngine.AI.NavMeshPath();

        // disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Update()
    {
        if ( navigationTargets.Count > 0)
        {
            NavMesh.SamplePosition(navigationTargets[0].transform.position, out hit, 1.0f, NavMesh.AllAreas);
                targetPosition = hit.position; 
                                               
            NavMesh.CalculatePath(player.position, targetPosition, NavMesh.AllAreas, navMeshPath);
            Debug.Log(navMeshPath.status);
            Debug.Log(targetPosition);
            Debug.Log(player.position);

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



}
