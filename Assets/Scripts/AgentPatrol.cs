using UnityEngine;
using System.Collections;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class AgentPatrol : MonoBehaviour
{
    // Publicvars
    public PatrolPathManager WaypointNetwork = null;
 
    public int CurrentIndex = 0;
     [HideInInspector] public bool HasPath = false;
    [HideInInspector] public bool PathPending = false;
    [HideInInspector] public bool PathStale = false;
    public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;


    // Private Members
    private NavMeshAgent _navAgent = null;

    // -----------------------------------------------------
    // Name :	Start
    // Desc	:	Cache MavMeshAgent and set initial 
    //			destination.
    // -----------------------------------------------------
    void Start()
    {
        // Cache NavMeshAgent Reference
        _navAgent = GetComponent<NavMeshAgent>();
      

        // If not valid Waypoint Network has been assigned then return
        if (WaypointNetwork == null) return;

        // Set first waypoint
        SetNextDestination(false);
    }

    // -----------------------------------------------------
    // Name	:	SetNextDestination
    // Desc	:	Optionally increments the current waypoint
    //			index and then sets the next destination
    //			for the agent to head towards.
    // -----------------------------------------------------
    void SetNextDestination(bool increment)
    {
        // If no network return
        if (!WaypointNetwork) return;

        // Calculatehow much the current waypoint index needs to be incremented
        int incStep = increment ? 1 : 0;
        Transform nextWaypointTransform = null;

        // Calculate index of next waypoint factoring in the increment with wrap-around and fetch waypoint 
        int nextWaypoint = (CurrentIndex + incStep >= WaypointNetwork.Waypoints.Count) ? 0 : CurrentIndex + incStep;
        nextWaypointTransform = WaypointNetwork.Waypoints[nextWaypoint];

        // Assuming we have a valid waypoint transform
        if (nextWaypointTransform != null)
        {
            // Update the current waypoint index, assign its position as the NavMeshAgents
            // Destination and then return
            CurrentIndex = nextWaypoint;
            _navAgent.destination = nextWaypointTransform.position;
            return;
        }

        // We did not find a valid waypoint in the list for this iteration
        CurrentIndex = nextWaypoint;
    }

    // ---------------------------------------------------------
    // Name	:	Update
    // Desc	:	Called each frame by Unity
    // ---------------------------------------------------------
    void Update()
    {
        // Copy NavMeshAgents state into inspector visible variables
        HasPath = _navAgent.hasPath;
        PathPending = _navAgent.pathPending;
        PathStale = _navAgent.isPathStale;
        PathStatus = _navAgent.pathStatus;


       


            _navAgent.isStopped = false;
            float distance = Vector3.Distance(transform.position, WaypointNetwork.Waypoints[CurrentIndex].position);
            if (distance < 10)
            {
                SetNextDestination(true);
            }
            else
                SetNextDestination(false);
        


    }


}










































































