using UnityEngine;

public class HealingPack : AInteractable
{
    public override void OnInteract()
    {
        var player = PlayerInfo.Instance;
        player.PlayerCurHp = player.playerMaxHp; 
    }
    public override void ExitInteract()
    {
        
    }
}
