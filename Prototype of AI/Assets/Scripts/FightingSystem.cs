using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FightingSystem : MonoBehaviour
{
    PlayerMovementHandler movementHandler;

    bool lightPunch;
    bool heavyPunch;
    bool blockingPunch;

    static int lightsPunchsCount;
    bool canIPunch;

    int lightAttackHash;
    int heavyAttackHash;
    int blockingPunchHash;

    public static bool isBlocking = false;
    public static bool isFighting = false;

    static bool canIMove = false;

    public bool LightPunch { get => lightPunch; set => lightPunch = value; }
    public bool HeavyPunch { get => heavyPunch; set => heavyPunch = value; }
    public bool BlockingPunch { get => blockingPunch; set => blockingPunch = value; }

    private void Start()
    {
        Init();
    }
    void Init()
    {
        movementHandler = GetComponentInParent<PlayerMovementHandler>();

        lightsPunchsCount = 0;
        canIPunch = true;

        lightAttackHash = Animator.StringToHash("LightAttack");
        heavyAttackHash = Animator.StringToHash("HeavyPunch");
        blockingPunchHash = Animator.StringToHash("IsBlocking");

    }
    void Update()
    {
        Tick();
    }

    void Tick()
    {
        if (lightPunch && !isBlocking && !movementHandler.IsJumpAnimating)
        {
            StartLightCombo();
        }

        if (heavyPunch && !isBlocking && !movementHandler.IsJumpAnimating)
        {
            StartHeavyPunch();
        }

        isBlocking = (blockingPunch && !isFighting) ? true : false;
        if (isBlocking)
            movementHandler.Anim.SetBool(blockingPunchHash, true);
        else
            movementHandler.Anim.SetBool(blockingPunchHash, false);

        if (canIMove)
        {
            movementHandler.MoveTowardsTarget();
            canIMove = false;
        }
    }

    void StartLightCombo()
    {
        if (canIPunch && lightsPunchsCount <= 3)
            ++lightsPunchsCount;

        if (lightsPunchsCount == 1)
        {
            movementHandler.Anim.SetInteger(lightAttackHash, 1);
            canIMove = true;
        }
        isFighting = true;
    }
    public void CheckLightCombo()
    {
        canIPunch = false;
        if (movementHandler.Anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && lightsPunchsCount == 1)
        {
            EndLightPunchCombo();
        }
        else if (movementHandler.Anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && lightsPunchsCount >= 2)
        {
            movementHandler.Anim.SetInteger(lightAttackHash, 2);
            canIMove = true;
            canIPunch = true;
        }
        else if (movementHandler.Anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && lightsPunchsCount == 2)
        {
            EndLightPunchCombo();
        }
        else if (movementHandler.Anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && lightsPunchsCount >= 3)
        {
            movementHandler.Anim.SetInteger(lightAttackHash, 3);
            canIPunch = false;
        }
        else if (movementHandler.Anim.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_3"))
        {
            EndLightPunchCombo();
        }

    }

    void EndLightPunchCombo()
    {
        movementHandler.Anim.SetInteger(lightAttackHash, 0);
        canIPunch = true;
        lightsPunchsCount = 0;

        isFighting = false;
    }

    void StartHeavyPunch()
    {
        if (canIPunch)
        {
            canIPunch = false;
            movementHandler.Anim.SetInteger(heavyAttackHash, 1);
            canIMove = true;
            isFighting = true;
        }
    }

    void CheckHeavyPunch()
    {
        if (movementHandler.Anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyPunch"))
        {
            movementHandler.Anim.SetInteger(heavyAttackHash, 0);
            canIPunch = true;
            isFighting = false;
        }
    }
}
