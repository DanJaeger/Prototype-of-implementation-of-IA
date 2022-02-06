using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch
{
    PunchType punchType;
    int damage;
    EnemyAnimations getHitAnimation;

    public Punch(PunchType punchType, int damage, EnemyAnimations animation)
    {
        this.punchType = punchType;
        this.damage = damage;
        this.getHitAnimation = animation;
    }
}
