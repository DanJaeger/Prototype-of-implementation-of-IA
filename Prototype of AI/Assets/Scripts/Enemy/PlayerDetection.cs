using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    FieldOfView fov;
    EnemyStateManager enemyStateManager;

    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;

    [SerializeField] GameObject player = null;

    const float viewRadius = 5.0f;
    const float viewAngle = 75.0f;

    public GameObject Player { get => player;}

    private void Start()
    {
        fov = GetComponent<FieldOfView>();
        enemyStateManager = GetComponent<EnemyStateManager>();
    }

    void Update()
    {
        FindVisibleTargets();

        fov.viewRadius = viewRadius;
        fov.viewAngle = viewAngle;
        fov.closestTarget = player;
    }

    public void FindVisibleTargets()
    {
        player = null;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (enemyStateManager.GettingHit) { 
                player = target.gameObject;
                return;
            }

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    if (distanceToTarget < viewRadius)
                    {
                        player = target.gameObject;
                    }
                }

            }
        }
    }
}
