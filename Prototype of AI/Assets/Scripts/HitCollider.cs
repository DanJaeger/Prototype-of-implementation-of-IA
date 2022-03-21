using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    [SerializeField] PunchType punchType;
    [SerializeField] int damage;
    [SerializeField] EnemyAnimations getHitAnimation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyStateManager enemyStateManager = other.GetComponentInParent<EnemyStateManager>();
            Animator enemyAnimator = other.GetComponentInParent<Animator>();
            Debug.LogFormat("I Hit {0} with {1}", other.name, punchType);
            enemyStateManager.GettingHit = true;
            enemyStateManager.Health -= damage;
            enemyAnimator.Play(getHitAnimation.ToString());

            Debug.LogFormat("Enemy Health: {0}", enemyStateManager.Health);
        }
    }
}
