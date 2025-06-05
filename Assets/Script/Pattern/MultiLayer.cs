using System;
using UnityEngine;

[Flags]
public enum MultiMask
{
    Interactable,
    Wall,
    Weapon,
    Player,
    Damageable,
    EnemyDamageable,

}

public class MultiLayer : MonoBehaviour
{
    public MultiMask multiMask;
}
