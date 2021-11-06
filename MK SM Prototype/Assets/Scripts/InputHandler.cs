using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA
{
    public class InputHandler : MonoBehaviour
    {
        StateManager states;
        FightingSystem fightingSystem;

        float verticalInput;
        float horizontalInput;
        bool jumpInput;
        bool lightPunch;
        bool heavyPunch;
        bool kick;

        private void Awake()
        {
            states = GetComponent<StateManager>();
            fightingSystem = GetComponentInChildren<FightingSystem>();
        }
        void Start()
        {
            states.Init();
            fightingSystem.Init();
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

            lightPunch = Input.GetKeyDown(KeyCode.J);

        }

        void UpdateStates()
        {
            states.verticalInput = verticalInput;
            states.horizontalInput = horizontalInput;
            states.isJumpPressed = jumpInput;

            states.Tick(Time.deltaTime);
            fightingSystem.Tick();

            fightingSystem.lightPunch = lightPunch;

        }

    }
}
