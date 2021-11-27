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
        bool blockingPunch;

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
            UpdateInputs();
        }


        void GetInput()
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
            jumpInput = Input.GetButtonDown("Jump");

            lightPunch = Input.GetKeyDown(KeyCode.J);
            heavyPunch = Input.GetKeyDown(KeyCode.K);
            blockingPunch = Input.GetKey(KeyCode.P);

        }

        void UpdateStates()
        {
            states.Tick(Time.deltaTime);
            fightingSystem.Tick(Time.deltaTime);
        }

        void UpdateInputs()
        {
            states.verticalInput = verticalInput;
            states.horizontalInput = horizontalInput;
            states.isJumpPressed = jumpInput;

            fightingSystem.lightPunch = lightPunch;
            fightingSystem.heavyPunch = heavyPunch;
            fightingSystem.blockingPunch = blockingPunch;
        }

    }
}
