using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    FieldOfView fov;

    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;

    GameObject target = null;

    const float viewRadius = 5.0f;
    const float viewAngle = 75.0f;

    private void Start()
    {
        fov = GetComponent<FieldOfView>();
    }

    void Update()
    {
        FindVisibleTargets();

        fov.viewRadius = viewRadius;
        fov.viewAngle = viewAngle;
        fov.closestTarget = target;
    }

    public void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

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
                        this.target = target.gameObject;
                    }
                }
                else
                {
                    this.target = null;
                }
            }
            else
            {
                this.target = null;
            }
        }
    }
    public GameObject GetTarget()
    {
        return target;
    }
}
