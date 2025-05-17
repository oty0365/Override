using System;
using System.Collections;
using System.Collections.Generic;
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
    Dash,
    SpearRun
}

public class PlayerAnimator : HalfSingleMono<PlayerAnimator>
{

    [Header("플레이어 컴포넌트")]
    public Animator ani;
    public AnimatorOverrideController overrideController;



    [Header("플레이어 설정")]
    public PlayerCommands playerCommands;
    public PlayerBehave playerBehave;

    private void Start()
    {
        WeaponBase.canInput = true;
        
    }

    public void OverrideAnimation(AnimationClip[] newClips)
    {
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);

        for (int i = 0; i < newClips.Length && i < overrides.Count; i++)
        {
            overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, newClips[i]);
        }

        overrideController.ApplyOverrides(overrides);
    }
    public void Override(AnimationClip[] newClips,Transform pos,GameObject overrideable)
    {
        WeaponBase.canInput = false;
        PlayerMove.Instance.canInput = false;
        PlayerInfo.Instance.CurrentOverridingObject = overrideable;
        gameObject.transform.position = pos.position;
        PlayerCamera.Instance.SetZoom(2f, 7.8f);
        OverrideAnimation(newClips);
        ani.runtimeAnimatorController = overrideController;
        StartCoroutine(OverrideFlow());
    }
    private IEnumerator OverrideFlow()
    {
        yield return new WaitForSeconds(0.4f);
        PlayerCamera.Instance.SetZoom(4.5f, 8);
        WeaponBase.canInput = true;
        PlayerMove.Instance.canInput = true;
    }
}
