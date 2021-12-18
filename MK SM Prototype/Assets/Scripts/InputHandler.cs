using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA
{
    public class InputHandler : MonoBehaviour
    {
        PlayerMovementHandler movementHandler;
        FightingSystem fightingSystem;

        Vector2 movementInput;
        bool jumpInput;
        bool lightPunch;
        bool heavyPunch;
        bool blockingPunch;

        void Awake()
        {
            movementHandler = GetComponent<PlayerMovementHandler>();
            fightingSystem = GetComponentInChildren<FightingSystem>();
        }

        void Update()
        {
            GetInput();
            UpdateInputs();
        }
        void GetInput()
        {
            movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            jumpInput = Input.GetButtonDown("Jump");

            lightPunch = Input.GetKeyDown(KeyCode.J);
            heavyPunch = Input.GetKeyDown(KeyCode.K);
            blockingPunch = Input.GetKey(KeyCode.P);

        }

        void UpdateInputs()
        {
            movementHandler.CurrentMovementInput = movementInput;
            movementHandler.IsJumpPressed = jumpInput;

            fightingSystem.LightPunch = lightPunch;
            fightingSystem.HeavyPunch = heavyPunch;
            fightingSystem.BlockingPunch = blockingPunch;
        }

    }
}
