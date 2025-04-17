using System;
using UnityEngine;

[Flags]
public enum PlayerCommands
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    Dash = 1 << 4,
    Run = 1<<5
}

public enum PlayerBehave
{
    Idel,
    Walk,
    KnockBack,
    Dash
}

public class PlayerAnimator : MonoBehaviour
{
    public static PlayerAnimator Instance { get; private set; }

    [Header("�÷��̾� ������Ʈ")]
    public Animator ani;

    [Header("�÷��̾� ����")]
    public PlayerCommands playerCommands;
    public PlayerBehave playerBehave;

    private void Awake()
    {
        Instance = this;
    }
}
