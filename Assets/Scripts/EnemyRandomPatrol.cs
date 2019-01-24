using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRandomPatrol : MonoBehaviour {

    //Privte Vars
    private NavMeshAgent navMeshAgent;
    private Vector3 _minBoundsPoint;
    private Vector3 _maxBoundsPoint;
    private float _boundsSize = float.NegativeInfinity;

    //Public Vars
    [Range(3.5f,7)]
    public float Speed=3.5f; //Set Speed Range
    

    protected void Start()
    {
        navMeshAgent =GetComponent<NavMeshAgent>();
        navMeshAgent.speed = Speed;
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
    }
    
    protected void FixedUpdate()
    {
        if (navMeshAgent.pathPending)
            return;
      // SetDestination();
    }


    protected void SetDestination()
    {
        if (navMeshAgent.desiredVelocity.magnitude == 0)
            navMeshAgent.SetDestination(GetRandomTargetPoint());
    }

  /// <summary>
  /// Get Random Point On Terrain Bounderys And Return 
  /// </summary>
  /// <returns></returns>
    private Vector3 GetRandomTargetPoint()
    {
        if (_boundsSize < 0)
        {
            _minBoundsPoint = Vector3.one * float.PositiveInfinity;
            _maxBoundsPoint = -_minBoundsPoint;
            var vertices = NavMesh.CalculateTriangulation().vertices;
            foreach (var point in vertices)
            {
                if (_minBoundsPoint.x > point.x)
                    _minBoundsPoint = new Vector3(point.x, _minBoundsPoint.y, _minBoundsPoint.z);
                if (_minBoundsPoint.y > point.y)
                    _minBoundsPoint = new Vector3(_minBoundsPoint.x, point.y, _minBoundsPoint.z);
                if (_minBoundsPoint.z > point.z)
                    _minBoundsPoint = new Vector3(_minBoundsPoint.x, _minBoundsPoint.y, point.z);
                if (_maxBoundsPoint.x < point.x)
                    _maxBoundsPoint = new Vector3(point.x, _maxBoundsPoint.y, _maxBoundsPoint.z);
                if (_maxBoundsPoint.y < point.y)
                    _maxBoundsPoint = new Vector3(_maxBoundsPoint.x, point.y, _maxBoundsPoint.z);
                if (_maxBoundsPoint.z < point.z)
                    _maxBoundsPoint = new Vector3(_maxBoundsPoint.x, _maxBoundsPoint.y, point.z);
            }
            _boundsSize = Vector3.Distance(_minBoundsPoint, _maxBoundsPoint);
        }
        var randomPoint = new Vector3(Random.Range(_minBoundsPoint.x, _maxBoundsPoint.x),
            Random.Range(_minBoundsPoint.y, _maxBoundsPoint.y),
            Random.Range(_minBoundsPoint.z, _maxBoundsPoint.z)
        );
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, _boundsSize, 1);
        return hit.position;
        
    }
}
