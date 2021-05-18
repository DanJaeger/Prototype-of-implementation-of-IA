using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA
{
    public class InputHandler : MonoBehaviour
    {
        StateManager states;

        float verticalInput;
        float horizontalInput;

        private void Awake()
        {
            states = GetComponent<StateManager>();
        }
        void Start()
        {
            states.Init();
        }

        private void Update()
        {
            //states.Tick(Time.deltaTime);
        }

        void FixedUpdate()
        {
            GetInput();
            UpdateStates();
            
        }


        void GetInput()
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
        }

        void UpdateStates()
        {
            states.verticalInput = verticalInput;
            states.horizontalInput = horizontalInput;

            if (horizontalInput != 0 || verticalInput != 0)
                states.isWalking = true;
            else
                states.isWalking = false;

            states.Tick(Time.deltaTime);
        }

    }
}
