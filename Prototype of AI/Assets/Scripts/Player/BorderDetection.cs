using UnityEngine;

public class BorderDetection : MonoBehaviour
{
    PlayerStateManager playerStateManager;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerStateManager = other.GetComponentInParent<PlayerStateManager>();
            playerStateManager.IsInBorderArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerStateManager = other.GetComponentInParent<PlayerStateManager>();
            playerStateManager.IsInBorderArea = false;
        }
    }

}
