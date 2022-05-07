using UnityEngine;

public class InputHandler : MonoBehaviour
{
    PlayerStateManager playerStateManager;

    Vector2 movementInput;
    bool jumpInput;
    bool lightPunch;

    void Awake()
    {
        playerStateManager = GetComponent<PlayerStateManager>();
    }

    void Update()
    {
        GetInput();
        if (!playerStateManager.IsGettingHit || !playerStateManager.IsFighting)
        {
            UpdateInputs();
        }
    }
    void GetInput()
    {
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        jumpInput = Input.GetButtonDown("Jump");

        lightPunch = Input.GetKeyDown(KeyCode.J);

    }

    void UpdateInputs()
    {
        playerStateManager.CurrentMovementInput = movementInput;
        playerStateManager.IsJumpPressed = jumpInput;
        playerStateManager.LightPunch = lightPunch;
    }
}
