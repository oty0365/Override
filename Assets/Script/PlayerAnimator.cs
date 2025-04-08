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

    [Header("플레이어 컴포넌트")]
    public Animator ani;

    [Header("플레이어 설정")]
    public PlayerCommands playerCommands;
    public PlayerBehave playerBehave;

    private void Awake()
    {
        Instance = this;
    }
}
