using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ElementType
{
    None=0,
    Fire=1<<0,
    Ice=1<<1,
    Thunder=1<<2,
    Grond = 1<<3,
    Dark = 1<<4,
}
[Serializable]
public class Attack
{
    public ElementType type;
    public float damage;
    public float infinateTime = 0.1f;
    public Collider2D attackCollider;
}

public abstract class AAttack : APoolingObject
{
    public Attack attack;
    public List<Enemy> contactedEnemies;

    public void CastDamage(Enemy target)
    {
        target.Hit(attack.attackCollider,attack.damage,attack.infinateTime);
    }

    public override void OnDeathInit()
    {
        foreach(var i in contactedEnemies)
        {
            i.RemoveDamaging(attack.attackCollider);
        }
    }
}
