using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA {
    public class BorderDetection : MonoBehaviour
    {
        StateManager states;

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                states = other.GetComponent<StateManager>();
                states.isInBorderArea = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                states = other.GetComponent<StateManager>();
                states.isInBorderArea = false;
            }
        }

    }
}
