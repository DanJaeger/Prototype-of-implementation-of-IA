using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GA
{
    public class FightingSystem : MonoBehaviour
    {
        StateManager states;

        [HideInInspector] public bool lightPunch;

        static int punchsCount;
        bool canIPunch;

        int lightAttackHash;

        [SerializeField] Collider[] extremities = new Collider[6];

        [SerializeField] LayerMask enemyLayers;
        [SerializeField] float detectionRadius = 4.0f;
        float distanceToPunch;
        [SerializeField] bool isInRadius = false;
        Collider closestEnemy = null;
        public void Init()
        {
            states = GetComponentInParent<StateManager>();

            distanceToPunch = detectionRadius * 2.5f;
            punchsCount = 0;
            canIPunch = true;

            lightAttackHash = Animator.StringToHash("LightAttack");

            DisableColliders();
        }
        public void Tick()
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
                EnableCollider(1);
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
                EnableCollider(2);
                canIPunch = true;
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && punchsCount == 2)
            {
                EndLightPunchCombo();
            }
            else if (states.anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && punchsCount >= 3)
            {
                states.anim.SetInteger(lightAttackHash, 3);
                EnableCollider(5);
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

            DisableColliders();
        }

        void DisableColliders()
        {
            foreach(Collider tip in extremities)
            {
                tip.enabled = false;
            }
        }

        void EnableCollider(int index)
        {
            for(int i = 0; i < extremities.Length; ++i)
            {
                if(i == index)
                {
                    extremities[i].enabled = true;
                }
                else
                {
                    extremities[i].enabled = false;
                }

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
                    closestEnemy = currentEnemy;
                }
                
                if (distanceToClosestEnemy <= distanceToPunch)
                    isInRadius = true;
                else
                    isInRadius = false;

            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
            if(closestEnemy != null && isInRadius)
                Gizmos.DrawLine(this.transform.position, closestEnemy.transform.position);
            
        }

    }
}
