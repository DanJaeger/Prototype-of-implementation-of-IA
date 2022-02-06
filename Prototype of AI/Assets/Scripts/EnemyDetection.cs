using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    FieldOfView fov;

    [SerializeField] LayerMask enemyMask;
    [SerializeField] LayerMask obstacleMask;

    GameObject closestEnemy = null;

    const float viewRadius = 5.0f;
    const float viewAngle = 75.0f;

    bool enemyInRadius = false;
    private void Start()
    {
        fov = GetComponent<FieldOfView>();
    }

    void Update()
    {
        FindVisibleTargets();

        fov.viewRadius = viewRadius;
        fov.viewAngle = viewAngle;
        fov.closestTarget = closestEnemy;
    }

    public void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, enemyMask);

        float distanceToClosestEnemy = Mathf.Infinity;

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    if (distanceToTarget < distanceToClosestEnemy)
                    {
                        distanceToClosestEnemy = distanceToTarget;
                        closestEnemy = target.gameObject;
                    }
                    enemyInRadius = (distanceToClosestEnemy <= viewRadius) ? true : false;
                }
                else
                {
                    closestEnemy = null;
                }
            }
            else
            {
                closestEnemy = null;
            }
        }
    }

    public GameObject GetClosestEnemy()
    {
        if (enemyInRadius)
            return closestEnemy;
        else
            return null;
    }
}
