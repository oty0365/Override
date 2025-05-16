using UnityEngine;

public class HealingPack : AInteractable
{
    public override void OnInteract()
    {
        var player = PlayerInfo.Instance;
        player.PlayerCurHp = player.PlayerMaxHp; 
    }
    public override void ExitInteract()
    {
        
    }
}
