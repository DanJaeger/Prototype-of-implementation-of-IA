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
        bool jumpInput;

        private void Awake()
        {
            states = GetComponent<StateManager>();
        }
        void Start()
        {
            states.Init();
        }

        void Update()
        {
            GetInput();
            UpdateStates();
            
        }


        void GetInput()
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
            jumpInput = Input.GetButtonDown("Jump");
        }

        void UpdateStates()
        {
            states.verticalInput = verticalInput;
            states.horizontalInput = horizontalInput;
            states.isJumpPressed = jumpInput;

            states.Tick(Time.deltaTime);

        }

    }
}
