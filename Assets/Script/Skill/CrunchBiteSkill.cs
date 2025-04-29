using UnityEngine;

public class CrunchBiteSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    public void Start()
    {

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["specialAttack1"]))
        {

        }
    }
    public override void UseSkill()
    {

    }

}
