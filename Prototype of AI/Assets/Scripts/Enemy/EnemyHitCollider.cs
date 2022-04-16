using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitCollider : MonoBehaviour
{
    [SerializeField] PunchType punchType;
    [SerializeField] int damage;
    const string getHitAnimation = "GetHit";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovementHandler playerMovementHandler = other.GetComponentInParent<PlayerMovementHandler>();
            Animator playerAnimator = playerMovementHandler.Anim;
            if(playerMovementHandler == null)
            {
                Debug.LogWarning("No Hay PlayerMovementHandler");
            }
            if (playerAnimator == null)
            {
                Debug.LogWarning("No Hay PlayerAnimator");
            }
            Debug.LogFormat("You got Hit by {0} with {1}", other.name, punchType);
            playerMovementHandler.IsGettingHit = true;
            playerMovementHandler.Health -= damage;
            playerAnimator.Play(getHitAnimation);

            Debug.LogFormat("Player Health: {0}", playerMovementHandler.Health);
        }
    }
}
