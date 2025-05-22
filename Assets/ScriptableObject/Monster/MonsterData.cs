using System;
using UnityEngine;

[Serializable]
public class DropTable
{
    public GameObject Item;
    public float dropRate;
}

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterCode;
    public float maxHp;
    public float maxStamina;
    public float moveSpeed;
    public float recoveryTime;
    public DropTable[] dropTable;
}
