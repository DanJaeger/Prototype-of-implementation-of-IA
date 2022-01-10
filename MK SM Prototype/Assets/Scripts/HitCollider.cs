using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA {
    public class HitCollider : MonoBehaviour
    {
        [SerializeField] PunchType punchType;
        [SerializeField] int damage;
        [SerializeField] EnemyAnimations getHitAnimation;
        Punch punch;
        private void Start()
        {
            punch = new Punch(punchType, damage, getHitAnimation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                Enemy enemyHealth = other.GetComponentInParent<Enemy>();
                EnemyStateManager enemyStateManager = other.GetComponentInParent<EnemyStateManager>();
                Animator enemyAnimator = other.GetComponentInParent<Animator>();
                Debug.LogFormat("I Hit {0} with {1}", other.name, punchType);
                enemyStateManager.GettingHit = true;
                enemyAnimator.Play(getHitAnimation.ToString());
                enemyHealth.Health -= damage;

                Debug.LogFormat("Enemy Health: {0}", enemyHealth.Health);
            }
        }
    }
}
