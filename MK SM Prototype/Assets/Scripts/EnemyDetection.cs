using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GA { 
    public class EnemyDetection : MonoBehaviour
    {
        [SerializeField] LayerMask enemyLayers;
        Transform playerTransform = null;

        const float detectionRadius = 4.0f;
        const float viewAngle = 80.0f;
        float distanceToPunch;

        public static bool enemyInRadius = false;
        GameObject closestEnemy = null;
        void Start()
        {
            playerTransform = GetComponentInParent<PlayerMovementHandler>().transform;

            distanceToPunch = detectionRadius * 2.5f;
        }

        void Update()
        {
            CheckForEnemyInRadius();
        }
        void CheckForEnemyInRadius()
        {
            float distanceToClosestEnemy = Mathf.Infinity;
            Collider[] enemiesClose = Physics.OverlapSphere(playerTransform.position, detectionRadius, enemyLayers);

            foreach (Collider currentEnemy in enemiesClose)
            {
                float distanceToEnemy = (currentEnemy.transform.position - playerTransform.position).sqrMagnitude;
                if (distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy.gameObject;
                }
                enemyInRadius = (distanceToClosestEnemy <= distanceToPunch) ? true : false;
            }
        }
        public Vector3 GetTargetDirection()
        {
            Vector3 targetDirection = Vector3.zero;

            targetDirection = (closestEnemy != null) ? closestEnemy.transform.position - playerTransform.position : 
                playerTransform.forward;

            return targetDirection;
        }

        public bool EnemyInFieldOfView()
        {
            float angle = Vector3.Angle(playerTransform.forward, GetTargetDirection());
            if (angle < viewAngle)
                return true;
            else
                return false;    
        }

        private void OnDrawGizmosSelected()
        {
            if(closestEnemy != null && enemyInRadius)
                Gizmos.DrawLine(playerTransform.position, closestEnemy.transform.position);
            
        }
        

    }
}
