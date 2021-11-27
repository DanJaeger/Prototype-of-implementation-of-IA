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
        [HideInInspector] public bool heavyPunch;
        [HideInInspector] public bool blockingPunch;

        static int lightsPunchsCount;
        bool canIPunch;

        int lightAttackHash;
        int heavyAttackHash;
        int blockingPunchHash;

        [SerializeField] LayerMask enemyLayers;
        const float detectionRadius = 4.0f;
        const float viewAngle = 80.0f;
        float distanceToPunch;
        public static bool enemyInRadius = false;
        public static bool isBlocking = false;
        public static bool isFighting = false;
        [HideInInspector] public static GameObject closestEnemy = null;

        static bool canIMove = false;

        public void Init()
        {
            states = GetComponentInParent<StateManager>();
            playerTransform = states.gameObject.transform;

            distanceToPunch = detectionRadius * 2.5f;
            lightsPunchsCount = 0;
            canIPunch = true;

            lightAttackHash = Animator.StringToHash("LightAttack");
            heavyAttackHash = Animator.StringToHash("HeavyPunch");
            blockingPunchHash = Animator.StringToHash("IsBlocking");

        }

        public void Tick(float deltaTime)
        {
            if (lightPunch && !isBlocking)
            {
                StartLightCombo();
            }
        
            if (heavyPunch && !isBlocking)
            {
                StartHeavyPunch();
            }
        
            isBlocking = (blockingPunch && !isFighting) ? true : false;
            if (isBlocking)
                states.anim.SetBool(blockingPunchHash, true);
            else
                states.anim.SetBool(blockingPunchHash, false);

            CheckForEnemyInRadius();
            if (canIMove)
            {
                StartCoroutine(MoveForwardWhilePunching(deltaTime));
            }
        }

        void StartLightCombo()
        {
            if (canIPunch && lightsPunchsCount <=3)
                ++lightsPunchsCount;

            if (lightsPunchsCount == 1)
            {
                states.anim.SetInteger(lightAttackHash, 1);
                canIMove = true;
            }
            isFighting = true;
        }
        public void CheckLightCombo()
        {
            canIPunch = false;
            if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && lightsPunchsCount == 1)
            {
                EndLightPunchCombo();
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && lightsPunchsCount >= 2)
            {
                states.anim.SetInteger(lightAttackHash, 2);
                canIMove = true;
                canIPunch = true;
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && lightsPunchsCount == 2)
            {
                EndLightPunchCombo();
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && lightsPunchsCount >= 3)
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
            lightsPunchsCount = 0;

            isFighting = false;
        }

        void StartHeavyPunch()
        {
            if (canIPunch)
            {
                canIPunch = false;
                states.anim.SetInteger(heavyAttackHash, 1);
                canIMove = true;
                isFighting = true;
            }
        }
        
        void CheckHeavyPunch()
        {
            if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyPunch"))
            {
                states.anim.SetInteger(heavyAttackHash, 0);
                canIPunch = true;
                isFighting = false;
            }
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
            Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
            if(closestEnemy != null && enemyInRadius)
                Gizmos.DrawLine(this.transform.position, closestEnemy.transform.position);
            
        }

        IEnumerator MoveForwardWhilePunching(float deltaTime)
        {
            states.MoveForwardWhilePunching(deltaTime);
            yield return new WaitForSeconds(0.15f);
            canIMove = false;
        }
    }
}
