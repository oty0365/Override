using System;
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

public abstract class AAttack : APoolingObject
{
    public ElementType type;
    public float damage;

    public void CastDamage(Enemy target)
    {
        target.Hit(damage);
    }
}
