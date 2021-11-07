using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GA
{
    public class FightingSystem : MonoBehaviour
    {
        StateManager states;
        Transform playerTransform;

        [HideInInspector] public bool lightPunch;

        static int punchsCount;
        bool canIPunch;

        int lightAttackHash;

        [SerializeField] LayerMask enemyLayers;
        public float detectionRadius = 4.0f;
        float viewAngle = 70.0f;
        float distanceToPunch;
        public static bool enemyInRadius = false;
        [HideInInspector] public static GameObject closestEnemy = null;

        public void Init()
        {
            states = GetComponentInParent<StateManager>();
            playerTransform = states.gameObject.transform;

            distanceToPunch = detectionRadius * 2.5f;
            punchsCount = 0;
            canIPunch = true;

            lightAttackHash = Animator.StringToHash("LightAttack");

        }
        public void Tick(float d)
        {
            if (lightPunch)
            {
                StartCombo();
                states.isLightPunching = true;
            }
            CheckForEnemyInRadius();
        }

        void StartCombo()
        {
            if (canIPunch && punchsCount <=3)
                ++punchsCount;

            if (punchsCount == 1)
            {
                states.anim.SetInteger(lightAttackHash, 1);
            }
            states.isFighting = true;
        }
        public void CheckCombo()
        {
            canIPunch = false;

            if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && punchsCount == 1)
            {
                EndLightPunchCombo();
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && punchsCount >= 2)
            {
                states.anim.SetInteger(lightAttackHash, 2);
                canIPunch = true;
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && punchsCount == 2)
            {
                EndLightPunchCombo();
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && punchsCount >= 3)
            {
                states.anim.SetInteger(lightAttackHash, 3);
                canIPunch = false;
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_3"))
            {
                EndLightPunchCombo();
            }

        }

        void EndLightPunchCombo()
        {
            states.anim.SetInteger(lightAttackHash, 0);
            canIPunch = true;
            punchsCount = 0;

            states.isFighting = false;
            states.isLightPunching = false;
        }
        void CheckForEnemyInRadius()
        {
            float distanceToClosestEnemy = Mathf.Infinity;
            Collider[] enemiesClose = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayers);

            foreach(Collider currentEnemy in enemiesClose)
            {
                float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
                if(distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy.gameObject;
                }
                
                if (distanceToClosestEnemy <= distanceToPunch)
                    enemyInRadius = true;
                else
                    enemyInRadius = false;

            }
        }
        public Vector3 GetTargetDirection()
        {
            Vector3 targetDirection = Vector3.zero;
            if (closestEnemy != null)
            {
                targetDirection = closestEnemy.transform.position - playerTransform.position;
            }
            else
            {
                targetDirection = playerTransform.forward;
            }
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
            Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
            if(closestEnemy != null && enemyInRadius)
                Gizmos.DrawLine(this.transform.position, closestEnemy.transform.position);
            
        }

    }
}
