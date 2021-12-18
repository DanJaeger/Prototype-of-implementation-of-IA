using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA {
    public class BorderDetection : MonoBehaviour
    {
        PlayerMovementHandler movementHandler;

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                movementHandler = other.GetComponent<PlayerMovementHandler>();
                movementHandler.isInBorderArea = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                movementHandler = other.GetComponent<PlayerMovementHandler>();
                movementHandler.isInBorderArea = false;
            }
        }

    }
}
