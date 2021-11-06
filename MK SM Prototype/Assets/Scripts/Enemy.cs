using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA
{
    public class Enemy : MonoBehaviour
    {
        public float health;

        private void Start()
        {
            health = 20;
        }

        //public float Health { get => health; set => health = value; }

        private void Update()
        {
            if(health <= 0)
            {
                Debug.Log("AHhhhhhhhhhhhhhhhhhhhh");
                Destroy(this.gameObject);
            }
        }
    }
}
