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
            PlayerStateManager playerStateManager = other.GetComponentInParent<PlayerStateManager>();
            if(playerStateManager == null)
            {
                Debug.LogWarning("No Hay PlayerMovementHandler");
            }
            Debug.LogFormat("You got Hit by {0} with {1}", other.name, punchType);
            playerStateManager.IsGettingHit = true;
            playerStateManager.Health -= damage;
            playerStateManager.Animator.Play(getHitAnimation);

            Debug.LogFormat("Player Health: {0}", playerStateManager.Health);
        }
    }
}
