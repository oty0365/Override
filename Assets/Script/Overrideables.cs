using UnityEngine;

public class Overrideables : AInteractable
{
    public OverrideablesData characterData;
    public CharacterSkillData characterSkillData;
    [SerializeField] private ParticleSystem prt;
    private SpriteRenderer _sr;

    void Start()
    {
        MapCleaner.Instance.Add(gameObject);
        _sr = GetComponent<SpriteRenderer>();
        var main = prt.main;
        switch (characterData.rarity)
        {
            case Rarity.Common:
                main.startColor = Color.white;
                break;
            case Rarity.Rare:
                main.startColor = Color.green;
                break;
            case Rarity.Epic:
                main.startColor = Color.blue;
                break;
            case Rarity.Legendary:
                main.startColor = Color.yellow;
                break;
            case Rarity.Unknown:
                main.startColor = Color.red;
                break;
        }
    }

    void Update()
    {
        
    }
    public override void OnInteract()
    {
        PlayerAnimator.Instance.Override(characterData.animationClips,gameObject.transform,characterData.prefabObject);
        Destroy(gameObject);
    }
    public override void ExitInteract()
    {
        
    }
}
