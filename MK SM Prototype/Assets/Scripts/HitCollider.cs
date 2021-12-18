using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA {
    public class HitCollider : MonoBehaviour
    {
        [SerializeField] string punchName;
        [SerializeField] float damage;

        PlayerMovementHandler movementHandler;

        private void Start()
        {
            movementHandler = FindObjectOfType<PlayerMovementHandler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                Enemy enemy = other.GetComponentInParent<Enemy>();
                Debug.LogFormat("I Hit {0} with {1}", other.name, punchName);
                enemy.Health -= damage;
                Debug.LogFormat("Enemy Health: {0}", enemy.Health);
            }
        }
    }
}
